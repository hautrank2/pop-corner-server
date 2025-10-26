using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Controllers
{
    [Route("api/genre")]
    [ApiController]
    public class GenreController : Controller
    {
        private readonly IGenreRepository genreRepository;
        public GenreController(IGenreRepository genreRepository) {
            this.genreRepository = genreRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await genreRepository.GetAllAsync();
            return Ok(genres);
        }
    }
}
