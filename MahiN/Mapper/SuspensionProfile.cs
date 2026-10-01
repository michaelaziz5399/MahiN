using AutoMapper;
using MahiN.DataBase.Dtos.SuspensionDtos;
using MahiN.DataBase.Models;

namespace MahiN.Mapper
{
    public class SuspensionProfile : Profile
    {
        public SuspensionProfile()
        {
            CreateMap<CreateSuspensionDto, PlayerSuspension>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Player, opt => opt.Ignore())
                .ForMember(dest => dest.Tournament, opt => opt.Ignore());

            CreateMap<PlayerSuspension, PlayerSuspensionDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player != null ? src.Player.UserName : string.Empty))
                .ForMember(dest => dest.TournamentName, opt => opt.MapFrom(src => src.Tournament != null ? src.Tournament.Name : null))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.Restriction, opt => opt.MapFrom(src => src.Restriction.ToString()))
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src.Scope.ToString()));
        }
    }
}
