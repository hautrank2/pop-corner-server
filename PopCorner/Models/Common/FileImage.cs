
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
        public string FilePath { get; set; } = string.Empty;

        [DefaultValue("/folder1/folder2")]
        public string Folder { get; set; } = string.Empty;
        public string FullPath { get; set; }

        // bạn có thể thêm các property riêng cho class này
        public Guid MovieId { get; set; }
    }
}
