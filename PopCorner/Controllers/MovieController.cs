using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IMovieRepository movieRepository;
        private readonly IMapper mapper;
        public MovieController(PopCornerDbContext dbContext, IMovieRepository movieRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.movieRepository = movieRepository;
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
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto createMovieDto)
        {
            var poster = createMovieDto.Poster;
            var imgFiles = createMovieDto.ImgFiles;

            try
            {
                
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
    }
}
