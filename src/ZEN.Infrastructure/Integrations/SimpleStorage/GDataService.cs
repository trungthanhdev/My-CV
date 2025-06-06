using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using ZEN.Domain.Services;

namespace ZEN.Infrastructure.Integrations.SimpleStorage;

public class GDataService : ISimpleStorage
{
    private readonly IMinioClient _s3Client;
    private readonly ILogger<GDataService> _logger;
    private readonly string _baseResUrl;
    private readonly IConfiguration _configuration;

    public GDataService(
        ILogger<GDataService> logger,
        IConfiguration configuration,
        IMinioClient client)
    {
        this._configuration = configuration;
        this._logger = logger;
        this._s3Client = client;
        this._baseResUrl = configuration["SimpleStorage:Bucket0:PublicUrl"] ?? 
            this._s3Client.Config.Endpoint;
    }

    public async Task<IEnumerable<string>> GetBucketsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _s3Client.ListBucketsAsync(cancellationToken);
            return response.Buckets.Select(e => e.Name);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error when get buckets");
        }
        return Array.Empty<string>();
    }

    public async Task<string> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream data,
        long size,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(size)
                .WithContentType(contentType)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "x-amz-acl", "public-read" }
            });

            var response = await _s3Client.PutObjectAsync(putObjectArgs, cancellationToken);
            if (response.ResponseStatusCode == System.Net.HttpStatusCode.OK){
                _logger?.LogInformation("File uploaded successfully to bucket {BucketName} with object name {ObjectName}", bucketName, objectName);

                var defaultResUrl = _baseResUrl.StartsWith("https://") ?
                    _baseResUrl : $"{_baseResUrl}/{bucketName}";
                    
                string fileUrl = $"{defaultResUrl}/{objectName}";

                return fileUrl;
            }
            _logger?.LogError($"Error when call api with http status code {response.ResponseStatusCode}");
            return string.Empty;
        }
        catch (MinioException e)
        {
            _logger?.LogError(e, "Error occurred while uploading file to bucket {BucketName} with object name {ObjectName}", bucketName, objectName);
            return string.Empty;
        }
    }
    public async Task<(Stream, string)> GetFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get object metadata to retrieve content type
            var statObject = await _s3Client.StatObjectAsync(new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName), cancellationToken);

            var memoryStream = new MemoryStream();
            await _s3Client.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                }), cancellationToken);

            memoryStream.Seek(0, SeekOrigin.Begin);
            _logger?.LogInformation("File retrieved successfully from bucket {BucketName} with object name {ObjectName}", bucketName, objectName);
            return (memoryStream, statObject.ContentType);
        }
        catch (MinioException e)
        {
            _logger?.LogError(e, "Error occurred while retrieving file from bucket {BucketName} with object name {ObjectName}", bucketName, objectName);
            throw;
        }
    }
}
