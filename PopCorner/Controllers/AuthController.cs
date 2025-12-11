using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;
using PopCorner.Services;
using PopCorner.Services.Interfaces;

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
        private readonly ISessionService sessionService;
        public AuthController(PopCornerDbContext dbContext, IUserRepository userRepository, IMapper mapper, ICloudinaryService cloudinarySrv, ISessionService sessionService)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
            this.sessionService = sessionService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginSync([FromBody] LoginDto dto, [FromServices] IJwtService jwt)
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
                        Birthday = user.Birthday,
                        Role = user.Role,
                        Token = token,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
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
        public async Task<IActionResult> LoginEmailSync([FromBody] LoginEmailDto dto, [FromServices] IJwtService jwt)
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
                    Birthday = user.Birthday,
                    Role = user.Role,
                    Token = token,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] EditPasswordDto dto)
        {
            try
            {
                var userId = sessionService.UserId;

                var user = await userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var yes = PasswordHelper.Verify(dto.Password, user.PasswordHash);

                if (!yes)
                {
                    return BadRequest("Password incorrect");
                }

                var hashPassword = PasswordHelper.Hash(dto.NewPassword);
                user.PasswordHash = hashPassword;
                await userRepository.UpdateAsync(userId, user);
                return Ok(user);
            }
            catch (Exception ex) 
            {
                return BadRequest("Update failed");
            }
        }
    }
}
