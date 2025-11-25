using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service.Interfaces;

namespace PopCorner.Controllers
{
    [Route("/api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly PopCornerDbContext dbContext;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinarySrv;
        public UserController(PopCornerDbContext dbContext, IUserRepository userRepository, IMapper mapper, ICloudinaryService cloudinarySrv)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.cloudinarySrv = cloudinarySrv;
        }

        [HttpGet]
        public async Task<IActionResult> GetSync([FromQuery] UserQueryDto query) 
        {
            var res= await userRepository.GetAllAsync(query);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddSync([FromForm] AddUserDto dto)
        {
            FileImage? avtRes = null;
            try
            {
                var avt = dto.Avatar;
                avtRes = await cloudinarySrv.UploadImage(new FileImage
                {
                    File = avt,
                    FileExtension = Path.GetExtension(avt.FileName),
                    FileSizeInBytes = avt.Length,
                    FileName = Guid.NewGuid().ToString(),
                    FileDescription = "User Avatar",
                    Folder = "/users"
                });
                var user = mapper.Map<User>(dto);
                user.AvatarUrl = avtRes.FilePath;
                user.PasswordHash = PasswordHelper.Hash(dto.Password);
                var addRes = await userRepository.CreateAsync(user);

                return Ok(addRes);
            }
            catch (Exception ex) 
            {
                if (avtRes != null) 
                {
                    await cloudinarySrv.RemoveFileByPathName(avtRes.FilePath);
                }
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> EditSync([FromRoute] Guid id, [FromForm] EditUserDto dto)
        {
            FileImage? newAvt = null;
            var removedAvt = false;
            try
            {
                var user = await userRepository.GetByIdAsync(id);

                if(user == null)
                {
                    return BadRequest("User not found");
                }

                // If New Password
                if(!string.IsNullOrEmpty(dto.Password))
                {
                    user.PasswordHash = PasswordHelper.Hash(dto.Password);
                }

                // If New Avatar
                var oldAvtUrl = user.AvatarUrl;
                if(dto.Avatar != null)
                {
                    newAvt = await cloudinarySrv.UploadImage(new FileImage
                    {
                        File = dto.Avatar,
                        FileDescription = "User Avatar",
                        FileExtension = Path.GetExtension(dto.Avatar.FileName),
                        FileName = Guid.NewGuid().ToString(),
                        Folder = "/users",
                        FileSizeInBytes = dto.Avatar.Length,
                    });
                    user.AvatarUrl = newAvt.FilePath;
                }

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.UpdatedAt = DateTime.UtcNow;

                var res = await userRepository.UpdateAsync(id, user);
                // Remove old avatar
                if (newAvt != null && !string.IsNullOrEmpty(oldAvtUrl))
                {
                    removedAvt = await cloudinarySrv.RemoveFileByPathName(oldAvtUrl);
                }
                return Ok(res);
            }
            catch (Exception ex) 
            {
                if(newAvt != null)
                {
                    await cloudinarySrv.RemoveFileByPathName(newAvt.FilePath);
                }
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteSync([FromRoute] Guid id)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                // Remove Avatar
                var avtUrl = user.AvatarUrl;

                var res = await userRepository.DeleteAsync(id);

                // Remove Avatar
                if(!string.IsNullOrEmpty(avtUrl))
                {
                    await cloudinarySrv.RemoveFileByPathName(avtUrl);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
