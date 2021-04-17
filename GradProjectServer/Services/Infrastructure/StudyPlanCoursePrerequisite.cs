using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private static StudyPlanCoursePrerequisite[]? _seed = null;
        public static StudyPlanCoursePrerequisite[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }

                var seed = new List<StudyPlanCoursePrerequisite>();
                var rand = new Random();

                foreach (var courseGroup in StudyPlanCourse.Seed.GroupBy(c => c.Category.StudyPlanId).Select(g => g.ToArray()))
                {
                    var graph = new List<StudyPlanCourse>();
                    foreach (var course in courseGroup)
                    {
                        var graphClone = new List<StudyPlanCourse>(graph);
                        var prerequisitesCount = rand.Next(graphClone.Count);
                        for (int i = 0; i < prerequisitesCount; i++)
                        {
                            var preq = rand.NextElement(graphClone);
                            graphClone.Remove(preq);
                            seed.Add(new StudyPlanCoursePrerequisite
                            {
                                CourseId = course.Id,
                                PrerequisiteId = preq.Id,
                            });
                        }
                        graph.Add(course);
                    }
                }
                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
