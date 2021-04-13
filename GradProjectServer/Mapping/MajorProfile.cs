using AutoMapper;
using GradProjectServer.DTO.Majors;
using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping
{
    public class MajorProfile : Profile
    {
        public MajorProfile()
        {
            CreateMap<Major, MajorMetadataDto>();
            CreateMap<Major, MajorDto>();
        }
    }
}
