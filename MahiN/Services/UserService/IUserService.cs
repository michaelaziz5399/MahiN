using MahiN.DataBase.Dtos.UserDto;

namespace MahiN.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseAllDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(string id);
        Task<UserResponseDto?> UpdateUserAsync(string id ,UpdateUserDto model);
        Task<bool> ToggleUserActiveStatusAsync(string id);
        Task<bool> DeleteUserByIdAsync(string id);

    }
}
