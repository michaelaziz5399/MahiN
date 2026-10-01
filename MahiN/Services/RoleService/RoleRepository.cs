using MahiN.DataBase.Dtos.RoleDto;
using MahiN.DataBase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.RoleService
{
    public class RoleRepository(UserManager<AppUser> userManager 
        , RoleManager<IdentityRole> roleManager) : IRoleRepository
    {
        // ============= { Methods for managing roles } =============
        public async Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync()
        {
            return await roleManager.Roles.Select(
                r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name!,
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null) return null;

            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name!
            };
        }

        public async Task<IdentityResult> CreateNewRoleAsync(CreateRoleDto model)
        {
            if(await roleManager.RoleExistsAsync(model.Name))
            {
                return IdentityResult.Failed(new IdentityError { Description = "هذا الدور موجود بالفعل" });
            }

            var newRole = new IdentityRole
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper()
            };

            return await roleManager.CreateAsync(newRole);
        }

        public async Task<IdentityResult> UpdateRoleAsync(string roleId, CreateRoleDto model)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "الدور المراد تعديله غير موجود!"
                });
            }

            var existingRole = await roleManager.FindByNameAsync(model.Name);
            if (existingRole != null && existingRole.Id != roleId)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "اسم الدور الجديد مستخدم بالفعل لدور آخر!"
                });
            }

            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();

            return await roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = " هذا الدور غير موجود بالفعل " });
            }

            const string defaultRole = "مستخدم";
            if (role.Name == defaultRole)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "لا يمكن حذف الدور الافتراضي (مستخدم)!"
                });
            }

            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);

            if (usersInRole.Any())
            {
                if (!await roleManager.RoleExistsAsync(defaultRole))
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = $"تعذر التحويل! الدور الافتراضي '{defaultRole}' غير موجود بجدول الأدوار."
                    });
                }

                foreach (var user in usersInRole)
                {
                    await userManager.RemoveFromRoleAsync(user, role.Name!);

                    if (!await userManager.IsInRoleAsync(user, defaultRole))
                    {
                        await userManager.AddToRoleAsync(user, defaultRole);
                    }
                }
            }

            return await roleManager.DeleteAsync(role);
        }


        // ============== { Methods for managing user roles } ==============

        public async Task<IdentityResult> AssignRoleToUserAsync(UserRoleDto model)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
            if(user == null || !user.IsActive)
            {
                return IdentityResult.Failed(new IdentityError { Description = " هذا المستخدم غير موجود " });
            }

            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = " هذا الدور غير موجود " });
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                return IdentityResult.Failed(new IdentityError { Description = "هذا المستخدم لديه دور بالفعل." });
            }

            return await userManager.AddToRoleAsync(user, role.Name!);
        }

        public async Task<IdentityResult> UpdateUserRoleAsync(UserRoleDto model)
        {
            // 1. البحث عن المستخدم
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
            if (user == null || !user.IsActive)
            {
                return IdentityResult.Failed(new IdentityError { Description = "هذا المستخدم غير موجود أو غير مفعل." });
            }

            // 2. البحث عن الدور الجديد المطلوب التعيين إليه
            var newRole = await roleManager.FindByIdAsync(model.RoleId);
            if (newRole == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "الدور الجديد غير موجود." });
            }

            // 3. جلب الأدوار الحالية للمستخدم
            var currentRoles = await userManager.GetRolesAsync(user);

            // 4. إزالة جميع الأدوار الحالية
            if (currentRoles.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return removeResult; // إرجاع الأخطاء لو فشلت عملية الحذف
                }
            }

            // 5. إسناد الدور الجديد
            return await userManager.AddToRoleAsync(user, newRole.Name!);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u=> u.Id == userId);
            if (user == null) { return Enumerable.Empty<string>(); }

            return await userManager.GetRolesAsync(user);
        }

    }
}
