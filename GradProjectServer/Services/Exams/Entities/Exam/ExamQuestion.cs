using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ExamQuestion
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public ICollection<ExamSubQuestion> ExamSubQuestions { get; set; }
        public Exam Exam { get; set; }
        public Question Question  { get;set;}
        public static void ConfigureEntity(EntityTypeBuilder<ExamQuestion> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Order)
                .IsRequired();
            b.HasOne(q => q.Exam)
                .WithMany(e => e.Questions)
                .HasForeignKey(q => q.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(q => q.Question)
                .WithMany()
                .HasForeignKey(q => q.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
