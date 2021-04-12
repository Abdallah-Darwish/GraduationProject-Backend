using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlanCourseCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int AllowedCreditHours { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
        public CourseCategory Category { get; set; }
        public ICollection<StudyPlanCourse> Courses { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<StudyPlanCourseCategory> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.AllowedCreditHours)
                 .IsRequired();
            b.HasOne(c => c.StudyPlan)
                 .WithMany(p => p.Categories)
                 .HasForeignKey(c => c.StudyPlanId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(c => c.Category)
                 .WithMany()
                 .HasForeignKey(c => c.StudyPlanId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);
            b.HasCheckConstraint("CK_STUDYPLANCOURSECATEGORY_ALLOWEDCREDITHOURS", $@"{nameof(AllowedCreditHours)} > 0");
        }
    }
}
