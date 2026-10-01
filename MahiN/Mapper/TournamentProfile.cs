using AutoMapper;
using MahiN.DataBase.Dtos.TournamentDtos;
using MahiN.DataBase.Enums;
using MahiN.DataBase.Models;

namespace MahiN.Mapper
{
    public class TournamentProfile : Profile
    {
        public TournamentProfile()
        {
            // 1. CreateTournamentDto -> Tournament (إنشاء بطولة جديدة)
            CreateMap<CreateTournamentDto, Tournament>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TournamentStatus.Upcoming))
                .ForMember(dest => dest.Playground, opt => opt.Ignore())
                .ForMember(dest => dest.Teams, opt => opt.Ignore())
                .ForMember(dest => dest.Matches, opt => opt.Ignore());

            // 2. UpdateTournamentStatusDto -> Tournament (تحديث حالة البطولة)
            CreateMap<UpdateTournamentStatusDto, Tournament>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PlaygroundId, opt => opt.Ignore())
                .ForMember(dest => dest.Playground, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationFee, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.Ignore())
                .ForMember(dest => dest.MinPlayersPerTeam, opt => opt.Ignore())
                .ForMember(dest => dest.MaxPlayersPerTeam, opt => opt.Ignore())
                .ForMember(dest => dest.Teams, opt => opt.Ignore())
                .ForMember(dest => dest.Matches, opt => opt.Ignore());

            // 3. Tournament -> TournamentDto (عرض قائمة البطولات)
            CreateMap<Tournament, TournamentDto>()
                .ForMember(dest => dest.PlaygroundName, opt => opt.MapFrom(src => src.Playground != null ? src.Playground.Name : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TeamsCount, opt => opt.MapFrom(src => src.Teams.Count));

            // 4. Tournament -> TournamentDetailsDto (عرض تفاصيل البطولة الكاملة مع الفرق والمباريات)
            CreateMap<Tournament, TournamentDetailsDto>()
                .IncludeBase<Tournament, TournamentDto>();
        }
    }
}
