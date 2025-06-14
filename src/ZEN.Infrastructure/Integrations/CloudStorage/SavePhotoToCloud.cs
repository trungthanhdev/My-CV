using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using Microsoft.Extensions.Options;
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
        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        public async Task<string> UploadPhotoAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = fileName,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Failed to upload image");

            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}