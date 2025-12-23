using TKH.Core.Common.Enums;

namespace TKH.Core.Utilities.Storage
{
    public interface IStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string folder, FileNamingMode fileNamingMode = FileNamingMode.Unique, string? customName = null);
        Task DeleteAsync(string folder, string fileName);
        Task RenameAsync(string folder, string oldFileName, string newFileName);
        string GetUrl(string folder, string fileName);
    }
}
