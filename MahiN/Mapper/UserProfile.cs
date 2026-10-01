using AutoMapper;
using MahiN.DataBase.Dtos.AuthDto;
using MahiN.DataBase.Dtos.SubscriptionDto;
using MahiN.DataBase.Dtos.UserDto;
using MahiN.DataBase.Models;

namespace MahiN.Mapper
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            // 🎯 طريقة الـ Lambda Expression مع قراءة الـ Request بأمان
            CreateMap<AppUser, UserResponseDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (string.IsNullOrEmpty(src.PhotoUrl)) return null;
                    if (src.PhotoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return src.PhotoUrl;

                    // قراءة الـ HttpContext الممرر
                    var cleanPath = src.PhotoUrl.StartsWith("/") ? src.PhotoUrl : $"/{src.PhotoUrl}";
                    return $"https://mahinproject.runasp.net{cleanPath}";
                }));

            CreateMap<AppUser, UserResponseAllDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (string.IsNullOrEmpty(src.PhotoUrl)) return null;
                    if (src.PhotoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return src.PhotoUrl;

                    var cleanPath = src.PhotoUrl.StartsWith("/") ? src.PhotoUrl : $"/{src.PhotoUrl}";
                    return $"https://mahinproject.runasp.net{cleanPath}";
                }));

            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // الـ UserName هو الإيميل
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore()) // هنعمل Ignore للصورة عشان هنحسب الرابط المرفوع يدوي
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))   // قيمة افتراضية
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap< UserSubscription , SubscriptionResposDto>()
                .ForMember(dest => dest.PlanName , opt=> opt.MapFrom(src => src.SubscriptionPlan!.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.SubscriptionPlan!.Price))
                .ReverseMap();

            // من UpdateUserDto إلى AppUser
            CreateMap<UpdateUserDto, AppUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore()) // 🎯 مهم جداً لعدم مسح مسار الصورة الحالية
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
