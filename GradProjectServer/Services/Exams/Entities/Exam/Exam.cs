using GradProjectServer.Common;
using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Services.Exams.Entities
{
  
    public class Exam
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public bool IsApproved { get; set; }
        public ExamType Type { get; set; }
        public Semester Semester { get; set; }
        public string Name { get; set; }
        public ICollection<ExamQuestion> Questions { get; set; }
        public TimeSpan Duration { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }
        public IEnumerable<Tag> Tags => Questions.SelectMany(e => e.Question.SubQuestions.SelectMany(q => q.Tags.Select(t => t.Tag))).Distinct();
        public static void ConfigureEntity(EntityTypeBuilder<Exam> b)
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Name)
                .IsRequired()
                .IsUnicode();
            b.Property(e => e.Year)
                .IsRequired();
            b.Property(e => e.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);
            b.Property(e => e.Semester)
                .IsRequired()
                .HasConversion<byte>();
            b.Property(e => e.Type)
                .IsRequired()
                .HasConversion<byte>();

            b.Property(e => e.Duration)
                .IsRequired()
                .HasConversion(new TimeSpanToTicksConverter());
            b.HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Volunteer)
                .WithMany(u => u.VolunteeredExams)
                .HasForeignKey(e => e.VolunteerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasCheckConstraint("CK_EXAM_YEAR", $@"{nameof(Year)} >= 1");
            b.HasCheckConstraint("CK_EXAM_DURATION", $@"{nameof(Duration)} > 0");

            b.HasData(Seed);
        }
        private static Exam[]? _seed = null;
        public static Exam[] Seed
        {
            get
            {
                if(_seed != null) { return _seed; }

                Random rand = new();
                List<Exam> seed = new();
                var coursesWithQuestions = SubQuestion.Seed
                    .Where(sq => sq.Question.IsApproved)
                    .GroupBy(q => q.Question.Course)
                    .Select(g => (Course : g.Key, QuestionCount: g.Count()));
                var examsTypes = Enum.GetValues<ExamType>();
                var semesters = Enum.GetValues<Semester>();
                var users = User.Seed;
                
                foreach(var (course, questionCount) in coursesWithQuestions)
                {
                    var examCount = Math.Min(Math.Max(1, rand.Next(questionCount / 2, questionCount + 1)), 50);
                    for (int i = 0; i < examCount; i++)
                    {
                        var volunteer = rand.NextElement(users);
                        var exam = new Exam
                        {
                            CourseId = course.Id,
                            Course = course,
                            IsApproved = rand.NextBool(),
                            Name = $"Course {course.Id}, exam {i}. {rand.NextText(rand.Next(10))}",
                            Type = rand.NextElement(examsTypes),
                            VolunteerId = volunteer.Id,
                            Volunteer = volunteer,
                            Duration = TimeSpan.FromMinutes(rand.Next(10, 120)),
                            Semester = rand.NextElement(semesters),
                            Year = rand.Next(2015, 2021),
                        };
                        seed.Add(exam);
                    }
                }
                for(int i = 1;i<=seed.Count;i++)
                {
                    seed[i - 1].Id = i;
                }
                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
