using MahiN.DataBase.Dtos.UserDto;
using MahiN.DataBase.Models;
using MahiN.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = " هذا المستخدم غير موجود " });
            }
            return Ok(user);
        }

        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromForm] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Id = id;

                var updatedUser = await userService.UpdateUserAsync(id, model);
                return Ok(updatedUser);
            }
            catch (InvalidOperationException ex)
            {
                //  أخطاء الـ Business Logic (زي المستخدم غير موجود) بترجع 400 BadRequest بوضوح
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                //  أخطاء قاعدة البيانات المباشرة
                var realError = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { message = "خطأ في قاعدة البيانات", details = realError });
            }
            catch (Exception ex)
            {
                //  أي خطأ غير متوقع على السيرفر يرجع 500
                var realError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "حدث خطأ غير متوقع", details = realError });
            }
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActiveStatus(string id)
        {
            try
            {
                var newStatus = await userService.ToggleUserActiveStatusAsync(id);
                return Ok(new { message = "تم تغيير حالة تفعيل المستخدم بنجاح.", isActive = newStatus });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-user/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            try
            {
                var isDeleted = await userService.DeleteUserByIdAsync(id);

                if (isDeleted)
                {
                    return Ok(new { message = "تم حذف المستخدم وإزالة الصلاحيات بنجاح" });
                }

                return BadRequest(new { message = "فشل في حذف المستخدم" });
            }
            catch (InvalidOperationException ex)
            {
                // هيستقبل الرسائل اللي رميناها في السيرفس (زي: عنده اشتراك / غير موجود / الخ)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // لأي خطأ غير متوقع
                return StatusCode(500, new { message = "حدث خطأ غير متوقع في السيرفر", details = ex.Message });
            }
        }

    }
}
