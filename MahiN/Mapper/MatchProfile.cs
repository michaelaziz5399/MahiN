using AutoMapper;
using MahiN.DataBase.Dtos.MatchDtos;
using MahiN.DataBase.Models;

namespace MahiN.Mapper
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            // 1. CreateTournamentMatchDto -> TournamentMatch (إنشاء مباراة جديدة)
            CreateMap<CreateTournamentMatchDto, TournamentMatch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.HomeScore, opt => opt.Ignore())
                .ForMember(dest => dest.AwayScore, opt => opt.Ignore())
                .ForMember(dest => dest.IsExtraTime, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsPenaltyShootout, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.HomePenaltyScore, opt => opt.Ignore())
                .ForMember(dest => dest.AwayPenaltyScore, opt => opt.Ignore())
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.WinnerTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.Tournament, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeam, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeam, opt => opt.Ignore())
                .ForMember(dest => dest.WinnerTeam, opt => opt.Ignore())
                .ForMember(dest => dest.NextMatch, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            // 2. UpdateMatchResultDto -> TournamentMatch (تحديث نتيجة المباراة)
            CreateMap<UpdateMatchResultDto, TournamentMatch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TournamentId, opt => opt.Ignore())
                .ForMember(dest => dest.Tournament, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeam, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeam, opt => opt.Ignore())
                .ForMember(dest => dest.HomePlaceholder, opt => opt.Ignore())
                .ForMember(dest => dest.AwayPlaceholder, opt => opt.Ignore())
                .ForMember(dest => dest.GroupName, opt => opt.Ignore())
                .ForMember(dest => dest.Stage, opt => opt.Ignore())
                .ForMember(dest => dest.MatchDate, opt => opt.Ignore())
                .ForMember(dest => dest.WinnerTeam, opt => opt.Ignore())
                .ForMember(dest => dest.NextMatchId, opt => opt.Ignore())
                .ForMember(dest => dest.NextMatch, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            // 3. CreateMatchEventDto -> MatchEvent (إنشاء حدث جديد)
            CreateMap<CreateMatchEventDto, MatchEvent>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Match, opt => opt.Ignore())
                .ForMember(dest => dest.Player, opt => opt.Ignore())
                .ForMember(dest => dest.Team, opt => opt.Ignore());

            // 4. MatchEvent -> MatchEventDto (عرض حدث)
            CreateMap<MatchEvent, MatchEventDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player != null ? src.Player.FullName : string.Empty))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : string.Empty))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType.ToString()));

            // 5. TournamentMatch -> TournamentMatchDto (عرض تفاصيل وقائمة المباريات)
            CreateMap<TournamentMatch, TournamentMatchDto>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeTeam != null ? src.HomeTeam.Name : src.HomePlaceholder))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayTeam != null ? src.AwayTeam.Name : src.AwayPlaceholder))
                .ForMember(dest => dest.WinnerTeamName, opt => opt.MapFrom(src => src.WinnerTeam != null ? src.WinnerTeam.Name : null))
                .ForMember(dest => dest.StageName, opt => opt.MapFrom(src => src.Stage.ToString()));
        }
    }
}