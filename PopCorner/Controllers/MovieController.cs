using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
using PopCorner.Models.DTOs;
using PopCorner.Models.Common;
using PopCorner.Repositories.Interfaces;
using PopCorner.Models.Domains;

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
        public async Task<IActionResult> GetAll()
        {
            var movies = await movieRepository.GetAllAsync();

            return Ok(movies);
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
    }
}
