using MahiN.DataBase.Dtos.RoleDto;
using Microsoft.AspNetCore.Identity;

namespace MahiN.Services.RoleService
{
    public interface IRoleRepository
    {
        // ============= { Methods for managing roles } =============
        Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync();
        Task<RoleResponseDto?> GetRoleByIdAsync(string roleId);
        Task<IdentityResult> CreateNewRoleAsync(CreateRoleDto model);
        Task<IdentityResult> UpdateRoleAsync(string roleId,CreateRoleDto model);
        Task<IdentityResult> DeleteRoleAsync(string roleId);

        // ============= { Methods for managing user roles } =============

        Task<IdentityResult> AssignRoleToUserAsync(UserRoleDto model);
        Task<IdentityResult> UpdateUserRoleAsync(UserRoleDto model);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);

    }
}
