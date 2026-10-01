namespace MahiN.DataBase.Dtos.PlaygroundDtos
{
    public class PlaygroundBasicDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public int Capacity { get; set; }
    }
}
