using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;

namespace PopCorner.Controllers
{
    [Route("api/credit")]
    [ApiController]

    public class CreditController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IMovieRepository movieRepository;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinarySrv;
        public CreditController(PopCornerDbContext dbContext, IMovieRepository movieRepository, IMapper mapper, ICloudinaryService cloudinarySrv)
        {
            this.dbContext = dbContext;
            this.movieRepository = movieRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
        }

        [HttpGet("role")]
        public async Task<ActionResult> GetRoles() {
            var roles = dbContext.CreditRole.ToArray();

            return Ok(roles);
        }
    }
}
