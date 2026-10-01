namespace MahiN.DataBase.Dtos.TeamDtos
{
    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public bool IsCaptain { get; set; }
    }
}
