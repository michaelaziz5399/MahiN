using MahiN.DataBase.Dtos.MatchDtos;
using MahiN.DataBase.Dtos.TeamDtos;

namespace MahiN.DataBase.Dtos.TournamentDtos
{
    public class TournamentDetailsDto : TournamentDto
    {
        public int MinPlayersPerTeam { get; set; }
        public int MaxPlayersPerTeam { get; set; }
        public List<TeamDto> Teams { get; set; } = new();
        public List<TournamentMatchDto> Matches { get; set; } = new();
    }
}
