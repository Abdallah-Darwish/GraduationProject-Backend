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
        public ICollection<ExamSubQuestion> SubQuestions { get; set; }
        public TimeSpan Duration { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }
        public IEnumerable<Tag> Tags => SubQuestions.SelectMany(e => e.SubQuestion.Tags.Select(t => t.Tag)).Distinct();
        //tags
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

            b.HasCheckConstraint("CK_EXAM_YEAR", $@"{nameof(Exam.Year)} >= 1");
            b.HasCheckConstraint("CK_EXAM_DURATION", $@"{nameof(Exam.Duration)} > 0");
        }
    }
}
