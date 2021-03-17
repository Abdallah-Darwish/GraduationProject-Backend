using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestionAnswer
    {
        public int AttemptId { get; set; }
        public int SubQuestionId { get; set; }
        public string? Answer { get; set; }
        public ExamAttempt Attempt { get; set; }
        public ExamSubQuestion SubQuestion { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<SubQuestionAnswer> b)
        {
            b.HasKey(a => new { a.AttemptId, a.SubQuestionId });
            b.Property(a => a.Answer)
                .IsUnicode();
            b.HasOne(a => a.SubQuestion)
                .WithMany()
                .HasForeignKey(a => a.SubQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(a => a.Attempt)
                .WithMany()
                .HasForeignKey(a => a.AttemptId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
