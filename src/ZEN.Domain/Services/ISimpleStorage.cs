namespace ZEN.Domain.Services;
public interface ISimpleStorage
{
    /// <returns>
    /// list buckets name
    /// </returns>
    Task<IEnumerable<string>> GetBucketsAsync(CancellationToken cancellationToken = default);

    /// <returns>file path</returns>
    Task<string> UploadFileAsync(
        string bucketName,
        string objectName,
        Stream data,
        long size,
        string contentType,
        CancellationToken cancellationToken = default);
    
    /// <returns>
    /// Stream: Datafile
    /// string: ContentType
    /// </returns>
    Task<(Stream, string)> GetFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default);
}