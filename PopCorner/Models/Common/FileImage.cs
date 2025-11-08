
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PopCorner.Models.Common
{
    public class FileImage : IFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [NotMapped]
        public IFormFile File { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string? FileDescription { get; set; }
        public string FileExtension { get; set; } = string.Empty;
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; } = string.Empty; // /artist/faa8a3c9-1031-4ef4-9693-403fd57cf6f7.jpg

        [DefaultValue("/folder1/folder2")]
        public string Folder { get; set; } = string.Empty;

        // bạn có thể thêm các property riêng cho class này
        public Guid MovieId { get; set; }
    }
}
