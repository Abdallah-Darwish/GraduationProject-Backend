using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlanCoursePrerequisite
    {
        /// <summary>
        /// Start of edge
        /// </summary>
        public int PrerequisiteId { get; set; }
        /// <summary>
        /// End of edge
        /// </summary>
        public int CourseId { get; set; }
        public StudyPlanCourse Prerequisite { get; set; }
        public StudyPlanCourse Course { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<StudyPlanCoursePrerequisite> b)
        {
            b.HasKey(p => new { p.PrerequisiteId, p.CourseId });
            b.HasOne(p => p.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(p => p.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(p => p.Prerequisite)
               .WithMany()
               .HasForeignKey(p => p.PrerequisiteId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
