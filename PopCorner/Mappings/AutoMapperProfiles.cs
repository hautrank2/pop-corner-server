using AutoMapper;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;

namespace PopCorner.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Movie, CreateMovieDto>().ReverseMap();
        }
    }
}
