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
            CreateMap<Movie, EditMovieDto>().ReverseMap();
            CreateMap<Artist, MovieArtistDto>();
            CreateMap<Genre, MovieGenreDto>();
            CreateMap<Movie, MovieDto>()
              .ForMember(dest => dest.Genres,
                  opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre)));

            CreateMap<Artist, CreateArtistDto>().ReverseMap();
            CreateMap<Artist, EditArtistDto>().ReverseMap();
        }
    }
}
