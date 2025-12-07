using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;
using PopCorner.Services;

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
        public async Task<IActionResult> LoginSync([FromBody] LoginDto dto, [FromServices] JwtService jwt)
        {
            try
            {
                var user = await dbContext.User.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if (user == null) {
                    return BadRequest("User is not exist");
                }

                var yes = PasswordHelper.Verify(dto.Password, user.PasswordHash);
                var token = jwt.GenerateToken(user.Id, user.Email, user.Role);
                
                if (yes) {
                    var response = new LoginResponseDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        AvatarUrl = user.AvatarUrl,
                        Role = user.Role,
                        Token = token
                    };

                    return Ok(response);
                }

                return BadRequest("Invalid Email or password");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login/email")]
        public async Task<IActionResult> LoginEmailSync([FromBody] LoginEmailDto dto, [FromServices] JwtService jwt)
        {
            try
            {
                var user = await dbContext.User.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if (user == null)
                {
                    return BadRequest("User is not exist");
                }
                var token = jwt.GenerateToken(user.Id, user.Email, user.Role);
                var response = new LoginResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    AvatarUrl = user.AvatarUrl,
                    Role = user.Role,
                    Token = token
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
    }
}
