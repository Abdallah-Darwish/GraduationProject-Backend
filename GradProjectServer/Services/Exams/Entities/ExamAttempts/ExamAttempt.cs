using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities.ExamAttempts
{
    public class ExamAttempt
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public Exam Exam { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ExamAttempt> b)
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.StartTime)
              .IsRequired();
            b.HasOne(a => a.Exam)
                .WithMany()
                .HasForeignKey(a => a.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
