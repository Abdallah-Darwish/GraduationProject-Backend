namespace GradProjectServer.DTO.StudyPlanCourses
{
    public class UpdateStudyPlanCourseDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Ids of StudyPlanCourse prerequisites.
        /// </summary>
        public int[]? PrerequisitesToAdd { get; set; }

        /// <summary>
        /// Ids of StudyPlanCourse prerequisites.
        /// </summary>
        public int[]? PrerequisitesToDelete { get; set; }
    }
}