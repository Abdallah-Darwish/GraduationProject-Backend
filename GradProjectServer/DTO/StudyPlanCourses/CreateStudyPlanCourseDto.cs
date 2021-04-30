namespace GradProjectServer.DTO.StudyPlanCourses
{
    public class CreateStudyPlanCourseDto
    {
        public int StudyPlanCourseCategoryId { get; set; }
        public int CourseId { get; set; }
        /// <summary>
        /// Ids of StudyPlanCourse prerequisites.
        /// </summary>
        public int[] Prerequisites { get; set; }
    }
}