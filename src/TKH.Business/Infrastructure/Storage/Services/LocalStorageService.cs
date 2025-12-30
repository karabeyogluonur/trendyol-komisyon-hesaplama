using Microsoft.AspNetCore.Hosting;
using TKH.Core.Common.Constants;
using TKH.Core.Common.Enums;
using TKH.Core.Utilities.Storage;

namespace TKH.Business.Infrastructure.Storage.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;
        private const string RootContainerName = "uploads";

        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder, FileNamingMode fileNamingMode = FileNamingMode.Unique, string? customName = null)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Stream boş olamaz.", nameof(fileStream));

            string path = Path.Combine(_env.WebRootPath, RootContainerName, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string finalFileName = GenerateFileName(fileName, fileNamingMode, customName);
            string fullPath = Path.Combine(path, finalFileName);

            using (FileStream fileSystemStream = new FileStream(fullPath, FileMode.Create))
            {
                if (fileStream.CanSeek)
                    fileStream.Position = 0;

                await fileStream.CopyToAsync(fileSystemStream);
            }

            return finalFileName;
        }

        private string GenerateFileName(string originalFileName, FileNamingMode fileNamingMode, string? customName)
        {
            string extension = Path.GetExtension(originalFileName);
            string baseName = !string.IsNullOrEmpty(customName) ? customName.ToUrlSlug() : Path.GetFileNameWithoutExtension(originalFileName).ToUrlSlug();

            switch (fileNamingMode)
            {
                case FileNamingMode.Specific:
                    return $"{baseName}{extension}";

                case FileNamingMode.Unique:
                default:
                    string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
                    return $"{baseName}-{uniqueSuffix}{extension}";
            }
        }

        public Task DeleteAsync(string folder, string fileName)
        {
            string fullPath = Path.Combine(_env.WebRootPath, RootContainerName, folder, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        public Task RenameAsync(string folder, string oldFileName, string newFileName)
        {
            string path = Path.Combine(_env.WebRootPath, RootContainerName, folder);
            string oldPath = Path.Combine(path, oldFileName);
            string newPath = Path.Combine(path, newFileName);

            if (File.Exists(oldPath))
                File.Move(oldPath, newPath);

            return Task.CompletedTask;
        }

        public string GetUrl(string folder, string fileName)
        {
            return $"/{ApplicationDefaults.BaseUploadsPath}/{folder}/{fileName}".Replace("\\", "/");
        }
    }
}
