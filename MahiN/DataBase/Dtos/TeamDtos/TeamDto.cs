namespace MahiN.DataBase.Dtos.TeamDtos
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TournamentId { get; set; }
        public bool IsPaid { get; set; }
        public int MembersCount { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
    }
}
