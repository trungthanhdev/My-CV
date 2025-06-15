using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using ZEN.Domain.Interfaces;

namespace ZEN.Infrastructure.Integrations.CloudStorage
{
    public class SavePhotoToCloud : ISavePhotoToCloud
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudName;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public SavePhotoToCloud()
        {
            Env.Load();
            _cloudName = Env.GetString("CLOUDNAME");
            _apiKey = Env.GetString("APIKEY");
            _apiSecret = Env.GetString("APISECRET");
            var acc = new Account(_cloudName, _apiKey, _apiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<bool> DeletePhotoAsync(string? img_url)
        {
            if (string.IsNullOrWhiteSpace(img_url))
                return false;

            var uri = new Uri(img_url);
            var segments = uri.Segments;

            // Tìm vị trí upload/
            var uploadIndex = Array.FindIndex(segments, s => s.Trim('/').Equals("upload", StringComparison.OrdinalIgnoreCase));
            if (uploadIndex == -1)
                throw new Exception("Invalid Cloudinary URL format");

            // Lấy phần sau upload/ và sau version vxxxx
            var publicIdSegments = segments.Skip(uploadIndex + 2) // bỏ cả version vxxxx/
                                            .Select(s => Uri.UnescapeDataString(s.Trim('/'))) // giải mã các kí tự encode như %20 thành khoảng trắng
                                            .ToArray();

            var fullFileName = string.Join("/", publicIdSegments); // vì có thể có folder
            var extensionIndex = fullFileName.LastIndexOf('.');
            if (extensionIndex >= 0)
            {
                fullFileName = fullFileName.Substring(0, extensionIndex); // bỏ phần mở rộng
            }

            var deleteParams = new DeletionParams(fullFileName);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        public async Task<string> UploadPhotoAsync(Stream fileStream, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var compressedStream = await CompressImageAsync(memoryStream);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, compressedStream),
                    PublicId = fileName,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Failed to upload image");

                return uploadResult.SecureUrl.AbsoluteUri;
            }
        }

        private async Task<Stream> CompressImageAsync(Stream inputStream)
        {
            inputStream.Position = 0;

            using var image = await Image.LoadAsync(inputStream);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new SixLabors.ImageSharp.Size(1024, 1024)
            }));

            var outputStream = new MemoryStream();
            await image.SaveAsJpegAsync(outputStream, new JpegEncoder
            {
                Quality = 75
            });

            outputStream.Position = 0;
            return outputStream;
        }
    }
}
