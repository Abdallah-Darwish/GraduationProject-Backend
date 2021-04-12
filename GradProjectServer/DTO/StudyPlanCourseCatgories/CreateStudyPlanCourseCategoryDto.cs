using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.StudyPlanCourseCategories
{
    public class CreateStudyPlanCourseCategoryDto
    {
        public int MajorId { get; set; }
        public int CatgoryId { get; set; }
        public int AllowedCreditHoures { get; set; }
        public int[] Courses { get; set; }
    }
}
