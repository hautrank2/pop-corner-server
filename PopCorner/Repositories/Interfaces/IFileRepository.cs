using PopCorner.Models.Common;

namespace PopCorner.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<FileImage> UploadImage(FileImage image);
    }
}
