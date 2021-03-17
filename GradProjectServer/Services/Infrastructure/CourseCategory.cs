using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class CourseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AllowedCreditHours { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
        public ICollection<StudyPlanCourse> Courses { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<CourseCategory> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name)
                 .IsUnicode()
                 .IsRequired();
            b.Property(c => c.AllowedCreditHours)
                 .IsRequired();
            b.HasOne(c => c.StudyPlan)
                 .WithMany(p => p.Categories)
                 .HasForeignKey(c => c.StudyPlanId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);
            b.HasCheckConstraint("CK_COURSECATEGORY_ALLOWEDCREDITHOURS", $@"{nameof(CourseCategory.AllowedCreditHours)} > 0");
        }
    }
}
