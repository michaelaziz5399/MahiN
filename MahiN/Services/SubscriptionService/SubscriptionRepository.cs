using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.SubscriptionDto;
using MahiN.DataBase.Enums;
using MahiN.DataBase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.SubscriptionService
{
    public class SubscriptionRepository(ApplicationDbContext applicationDbContext, UserManager<AppUser> userManager) : ISubscriptionRepository
    {
        // ==================== 1.(Subscription Plans) ====================
        #region Subscription Plans
        public async Task<IEnumerable<SubscriptionPlanResponseDto>> GetAllPlansAsync()
        {
            return await applicationDbContext.SubscriptionPlans.Select(plan => new SubscriptionPlanResponseDto
            {
                Id = plan.Id,
                Name = plan.Name,
                TargetAgeGroup = plan.TargetAgeGroup,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                IsActive = plan.IsActive,
            }).ToListAsync();
        }

        public async Task<SubscriptionPlanResponseDto?> GetPlanByIdAsync(int planId)
        {
            var plan = await applicationDbContext.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return null;

            return new SubscriptionPlanResponseDto
            {
                Id = plan.Id,
                Name = plan.Name,
                TargetAgeGroup = plan.TargetAgeGroup,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                IsActive = plan.IsActive,
            };
        }

        public async Task<SubscriptionPlanResponseDto> CreatePlanAsync(CreateUpdateSubscriptionPlanDto model)
        {
            var plan = new SubscriptionPlan
            {
                Name = model.Name,
                Price = model.Price,
                DurationInDays = model.DurationInDays,
                
                TargetAgeGroup = model.TargetAgeGroup,
                IsActive = true
            };

            await applicationDbContext.SubscriptionPlans.AddAsync(plan);
            await applicationDbContext.SaveChangesAsync();

            return new SubscriptionPlanResponseDto
            {
                Id = plan.Id,
                Name = plan.Name,
                TargetAgeGroup = plan.TargetAgeGroup,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                IsActive = plan.IsActive,
            };
        }

        public async Task<SubscriptionPlanResponseDto?> UpdatePlanAsync(int planId, CreateUpdateSubscriptionPlanDto model)
        {
            var plan = await applicationDbContext.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return null;

            plan.Name = model.Name;
            plan.Price = model.Price;
            plan.DurationInDays = model.DurationInDays;
            plan.TargetAgeGroup = model.TargetAgeGroup;

            await applicationDbContext.SaveChangesAsync();

            return new SubscriptionPlanResponseDto
            {
                Id = plan.Id,
                Name = plan.Name,
                TargetAgeGroup = plan.TargetAgeGroup,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                IsActive = plan.IsActive,
            };
        }

        public async Task<bool> TogglePlanStatusAsync(int planId)
        {
            var plan = await applicationDbContext.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return false;
            plan.IsActive = !plan.IsActive;
            await applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubscriptionPlanResponseDto>> GetPlansByAgeGroupAsync(AgeGroup ageGroup)
        {
            return await applicationDbContext.SubscriptionPlans.Where(p => p.TargetAgeGroup == ageGroup && p.IsActive)
                .Select(plan => new SubscriptionPlanResponseDto
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    TargetAgeGroup = plan.TargetAgeGroup,
                    Price = plan.Price,
                    DurationInDays = plan.DurationInDays,
                    IsActive = plan.IsActive,
                }).ToListAsync();
        }

        #endregion

        // ==================== 2. (User Subscriptions) ====================
        #region User Subscriptions
        public async Task<UserSubscriptionResponseDto?> SubscribeUserAsync(CreateUserSubscriptionDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null || !user.IsActive) throw new InvalidOperationException("هذا المستخم غير موجود.");

            var plan = await applicationDbContext.SubscriptionPlans.FindAsync(model.SubscriptionPlanId);
            if (plan == null || !plan.IsActive) throw new InvalidOperationException("هذا الاشتراك غير موجود.");

            if (plan.TargetAgeGroup != AgeGroup.الجميع && plan.TargetAgeGroup != user.AgeGroup)
            {
                throw new InvalidOperationException("اشتراك هذه الخطة غير مخصص للمرحلة العمرية لهذا المستخدم.");
            }

            var currentActiveSub = await applicationDbContext
                .UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == model.UserId && s.IsActive && s.EndDate > DateTime.UtcNow);
            if (currentActiveSub != null) throw new InvalidOperationException("هذا المستخدم لديه اشتراك ساري بالفعل.");

            var startDate = DateTime.UtcNow;
            var userSubscription = new UserSubscription
            {
                UserId = model.UserId,
                SubscriptionPlanId = model.SubscriptionPlanId,
                StartDate = startDate,
                EndDate = startDate.AddDays(plan.DurationInDays),
                PricePaid=model.PricePaid,
                IsActive = true,
            };

            await applicationDbContext.UserSubscriptions.AddAsync(userSubscription);
            await applicationDbContext.SaveChangesAsync();

            return MapToUserSubscriptionDto(userSubscription);
        }


        public async Task<bool> CancelUserSubscriptionAsync(int userSubscriptionId)
        {
            var subscription = await applicationDbContext.UserSubscriptions.FindAsync(userSubscriptionId);
            if (subscription == null || !subscription.IsActive) return false;

            subscription.IsActive = false;
            await applicationDbContext.SaveChangesAsync();
            return true;
        }


        public async Task<UserSubscriptionResponseDto?> GetActiveSubscriptionByUserIdAsync(string userId)
        {
            var subscription = await applicationDbContext
                .UserSubscriptions
                .Include(u => u.User)
                .Include(s => s.SubscriptionPlan)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow);
            if (subscription == null) return null;

            return MapToUserSubscriptionDto(subscription);
        }


        public async Task<IEnumerable<UserSubscriptionResponseDto>> GetAllUserSubscriptionsAsync(bool? activeOnly = null)
        {
            var query = applicationDbContext.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Include(s => s.User)
                .AsQueryable();

            if (activeOnly.HasValue && activeOnly.Value)
            {
                query = query.Where(s => s.IsActive && s.EndDate > DateTime.UtcNow);
            }

            var subscriptions = await query.OrderByDescending(s => s.StartDate).ToListAsync();
            return subscriptions.Select(MapToUserSubscriptionDto);
        }



        public async Task<IEnumerable<UserSubscriptionResponseDto>> GetUserSubscriptionHistoryAsync(string userId)
        {
            var subscriptions = await applicationDbContext.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            return subscriptions.Select(MapToUserSubscriptionDto);
        }

        #endregion

        private static UserSubscriptionResponseDto MapToUserSubscriptionDto(UserSubscription s)
        {
            bool isCurrentlyActive = s.IsActive && s.EndDate > DateTime.UtcNow;

            return new UserSubscriptionResponseDto
            {
                Id = s.Id,
                UserId = s.UserId,
                UserName = s.User?.FullName ?? s.User?.UserName ?? "",
                UserPhoneNumber = s.User?.PhoneNumber ?? "",
                SubscriptionPlanId = s.SubscriptionPlanId,
                PlanName = s.SubscriptionPlan?.Name ?? "",
                Price = s.SubscriptionPlan?.Price ?? 0,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = isCurrentlyActive,
            };
        }

        public async Task<IdentityResult> DeletePlanAsync(int PlanId)
        {
            var plan = await applicationDbContext.SubscriptionPlans.FindAsync(PlanId);
            if (plan == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "هذا الاشتراك غير موجود" });
            }
            var hasActiveSubscription = await applicationDbContext.UserSubscriptions.AnyAsync(x => x.SubscriptionPlanId == PlanId);
            if (hasActiveSubscription)
            {
                return IdentityResult.Failed(new IdentityError { Description = "يوجد يوزر يستخدمون هذا الاشتراك" });
            }
            applicationDbContext.SubscriptionPlans.Remove(plan);
            await applicationDbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }
    }

}