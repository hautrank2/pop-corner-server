using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PopCorner.Controllers
{
    [Route("/api/artist")]
    [ApiController]
    public class ArtistController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IArtistRepository artistRepository;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinarySrv;
        public ArtistController(PopCornerDbContext dbContext, IArtistRepository artistRepository, IMapper mapper, ICloudinaryService cloudinarySrv)
        {
            this.dbContext = dbContext;
            this.artistRepository = artistRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ArtistQueryDto query)
        {
            var list = await artistRepository.GetAllAsync(query);
            return Ok(list);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateArtistDto dto)
        {
            var avt = dto.Avatar;
            FileImage? avtRes = null;
            try
            {
                avtRes = await cloudinarySrv.UploadImage(new FileImage { File = avt, FileDescription = "artist avatar", FileName = Guid.NewGuid().ToString(), Folder = "/artist", FileExtension = Path.GetExtension(avt.FileName) });
                var body = mapper.Map<Artist>(dto);
                body.AvatarUrl = avtRes.FilePath;
                var res = await artistRepository.CreateAsync(body);
                return Ok(res);
            }
            catch (Exception ex)
            {
                if (avtRes != null)
                {
                    await cloudinarySrv.RemoveFileByPathName(avtRes.FilePath);
                }

                return BadRequest($"Create movie failed: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var artist = await artistRepository.GetByIdAsync(id);
                return Ok(artist);
            } catch (Exception ex)
            {
                return BadRequest($"Create movie failed: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] EditArtistDto dto)
        {
            IFormFile? avt = dto.Avatar;
            FileImage? avtRes = null;
            var isNewAvt = avt is { Length: > 0 };
            try
            {
                var entity = await artistRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    return NotFound($"Can't find artist with id {id}");
                };

                var oldAvtUrl = entity.AvatarUrl;

                // Update new image
                if (isNewAvt)
                {
                    avtRes = await cloudinarySrv.UploadImage(new FileImage 
                        { 
                            File = avt, 
                            FileDescription = "artist avatar",
                            FileName = Guid.NewGuid().ToString(), 
                            Folder = "/artist", 
                            FileExtension = Path.GetExtension(avt.FileName) 
                        }
                    );

                }
                var body = mapper.Map<Artist>(dto);
                body.AvatarUrl = avtRes != null ? avtRes.FilePath : oldAvtUrl;
                var res = await artistRepository.UpdateAsync(id, body);

                // Remove old avatar
                if (res != null && avtRes != null && oldAvtUrl != null)
                {
                    var (deleted, error) = await DeleteAvt(oldAvtUrl);

                    if (!deleted)
                    {
                        return StatusCode(207, new
                        {
                            message = "Artist deleted, but failed to remove avatar file.",
                            fileError = error,
                            code = "FILE_DELETE_FAILED",
                            data = entity
                        });
                    }
                }
               
                return Ok(res);
            }
            catch (Exception ex)
            {
                if (avtRes != null)
                {
                    await cloudinarySrv.RemoveFileByPathName(avtRes.FilePath);
                }

                return BadRequest($"Create movie failed: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var entity = await artistRepository.DeleteAsync(id);

                if (entity == null)
                {
                    return NotFound($"Can't find artist with id {id}");
                }
                var (deleted, error) =  await DeleteAvt(entity.AvatarUrl);

                if(!deleted)
                {
                    return StatusCode(207, new
                    {
                        message = "Artist deleted, but failed to remove avatar file.",
                        fileError = error,
                        code = "FILE_DELETE_FAILED",
                        data = entity
                    });
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest($"Create movie failed: {ex.Message}");
            }

        }

        async Task<(bool deleted, string? error)> DeleteAvt(string url)
        {
            try
            {
                var ok = await cloudinarySrv.RemoveFileByPathName(url);
                return (ok, ok ? "null" : "Unknown error when removing file");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
