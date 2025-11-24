using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PopCorner.Data;
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
            try
            {
                var avt = dto.Avatar;
                var avtRes = await cloudinarySrv.UploadImage(new FileImage
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
                var addRes = userRepository.CreateAsync(user);

                return Ok(addRes);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
