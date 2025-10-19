using Microsoft.AspNetCore.Mvc;
using Npgsql.BackendMessages;
using PopCorner.Models.Common;
using PopCorner.Models.DTOs;
using PopCorner.Repositories;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly IFileRepository fileRepository;
        public FileController(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        [HttpPost]
        [Route("upload-image")]
        public async Task<IActionResult> UploadFile([FromForm] CreateFileImageDto dto)
        {
            var body = new FileImage
            {
                File = dto.File,
                FileExtension = Path.GetExtension(dto.File.FileName),
                FileSizeInBytes = dto.File.Length,
                FileName = dto.FileName,
                FileDescription = dto.FileDescription,
                Folder = dto.Folder
            };
            var result = await fileRepository.UploadImage(body);

            return Ok(result);
        }
    }
}
