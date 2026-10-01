using MahiN.DataBase.Dtos.RoleDto;
using MahiN.Services.RoleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MahiN.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleRepository roleRepository) : ControllerBase
    {
        // ========== Role Management Endpoints ==========

        [HttpGet("get-all-role")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetAllRoles()
        {
            var roles = await roleRepository.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("get-role-by-id/{id}")]
        public async Task<ActionResult<RoleResponseDto>> GetRoleById(string id)
        {
            var role = await roleRepository.GetRoleByIdAsync(id);
            if(role == null)
            {
                return NotFound(new {message= "هذا الدور غير موجود "});
            }
            return Ok(role);
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await roleRepository.CreateNewRoleAsync(model);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "فشلت عملية إنشاء الدور", errors });
            }

            return Ok(new { message = $"تم إنشاء دور '{model.Name}' بنجاح!" });
        }

        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] CreateRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await roleRepository.UpdateRoleAsync(id, model);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "فشلت عملية تعديل الدور", errors });
            }

            return Ok(new { message = "تم تعديل اسم الدور بنجاح!" });
        }

        [HttpDelete("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await roleRepository.DeleteRoleAsync(id);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "فشلت عملية حذف الدور", errors });
            }

            return Ok(new { message = "تم حذف الدور ونقل المستخدمين التابعين له الي الدور الافتراضي بنجاح!" });
        }


        // ========== User Role Management Endpoints ==========

        [HttpPost("assign-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await roleRepository.AssignRoleToUserAsync(model);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "فشلت عملية إسناد الدور للمستخدم", errors });
            }

            return Ok(new { message = "تم اسناد الدور لهذا المستخدم بنجاح!" });
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("update-user-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await roleRepository.UpdateUserRoleAsync(model);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "فشلت عملية تحديث دور المستخدم", errors });
                }

                return Ok(new { message = "تم تحديث دور المستخدم بنجاح!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "حدث خطأ أثناء معالجة البيانات على السيرفر",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("user-roles/{userId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
        {
            var roles = await roleRepository.GetUserRolesAsync(userId);
            return Ok(roles);
        }
    }
}
