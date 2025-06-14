using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Domain.Interfaces
{
    public interface ISavePhotoToCloud
    {
        Task<string> UploadPhotoAsync(Stream fileStream, string fileName);
        Task<bool> DeletePhotoAsync(string publicId);
    }
}