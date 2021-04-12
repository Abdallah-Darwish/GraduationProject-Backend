using AutoMapper;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.UserSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserMetadataDto>();
        }
    }
}
