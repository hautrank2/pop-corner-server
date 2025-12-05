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
                  opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre)))
              .ForMember(dest => dest.Credits,
                  opt => opt.MapFrom(src => src.MovieCredits));
            CreateMap<MovieCredit, MovieCreditResponseDto>();
            CreateMap<Artist, MovieCreditArtist>();
            CreateMap<CreditRole, MovieCreditRoleDto>();


            CreateMap<Artist, CreateArtistDto>().ReverseMap();
            CreateMap<Artist, EditArtistDto>().ReverseMap();

            // User
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, AddUserDto>().ReverseMap();

            // Comment
            CreateMap<AddMovieCommentDto, Comment>().ReverseMap();

            // Movie
            CreateMap<Rating, MovieRatingResponseDto>().ReverseMap();
        }
    }
}
