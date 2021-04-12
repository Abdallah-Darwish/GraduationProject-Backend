using GradProjectServer.DTO.StudyPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Majors
{
    public class MajorDto : MajorMetadataDto
    {
        public StudyPlanMetadataDto[] StudyPlans { get; set; }
    }
}
