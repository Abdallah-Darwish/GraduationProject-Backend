using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlanCourse
    {
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public Course Course { get; set; }
        public CourseCategory Category { get; set; }
        public ICollection<StudyPlanCoursePrerequisite> Prerequisites { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<StudyPlanCourse> b)
        {
            b.HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .IsRequired();
            b.HasOne(c => c.Category)
               .WithMany(ca => ca.Courses)
               .HasForeignKey(c => c.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
