using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlan
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int MajorId { get; set; }
        public Major Major { get; set; }
        public ICollection<StudyPlanCourseCategory> Categories { get; set; }
        public ICollection<StudyPlanCourse> Courses { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<StudyPlan> b)
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Year)
                 .IsRequired();
            b.HasOne(p => p.Major)
                 .WithMany(m => m.StudyPlans)
                 .HasForeignKey(p => p.MajorId)
                 .IsRequired();
            b.HasCheckConstraint("CK_STUDYPLAN_YEAR", $@"{nameof(StudyPlan.Year)} > 0");
        }
    }
}
