using AutoMapper;
using MahiN.DataBase.Dtos.PlaygroundDtos;
using MahiN.DataBase.Models;

namespace MahiN.Mapper
{
    public class PlaygroundProfile : Profile
    {
        public PlaygroundProfile()
        {
            string baseUrl = "https://mahinproject.runasp.net";

            CreateMap<CreatePlaygroundDto, Playground>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore());

            CreateMap<Playground, PlaygroundResponseDto>()
                .ForMember(dest => dest.PlaygroundType, opt => opt.MapFrom(src => src.PlaygroundType.ToString()))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.PhotoUrl)
                        ? (src.PhotoUrl.StartsWith("http") ? src.PhotoUrl : $"{baseUrl}{(src.PhotoUrl.StartsWith("/") ? "" : "/")}{src.PhotoUrl}")
                        : null));

            CreateMap<Playground, PlaygroundBasicDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.PhotoUrl)
                        ? (src.PhotoUrl.StartsWith("http") ? src.PhotoUrl : $"{baseUrl}{(src.PhotoUrl.StartsWith("/") ? "" : "/")}{src.PhotoUrl}")
                        : null));
        }
    }
}

