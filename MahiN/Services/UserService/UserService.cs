 using AutoMapper;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.SubscriptionDto;
using MahiN.DataBase.Dtos.UserDto;
using MahiN.DataBase.Models;
using MahiN.Services.FileService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.UserService
{
    public class UserService(UserManager<AppUser> userManager
        , IMapper mapper 
        , ApplicationDbContext applicationDbContext 
        , IFileService fileService) : IUserService
    {
        public async Task<bool> DeleteUserByIdAsync(string id)
        {
            var user= await userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException(" المستخدم غير موجود بالفعل ");
            }

            var hasSubscriptions = await applicationDbContext.UserSubscriptions.AnyAsync(u => u.UserId == id);
            if(hasSubscriptions)
            {
                throw new InvalidOperationException(" لا يمكنك حذف هذا المستخدم بسبب وجود اشتراك له ");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                var removeRolesResult = await userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeRolesResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", removeRolesResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"فشل في إزالة الـ Roles الخاصة بالمستخدم: {roleErrors}");
                }
            }
            var deleteResult = await userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                var deleteErrors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"حدث خطأ أثناء حذف المستخدم: {deleteErrors}");
            }
            return true;
        }

        public async Task<IEnumerable<UserResponseAllDto>> GetAllUsersAsync()
        {
            var users = await userManager.Users.AsNoTracking().ToListAsync();

            return mapper.Map<IEnumerable<UserResponseAllDto>>(users);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user == null)
            {
                throw new InvalidOperationException("المستخدم غير موجود.");
            }

            var dto = mapper.Map<UserResponseDto>(user);
            var roles = await userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? string.Empty;

            var activeSub = await applicationDbContext
                .UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.IsActive && s.EndDate >= DateTime.UtcNow);

            if (activeSub != null)
            {
                dto.ActiveSubscription = mapper.Map<SubscriptionResposDto>(activeSub);
            }

            return dto;
        }

        public async Task<UserResponseDto?> UpdateUserAsync(string id, UpdateUserDto model)
        {
            if (id != model.Id)
            {
                throw new InvalidOperationException("برجاء إضافة بيانات صحيحة.");
            }

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
                throw new InvalidOperationException("المستخدم غير موجود.");

            // 1. نقل البيانات النصية والأساسية أولاً عبر الـ Mapper
            mapper.Map(model, user);

            // 2. معالجة الصورة (في حالة تم رفع ملف جديد فعلياً في PhotoUrl)
            if (model.PhotoUrl != null && model.PhotoUrl.Length > 0)
            {
                // مسح الصورة القديمة من السيرفر لو كانت مسار ملف وليست Base64
                if (!string.IsNullOrEmpty(user.PhotoUrl) && !user.PhotoUrl.StartsWith("data:image"))
                {
                    fileService.DeleteFile(user.PhotoUrl, "users");
                }

                // حفظ الصورة الجديدة وأخذ المسار النسبي
                string relativePath = await fileService.SaveFileAsync(model.PhotoUrl, "users");
                user.PhotoUrl = relativePath;
            }

            // 3. حفظ التغيرات في قاعدة البيانات
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"فشلت عملية التعديل: {errors}");
            }

            return mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> ToggleUserActiveStatusAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user == null)
                throw new InvalidOperationException("المستخدم غير موجود.");

            user.IsActive = !user.IsActive;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("تعذر تغيير حالة تفعيل المستخدم.");
            }

            return user.IsActive;
        }

    }
}
