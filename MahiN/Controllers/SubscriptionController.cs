using MahiN.DataBase.Dtos.SubscriptionDto;
using MahiN.DataBase.Enums;
using MahiN.Services.SubscriptionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MahiN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SubscriptionController(ISubscriptionRepository subscriptionRepository) : ControllerBase
    {
        [HttpGet("get-all-plans")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanResponseDto>>> GetAllPlans()
        {
            var plans = await subscriptionRepository.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpGet("get-plan-id/{id:int}")]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> GetPlanById(int id)
        {
            var plan = await subscriptionRepository.GetPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound(new { message = "خطة الاشتراك غير موجودة!" });
            }

            return Ok(plan);
        }

        [HttpGet("plans/by-age-group/{ageGroup}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanResponseDto>>> GetPlansByAgeGroup(AgeGroup ageGroup)
        {
            var plans = await subscriptionRepository.GetPlansByAgeGroupAsync(ageGroup);
            return Ok(plans);
        }

        [HttpPost("craete-plan")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateUpdateSubscriptionPlanDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdPlan = await subscriptionRepository.CreatePlanAsync(model);
            return CreatedAtAction(nameof(GetPlanById), new { id = createdPlan.Id }, createdPlan);
        }

        [HttpPut("update-plans/{id:int}")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] CreateUpdateSubscriptionPlanDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedPlan = await subscriptionRepository.UpdatePlanAsync(id, model);
            if (updatedPlan == null)
            {
                return NotFound(new { message = "خطة الاشتراك غير موجودة لتعديلها!" });
            }

            return Ok(updatedPlan);
        }

        [HttpPatch("plans/{id:int}/toggle-status")]
        public async Task<IActionResult> TogglePlanStatus(int id)
        {
            var result = await subscriptionRepository.TogglePlanStatusAsync(id);
            if (!result)
            {
                return NotFound(new { message = "خطة الاشتراك غير موجودة!" });
            }

            return Ok(new { message = "تم تغيير حالة تفعيل الخطة بنجاح!" });
        }


        [HttpDelete("delete-plan/{id:int}")]
        public async Task<IActionResult> DeleteSubscriptionPlan(int id)
        {
            var result = await subscriptionRepository.DeletePlanAsync(id);

            if (!result.Succeeded)
            {
                var isNotFound = result.Errors.Any(e => e.Code == "NotFound");
                if (isNotFound)
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(new { message = "تم حذف خطة الاشتراك بنجاح." });
        }



        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeUser([FromBody] CreateUserSubscriptionDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // تأمين الطلب: التأكد أن المستخدم يشترك لنفسه، إلا إذا كان الموجه هو الأدمن
            /** var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (currentUserId != model.UserId && !User.IsInRole("أدمن"))
             {
                 return Forbid();
             }**/

            try
            {
                var subscription = await subscriptionRepository.SubscribeUserAsync(model);
                if (subscription == null)
                {
                    return BadRequest(new { message = "تعذرت عملية الاشتراك! تأكد من وجود المستخدم أو الخطة المطلوبين." });
                }

                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "حدث خطأ غير متوقع في السيرفر أثناء عملية الاشتراك." });
            }
        }

        [HttpPost("cancel-Subscription/{subscriptionId:int}")]
        public async Task<IActionResult> CancelSubscription(int subscriptionId)
        {
            var result = await subscriptionRepository.CancelUserSubscriptionAsync(subscriptionId);
            if (!result)
            {
                return BadRequest(new { message = "تعذر إلغاء الاشتراك! قد يكون غير موجود أو ملغى بالفعل." });
            }

            return Ok(new { message = "تم إلغاء الاشتراك بنجاح!" });
        }

        [HttpGet("user-active-subscription/{userId}")]
        public async Task<IActionResult> GetUserActiveSubscriptionByAdmin(string userId)
        {
            var subscription = await subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
            if (subscription == null)
            {
                return NotFound(new { message = "لا يوجد اشتراك فعال لهذا المستخدم." });
            }

            return Ok(subscription);
        }

        [HttpGet("my-subscription-history/{userId}")]
        public async Task<IActionResult> GetMySubscriptionHistory(string userId)
        {
            var history = await subscriptionRepository.GetUserSubscriptionHistoryAsync(userId);
            return Ok(history);
        }

        [HttpGet("all-user-subscriptions")]
        public async Task<IActionResult> GetAllUserSubscriptions([FromQuery] bool? activeOnly)
        {
            var subscriptions = await subscriptionRepository.GetAllUserSubscriptionsAsync(activeOnly);
            return Ok(subscriptions);
        }
    }
}
