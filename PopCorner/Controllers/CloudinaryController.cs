using Microsoft.AspNetCore.Mvc;
using PopCorner.Models.Common;
using PopCorner.Models.DTOs;
using PopCorner.Service.Interfaces;

namespace PopCorner.Controllers
{
    [Route("/api/cloudinary")]
    [ApiController]
    public class CloudinaryController : Controller
    {

        private readonly ICloudinaryService cloudnaryService;
        public CloudinaryController(ICloudinaryService cloudnaryService)
        {
            this.cloudnaryService = cloudnaryService;
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
            var result = await cloudnaryService.UploadImage(body);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> UploadFile([FromQuery] RemoveFileDto dto)
        {
            Console.WriteLine($"Delete publicID {dto.Pathname}");
            var result = await cloudnaryService.RemoveFileByPathName(dto.Pathname);
            return Ok(result);
        }
    }
}
