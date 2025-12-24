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
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

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
        private readonly IAuthService authService;
        private readonly IEmailService emailService;
        public AuthController(PopCornerDbContext dbContext, IUserRepository userRepository, IMapper mapper, ICloudinaryService cloudinarySrv, ISessionService sessionService, IAuthService authService, IEmailService emailService)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
            this.sessionService = sessionService;
            this.authService = authService;
            this.emailService = emailService;
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            try
            {
                var email = dto.Email;
                var user = await userRepository.GetByEmail(email);
                var response = Ok(new { ok = true });

                if (user == null)
                {
                    // always return Ok to prevent searching email
                    return response;
                }

                var otp = await authService.CreateOtpAsync(email);
                var hashOtp = await authService.HashOtp(otp);
                var key = GetKeyOtp(email);
                await authService.SaveOtp(key, hashOtp);
                await emailService.SendOtpAsync(email, otp);
                return response;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return BadRequest($"{ex.Message}");
            }

        }

        [HttpPost("verify-forgot-password-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifiOtp dto)
        {
            var email = dto.Email;
            var user = await userRepository.GetByEmail(email);
            var response = Ok(new { ok = true });

            if (user == null)
            {
                return BadRequest("User not exist");
            }
            var key = GetKeyOtp(email);
            var valid = await VerifyForgotPasswordOtp(key, dto.Otp);
            return Ok(new { Ok = valid });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
        {
            var email = dto.Email;
            var user = await userRepository.GetByEmail(email);
            var response = Ok(new { ok = true });

            if (user == null)
            {
                return response;
            }

            var key = GetKeyOtp(email);
            var validOtp = await VerifyForgotPasswordOtp(key, dto.Otp);

            if(!validOtp)
            {
                return BadRequest("OTP not valid");
            }

            // Handle new password
            var hashPassword = PasswordHelper.Hash(dto.NewPassword);
            user.PasswordHash = hashPassword;
            await authService.RemoveOtp(key);

            var newUser = await userRepository.UpdateAsync(user.Id, user);

            return response;
        }

        private async Task<bool> VerifyForgotPasswordOtp(string key, string targetOtp)
        {
            var hashOtp = authService.GetHashOtp(key);
            if (hashOtp == null)
            {
                return false;
            }
            var valid = await authService.VerifyOtp(hashOtp.ToString(), targetOtp);
            return valid;
        }

        private string GetKeyOtp(string key)
        {
            return $"otp.{key}";
        }
    }
}
