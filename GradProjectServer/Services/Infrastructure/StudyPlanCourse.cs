using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlanCourse
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public Course Course { get; set; }
        public StudyPlanCourseCategory Category { get; set; }
        public ICollection<StudyPlanCoursePrerequisite> Prerequisites { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<StudyPlanCourse> b)
        {
            b.HasKey(s => s.Id);
            b.HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .IsRequired();
            b.HasOne(c => c.Category)
                .WithMany(ca => ca.Courses)
                .HasForeignKey(c => c.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(c => new {c.CourseId, c.CategoryId})
                .IsUnique();
        }

        private static StudyPlanCourse[]? _seed = null;

        public static StudyPlanCourse[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                var seed = new List<StudyPlanCourse>();
                var rand = new Random();
                foreach (var catGroup in StudyPlanCourseCategory.Seed.GroupBy(c => c.StudyPlanId)
                    .Select(g => g.ToArray()))
                {
                    foreach (var course in Course.Seed)
                    {
                        var cat = rand.NextElement(catGroup);
                        seed.Add(new StudyPlanCourse
                        {
                            CourseId = course.Id,
                            CategoryId = cat.Id,
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