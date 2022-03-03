using AutoMapper;
using DAL.Entities.Identity;
using Models.Admin.Inputs;
using Models.Admin.Outputs;

namespace Services.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<User, LoginAdminOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
            CreateMap<UpdateAdmin, User>();
        }
    }
}