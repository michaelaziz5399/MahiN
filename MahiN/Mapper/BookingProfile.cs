using AutoMapper;
using MahiN.DataBase.Dtos.BookingDto;
using MahiN.DataBase.Models;

namespace MahiN.Mapper
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // From CreateDto to Entity
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.BookingPlaygrounds, opt => opt.Ignore()); // سيتم ربط الملاعب يدويًا في الـ Repository

            CreateMap<CreateBookingDto, CheckAvailabilityDto>();

            // From Entity to ResponseDto (التفاصيل الكاملة)
            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.UserName : string.Empty))
                .ForMember(dest => dest.Playgrounds, opt => opt.MapFrom(src => src.BookingPlaygrounds.Select(bp => bp.Playground)));

            // From Entity to ResponseAllDto (القائمة)
            CreateMap<Booking, BookingResponseAllDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty));

            CreateMap<UpdateBookingStatusDto , Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ApprovedById, opt => opt.MapFrom(src => src.ApprovedById));
        }
    }
}
