namespace MahiN.DataBase.Dtos.PlaygroundDtos
{
    public class PlaygroundResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // مسار الصورة الكامل بعد تركيب الـ Base URL بواسطة AutoMapper
        public string? PhotoUrl { get; set; }

        public string PlaygroundType { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
    }
}
