using MahiN.DataBase.Dtos.PlaygroundDtos;

namespace MahiN.Services.PlaygroundService
{
    public interface IPlaygroundRepository
    {
        Task<IEnumerable<PlaygroundResponseDto>> GetAllPlaygroundsAsync(bool onlyActive= false);
        Task<PlaygroundResponseDto?> GetPlaygroundByIdAsync(int id);
        Task<PlaygroundResponseDto> CreatePlaygroundAsync(CreatePlaygroundDto model);
        Task<PlaygroundResponseDto?> UpdatePlaygroundAsync(int id, CreatePlaygroundDto model);
        Task<bool> DeletePlaygroundAsync(int id);
        Task<bool> ToggleStatusAsync(int id);
    }
}
