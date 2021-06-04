using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlan
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int MajorId { get; set; }
        public Major Major { get; set; }
        public ICollection<StudyPlanCourseCategory> CourseCategories { get; set; }
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
            b.HasIndex(p => new {p.MajorId, p.Year})
                .IsUnique();
            b.HasCheckConstraint("CK_STUDYPLAN_YEAR", $"\"{nameof(Year)}\" > 0");
        }

        private static StudyPlan[]? _seed = null;

        public static StudyPlan[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                var rand = new Random();
                var seed = new List<StudyPlan>();
                foreach (var major in Major.Seed)
                {
                    var y = rand.Next(2015, 2019);
                    seed.Add(new StudyPlan
                    {
                        MajorId = major.Id,
                        Year = y,
                    });
                    seed.Add(new StudyPlan
                    {
                        MajorId = major.Id,
                        Year = y + 2,
                    });
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