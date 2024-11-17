using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using TestCase.Models.Database;
using TestCase.Models.ViewModels;

namespace TestCase.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCouponRequest, CouponDto>()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CouponStatus.Active));

            CreateMap<CouponDto, CouponViewModel>();

            CreateMap<UserDto, UserViewModel>();
        }
    }
}
