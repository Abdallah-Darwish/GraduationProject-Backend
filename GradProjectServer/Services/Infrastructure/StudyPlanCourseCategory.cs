using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

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
                .WithMany(p => p.CourseCategories)
                .HasForeignKey(c => c.StudyPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.StudyPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasCheckConstraint("CK_STUDYPLANCOURSECATEGORY_ALLOWEDCREDITHOURS", $"\"{nameof(AllowedCreditHours)}\" > 0");
            b.HasIndex(c => new { c.StudyPlanId, c.CategoryId })
                .IsUnique();
        }
        private static StudyPlanCourseCategory[]? _seed = null;
        public static StudyPlanCourseCategory[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }
                var rand = new Random();
                var seed = new List<StudyPlanCourseCategory>();
                foreach (var sp in StudyPlan.Seed)
                {
                    foreach (var cat in CourseCategory.Seed)
                    {
                        seed.Add(new StudyPlanCourseCategory
                        {
                            AllowedCreditHours = rand.Next(3, 51),
                            CategoryId = cat.Id,
                            StudyPlanId = sp.Id,
                        });
                    }
                }
                for (int i = 1; i <= seed.Count; i++)
                {
                    seed[i - 1].Id = i;
                }
                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
