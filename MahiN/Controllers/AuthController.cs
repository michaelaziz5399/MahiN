using AutoMapper;
using MahiN.DataBase.Dtos.AuthDto;
using MahiN.DataBase.Models;
using MahiN.Services.FileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MahiN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration
        , UserManager<AppUser> userManager 
        , IFileService fileService 
        ,IMapper mapper) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromForm] RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 1. التأكد من البريد ورقم الهاتف
                var userExists = await userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                    return BadRequest(new { message = "هذا البريد الإلكتروني مسجل بالفعل!" });

                var phoneExists = await userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (phoneExists)
                    return BadRequest(new { message = "رقم الهاتف مسجل بالفعل لمستخدم آخر!" });

                // 2. استخدام AutoMapper
                var user = mapper.Map<AppUser>(model);

                // 3. رفع الصورة بأمان (فقط لو تم إرفاق ملف صورة حقيقي)
                if (model.PhotoUrl != null && model.PhotoUrl.Length > 0)
                {
                    // تأمين مسار الـ WebRootPath للاستضافات الخارجية
                    string relativePath = await fileService.SaveFileAsync(model.PhotoUrl, "users");
                    user.PhotoUrl = relativePath;
                }

                // 4. حفظ المستخدم
                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "فشلت عملية إنشاء الحساب", errors });
                }

                // 5. الدور الافتراضي
                await userManager.AddToRoleAsync(user, "user");

                return Ok(new { message = "تم تسجيل الحساب بنجاح!" });
            }
            catch (Exception ex)
            {
                // 🎯 الحركة دي هتمنع الـ 500 وتظهرلك رسالة الخطأ الحقيقية في الـ Alert بدلاً من خطأ السيرفر المبهم!
                return BadRequest(new
                {
                    message = "حدث خطأ أثناء معالجة البيانات على السيرفر",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }



        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto model)
        {
            // 1. التأكد من وجود المستخدم بالـ PhoneNumber
            var user = await userManager.Users.FirstOrDefaultAsync(user => user.PhoneNumber == model.PhoneNumber);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "خطأ في رقم الهاتف أو كلمة المرور!" });
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("FullName", user.FullName ?? string.Empty),
                new Claim("IsActive", user.IsActive.ToString().ToLower()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                // بنضيف الـ Role كـ Claim جوه التوكن
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // 1. قراءة الـ Secret وتوليد مفتاح التشفير
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!));

            // 2. قراءة عدد الأيام وتحويلها لـ double (مع قيمة افتراضية 1 يوم في حال عدم وجودها)
            var tokenExpiryInDays = double.Parse(configuration["JWT:DurationInDays"] ?? "1");

            // 3. إنشاء الـ Token
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddDays(tokenExpiryInDays),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                message = $"أهلاً بك يا {user.FullName}، تم تسجيل الدخول بنجاح!"
            });
        }



        [Authorize] // تضمن أن الدالة لا تُستدعى إلا من مستخدم مسجل الدخول ويحمل Token صالحة
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // 1. استخراج الـ UserId والـ Jti من الـ Claims الخاصة بالتوكن الحالية
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "الرمز غير صالح أو تم تسجيل الخروج بالفعل!" });
            }


            // 3. إرجاع رسالة نجاح للـ Frontend
            return Ok(new
            {
                message = "تم تسجيل الخروج بنجاح!",
                loggedOutAt = DateTime.UtcNow
            });
        }
    }
}
