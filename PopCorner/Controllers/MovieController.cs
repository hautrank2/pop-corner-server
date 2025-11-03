using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories;
using PopCorner.Repositories.Interfaces;
using System.Text.Json;

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

            var dto = new PaginationResponse<MovieDto>
            {
                Page = res.Page,
                PageSize = res.PageSize,
                Total = res.Total,
                TotalPage = res.TotalPage,
                Items = mapper.Map<List<MovieDto>>(res.Items)
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromForm] CreateMovieDto createMovieDto)
        {
            var poster = createMovieDto.Poster;
            var imgFiles = createMovieDto.ImgFiles;

            var countA = createMovieDto.ImgFiles?.Count ?? 0;
            var countB = Request.Form.Files.Count;

            Console.WriteLine($"File {countA} {countB}");


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

                if (posterUploadResult == null)
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
                movie.MovieActors = createMovieDto.ActorIds.Select(id => new MovieActor { ArtistId = id }).ToArray();

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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EditMovieDto dto)
        {
            try
            {
                var oldDto = await dbContext.Movies
                            .Include(m => m.MovieGenres)
                            .Include(m => m.MovieActors)
                            .FirstOrDefaultAsync(m => m.Id == id);

                if (oldDto == null)
                {
                    return BadRequest($"Movie with {id} is not exists");
                }
                // Check Director exist
                bool existDirector = await dbContext.Artist.AnyAsync(a => a.Id == dto.DirectorId);
                if (!existDirector)
                {
                    return BadRequest($"DirectorId {dto.DirectorId} is not exists");
                }

                // Sync Many to Many MovieGenre
                var newGenreIds = dto.GenreIds.ToHashSet();
                var oldGenreIds = oldDto.MovieGenres.Select(x => x.GenreId).ToHashSet();
                var toAddGenres = newGenreIds.Except(oldGenreIds)
                    .Select(gid => new MovieGenre { MovieId = id, GenreId = gid })
                    .ToList();
                var toRemoveGenres = oldDto.MovieGenres.Where(mg => !newGenreIds.Contains(mg.GenreId)).ToList();

                if (toRemoveGenres.Count > 0) dbContext.MovieGenre.RemoveRange(toRemoveGenres);
                if (toAddGenres.Count > 0) await dbContext.MovieGenre.AddRangeAsync(toAddGenres);

                // Sync Many to Many MovieActor
                var newActorIds = dto.ActorIds.ToHashSet();
                var oldActorIds = oldDto.MovieActors.Select(x => x.ArtistId).ToHashSet();

                var toAddActors = newActorIds.Except(oldActorIds)
                    .Select(aid => new MovieActor { MovieId = id, ArtistId = aid })
                    .ToList();
                var toRemoveActors = oldDto.MovieActors.Where(ma => !newActorIds.Contains(ma.ArtistId)).ToList();

                if (toRemoveActors.Count > 0) dbContext.MovieActor.RemoveRange(toRemoveActors);
                if (toAddActors.Count > 0) await dbContext.MovieActor.AddRangeAsync(toAddActors);

                var movie = mapper.Map<Movie>(dto);
                movie.PosterUrl = oldDto.PosterUrl;
                movie.ImgUrls = oldDto.ImgUrls;
                movie.UpdatedAt = DateTime.UtcNow;
                movie.MovieGenres = toAddGenres;
                movie.MovieActors = toAddActors;

                await dbContext.SaveChangesAsync();

                var result = await movieRepository.UpdateAsync(id, movie);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Create movie failed: {ex.Message}");
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

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var res = await movieRepository.GetByIdAsync(id);
            return Ok(res);
        }

        [HttpPut]
        [Route("{id:Guid}/poster")]
        public async Task<IActionResult> UpdatePoster([FromRoute] Guid id, [FromForm] MovieUpdatePosterDto dto)
        {
            try
            {
                var movie = await dbContext.Movies
                            .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    return NotFound($"Movie with id {id.ToString()} not exist");
                }

                IFormFile file = dto.Poster;
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file.");
                }

                if (!file.ContentType.StartsWith("image/"))
                {
                    return BadRequest("File must be an image.");
                }


                var imgRes = await fileRepository.UploadImage(new FileImage
                {
                    File = file,
                    FileDescription = "Movie poster",
                    FileExtension = Path.GetExtension(file.FileName),
                    FileSizeInBytes = file.Length,
                    FileName = Guid.NewGuid().ToString(),
                    Folder = "/movies"
                });

                await DeleteAvt(movie.PosterUrl);

                movie.UpdatedAt = DateTime.UtcNow;
                movie.PosterUrl = imgRes.FullPath;

                await dbContext.SaveChangesAsync();

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
