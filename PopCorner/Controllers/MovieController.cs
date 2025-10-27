using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IMovieRepository movieRepository;
        private readonly IFileRepository fileRepository;
        private readonly IMapper mapper;
        public MovieController(PopCornerDbContext dbContext, IMovieRepository movieRepository, IFileRepository fileRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.movieRepository = movieRepository;
            this.fileRepository = fileRepository;
            this.mapper = mapper;
        }

        // GET: MovieController
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MovieQueryDto movieQuery)
        {
            var res = await movieRepository.GetAllAsync(movieQuery);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromForm] CreateMovieDto createMovieDto)
        {
            var poster = createMovieDto.Poster;
            var imgFiles = createMovieDto.ImgFiles;

            FileImage? posterUploadResult = null;
            List<FileImage> uploadedImages = new();

            try
            {
                // Check Director exist
                bool existDirector = await dbContext.Artist.AnyAsync(a => a.Id == createMovieDto.DirectorId);
                if (!existDirector)
                {
                    return BadRequest($"DirectorId {createMovieDto.DirectorId} is not exists");
                }

                posterUploadResult = await fileRepository.UploadImage(new FileImage
                {
                    File = poster,
                    FileExtension = Path.GetExtension(poster.FileName),
                    FileSizeInBytes = poster.Length,
                    FileName = Guid.NewGuid().ToString(),
                    FileDescription = "Movie Poster",
                    Folder = "/movies"
                });

                if(posterUploadResult == null)
                {
                    return BadRequest("Upload Poster failed");
                }

                List<Task<FileImage>> uploadImgTasks = new List<Task<FileImage>>();
                foreach (var img in createMovieDto.ImgFiles)
                {
                    uploadImgTasks.Add(fileRepository.UploadImage(new FileImage
                    {
                        File = img,
                        FileExtension = Path.GetExtension(img.FileName),
                        FileSizeInBytes = img.Length,
                        FileName = Guid.NewGuid().ToString(),
                        FileDescription = "Movie Poster",
                        Folder = "/movies"
                    }));
                }
                FileImage[] imgUploadResult = await Task.WhenAll(uploadImgTasks);
                uploadedImages = imgUploadResult.ToList();

                if (imgUploadResult.Length != imgFiles.Count)
                {
                    return BadRequest("Upload Image failed");
                }

                var movie = mapper.Map<Movie>(createMovieDto);
                movie.PosterUrl = posterUploadResult.FullPath;
                movie.ImgUrls = uploadedImages.Select(i => i.FullPath).ToArray();
                movie.CreatedAt = DateTime.UtcNow;
                movie.UpdatedAt = DateTime.UtcNow;
                movie.MovieGenres = createMovieDto.GenreIds.Select(id => new MovieGenre { GenreId = id }).ToArray();

                var createMovieResult = await movieRepository.CreateAsync(movie);
                return Ok(createMovieResult);
            }
            catch (Exception ex)
            {
                var removeErrors = new List<string>();

                try
                {
                    if (posterUploadResult != null)
                    {
                        var removed = await fileRepository.RemoveFileByPathName(posterUploadResult.FullPath);
                        if (!removed)
                            removeErrors.Add($"Remove poster failed: {posterUploadResult.FullPath}");
                    }

                    foreach (var fi in uploadedImages)
                    {
                        var removed = await fileRepository.RemoveFileByPathName(fi.FullPath);
                        if (!removed)
                            removeErrors.Add($"Không thể xóa ảnh: {fi.FullPath}");
                    }
                }
                catch (Exception innerEx)
                {
                    removeErrors.Add($"Remoive Image files failed: {innerEx.Message}");
                }

                var removeMessage = removeErrors.Count > 0
                    ? string.Join("\n", removeErrors)
                    : "Rollback successfully (all files deleted).";

                return BadRequest($"Create movie failed: {ex.Message}\n{removeMessage}");
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var entity = await movieRepository.DeleteAsync(id);

                if (entity == null)
                {
                    return NotFound($"Can't find artist with id {id}");
                }
                var (deleted, error) = await DeleteAvt(entity.PosterUrl);

                if (!deleted)
                {
                    return StatusCode(207, new
                    {
                        message = "Movie deleted, but failed to remove poster imagee.",
                        fileError = error,
                        code = "FILE_DELETE_FAILED",
                        data = entity
                    });
                }

                var failedDeletes = new List<object>();

                foreach (var imgUrl in entity.ImgUrls)
                {
                    var (deletedImg, imgError) = await DeleteAvt(imgUrl);
                    if (!deletedImg)
                    {
                        failedDeletes.Add(new
                        {
                            imgUrl,
                            error = imgError
                        });
                    }
                }

                // Nếu có ảnh xóa thất bại => trả 207 Multi-Status
                if (failedDeletes.Any())
                {
                    return StatusCode(207, new
                    {
                        message = "Movie deleted, but some images failed to delete.",
                        failedImages = failedDeletes,
                        code = "FILE_DELETE_PARTIAL",
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
                var ok = await fileRepository.RemoveFileByPathName(url);
                return (ok, ok ? "null" : "Unknown error when removing file");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
