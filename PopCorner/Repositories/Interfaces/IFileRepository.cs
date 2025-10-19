using PopCorner.Models.Common;
using PopCorner.Models.DTOs;

namespace PopCorner.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<FileImage> UploadImage(FileImage image);
        Task<bool> RemoveFileByPathName(string pathname);

    }
}
