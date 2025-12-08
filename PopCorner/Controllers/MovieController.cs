using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;
using PopCorner.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PopCorner.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IMovieRepository movieRepository;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinarySrv;
        private readonly ICommentRepository commentRepository;
        private readonly ISessionService sessionService;

        public MovieController(PopCornerDbContext dbContext, IMovieRepository movieRepository, IMapper mapper, ICloudinaryService cloudinarySrv, ICommentRepository commenRepository, ISessionService sessionService)
        {
            this.dbContext = dbContext;
            this.movieRepository = movieRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
            this.commentRepository = commenRepository;
            this.sessionService = sessionService;
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

                posterUploadResult = await cloudinarySrv.UploadImage(new FileImage
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
                    uploadImgTasks.Add(cloudinarySrv.UploadImage(new FileImage
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

                movie.PosterUrl = posterUploadResult.FilePath;
                movie.ImgUrls = uploadedImages.Select(i => i.FilePath).ToArray();
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
                        var removed = await cloudinarySrv.RemoveFileByPathName(posterUploadResult.FilePath);
                        if (!removed)
                            removeErrors.Add($"Remove poster failed: {posterUploadResult.FilePath}");
                    }

                    foreach (var fi in uploadedImages)
                    {
                        var removed = await cloudinarySrv.RemoveFileByPathName(fi.FilePath);
                        if (!removed)
                            removeErrors.Add($"Không thể xóa ảnh: {fi.FilePath}");
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


                // State: handle MovieCredits
                if (dto.Credits != null)
                {
                    var oldCredits = await dbContext.MovieCredit
                        .Where(e => e.MovieId == id)
                        .ToListAsync();

                    if (oldCredits.Count > 0)
                        dbContext.MovieCredit.RemoveRange(oldCredits);

                    var newCredits = dto.Credits.Select(c => new MovieCredit
                    {
                        MovieId = id,
                        ArtistId = c.ArtistId,
                        CreditRoleId = c.CreditRoleId,
                        CharacterName = c.CharacterName,
                        Order = c.Order
                    }).ToList();

                    await dbContext.MovieCredit.AddRangeAsync(newCredits);
                }
                await dbContext.SaveChangesAsync();

                var movie = mapper.Map<Movie>(dto);
                movie.PosterUrl = oldDto.PosterUrl;
                movie.ImgUrls = oldDto.ImgUrls;
                movie.UpdatedAt = DateTime.UtcNow;
                movie.MovieGenres = toAddGenres;
                var result = await movieRepository.UpdateAsync(id, movie);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Create movie failed: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}/rate")]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] AddMovieRateDto dto)
        {
            try
            {
                var movie = await movieRepository.GetByIdAsync(id);

                if (movie == null) 
                {
                    return BadRequest("Movie not found");
                }


            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            return Ok();
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


                var imgRes = await cloudinarySrv.UploadImage(new FileImage
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
                movie.PosterUrl = imgRes.FilePath;

                await dbContext.SaveChangesAsync();

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("{id:Guid}/images")]
        public async Task<IActionResult> AddImage([FromRoute] Guid id, [FromForm] FilesDto dto)
        {
            try
            {
                var movie = await dbContext.Movies.FirstOrDefaultAsync(m => m.Id == id);
                if (movie == null)
                {
                    return NotFound($"Movie with id {id} not exist");
                }

                // Lấy danh sách file upload
                var files = dto.Files;
                if (files == null || !files.Any())
                {
                    return BadRequest("No files uploaded.");
                }

                // Đảm bảo mảng ImgUrls không null
                var imgUrls = (movie.ImgUrls ?? Array.Empty<string>()).ToList();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                        return BadRequest("File is empty.");

                    if (!file.ContentType.StartsWith("image/"))
                        return BadRequest("All files must be images.");

                    var imgRes = await cloudinarySrv.UploadImage(new FileImage
                    {
                        File = file,
                        FileDescription = "Movie image",
                        FileExtension = Path.GetExtension(file.FileName),
                        FileSizeInBytes = file.Length,
                        FileName = Guid.NewGuid().ToString(),
                        Folder = "/movies"
                    });

                    imgUrls.Add(imgRes.FilePath);
                }

                movie.ImgUrls = imgUrls.ToArray();
                movie.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:Guid}/images/{imgIndex:int}")]
        public async Task<IActionResult> DeleteImage([FromRoute] Guid id, [FromRoute] int imgIndex)
        {
            try
            {
                var movie = await dbContext.Movies
                            .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    return NotFound($"Movie with id {id.ToString()} not exist");
                }

                if (imgIndex < 0 || movie.ImgUrls == null || imgIndex >= movie.ImgUrls.Length)
                {
                    return BadRequest("Image index invalid");
                }

                var deleteImgUrl = movie.ImgUrls[imgIndex];
                await DeleteAvt(deleteImgUrl);

                var imgUrls = movie.ImgUrls.ToList();

                imgUrls.RemoveAt(imgIndex);
                movie.UpdatedAt = DateTime.UtcNow;
                movie.ImgUrls = imgUrls.ToArray();

                await dbContext.SaveChangesAsync();

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id:Guid}/comment")]
        public async Task<IActionResult> GetComments([FromRoute] Guid id)
        {
            var data = await movieRepository.GetCommentsAsync(id);
            return Ok(data);
        }

        [HttpPost("{id:Guid}/comment")]
        public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] AddMovieCommentDto query)
        {
            var body = mapper.Map<Comment>(query);
            var userId = sessionService.UserId;
            body.UserId = userId;
            body.MovieId = id;
            body.IsEdited = false;
            var data = await commentRepository.CreateAsync(body);
            return Ok(data);
        }

        [HttpGet("{id:Guid}/rate")]
        public async Task<IActionResult> GetRateSync([FromRoute] Guid id)
        {
            var movieRes = await movieRepository.GetRatingSync(id);
            return Ok(movieRes);
        }


        [Authorize]
        [HttpPost("{id:Guid}/rate")]
        public async Task<IActionResult> Rate([FromRoute] Guid id, [FromBody] MovieRateDto dto)
        {
            var userId = sessionService.UserId;
            var movieRes = await movieRepository.Rate(id, userId, dto);
            return Ok(movieRes);
        }

        [Authorize]
        [HttpPost("{id:Guid}/reaction")]
        public async Task<IActionResult> React([FromRoute] Guid id, [FromBody] UpsertMovieReactionDto dto)
        {
            var userId = sessionService.UserId;
            var reaction = mapper.Map<MovieReaction>(dto);

            reaction.UserId = userId;
            reaction.MovieId = id;
            reaction.CreatedAt = DateTime.UtcNow;

            var res = await movieRepository.AddReactionSync(reaction);

            return Ok(res);
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
