using static System.Runtime.InteropServices.JavaScript.JSType;
using TestCase.Models;
using TestCase.ViewModels;
using AutoMapper;

namespace TestCase.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCouponRequest, CouponDb>()
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CouponStatus.Active));

            CreateMap<CouponDb, CouponResponse>();
        }
    }
}
