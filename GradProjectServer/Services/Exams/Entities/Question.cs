using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Services.Exams.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<SubQuestion> SubQuestions { get; set; }
        public string Title { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }
        public IEnumerable<Tag> Tags => SubQuestions.SelectMany(s => s.Tags.Select(t => t.Tag)).Distinct();

        public static void ConfigureEntity(EntityTypeBuilder<Question> b)
        {
            b.ToTable(nameof(Question))
                .HasKey(q => q.Id);
            b.Property(q => q.Title)
                .IsUnicode()
                .IsRequired();
            b.Property(q => q.Content)
                .IsUnicode()
                .IsRequired();
            b.Property(q => q.IsApproved)
                .HasDefaultValue(false)
                .IsRequired();
            b.Ignore(q => q.Tags);

            b.HasOne(q => q.Course)
                .WithMany()
                .HasForeignKey(q => q.CourseId)
                .IsRequired();
            b.HasOne(b => b.Volunteer)
                .WithMany(b => b.VolunteeredQuestions)
                .HasForeignKey(b => b.VolunteerId)
                .IsRequired();
        }

        private static Question[]? _seed = null;

        public static Question[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                var rand = new Random();
                var seed = new List<Question>();
                var courses = Course.Seed;
                var users = User.Seed;
                var questionsCount = rand.Next(300, 500);
                for (int i = 1; i <= questionsCount; i++)
                {
                    var user = rand.NextElement(users);
                    var course = rand.NextElement(courses);

                    var question = new Question
                    {
                        Id = i,
                        Content = $"Question {i} content {rand.NextText(rand.Next(1, 300))}",
                        Title = $"Question {i} Title {rand.NextText(rand.Next(1, 20))}",
                        IsApproved = rand.NextBool(),
                        CourseId = course.Id,
                        VolunteerId = user.Id,
                    };
                    seed.Add(question);
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}