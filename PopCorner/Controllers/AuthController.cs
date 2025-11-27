using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;

namespace PopCorner.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinarySrv;
        public AuthController(PopCornerDbContext dbContext, IUserRepository userRepository, IMapper mapper, ICloudinaryService cloudinarySrv)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginSync([FromBody] LoginDto dto)
        {
            try
            {
                var user = await dbContext.User.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if (user == null) {
                    return BadRequest("User is not exist");
                }

                var yes = PasswordHelper.Verify(user.PasswordHash, dto.Password);

                if (yes) { 
                    return Ok(user);
                }

                return BadRequest("Password is not correct");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
