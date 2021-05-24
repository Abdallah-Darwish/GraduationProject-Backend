using System;
using System.Collections.Generic;
using GradProjectServer.Services.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities.ExamAttempts
{
    public class ExamAttempt
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int OwnerId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public Exam Exam { get; set; }
        public User Owner { get; set; }
        /// <summary>
        /// Over exam attempts are left in the db so the user can download his programming sub questions answers, it will be removed when he creates a new one.
        /// </summary>
        public bool IsFinished { get; set; }

        public bool IsTimeOver => (StartTime - DateTimeOffset.Now) >= Exam.Duration;
        public bool IsOver => IsFinished || IsTimeOver;
        public ICollection<SubQuestionAnswer> Answers { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ExamAttempt> b)
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.StartTime)
                .IsRequired();
            b.Property(e => e.IsFinished)
                .IsRequired();
            b.Ignore(e => e.IsTimeOver);
            b.Ignore(e => e.IsOver);
            b.HasOne(a => a.Exam)
                .WithMany()
                .HasForeignKey(a => a.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Owner)
                .WithOne()
                .HasForeignKey<ExamAttempt>(e => e.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}