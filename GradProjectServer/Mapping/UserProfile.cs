using AutoMapper;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.UserSystem;

namespace GradProjectServer.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserMetadataDto>();
            CreateMap<User, UserDto>();
        }
    }
}
