using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities.ExamAttempts
{
    public class SubQuestionAnswer
    {
        public int Id { get; set; }
        public int AttemptId { get; set; }
        public int SubQuestionId { get; set; }
        public ExamAttempt Attempt { get; set; }
        public ExamSubQuestion SubQuestion { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<SubQuestionAnswer> b)
        {
            b.ToTable(nameof(SubQuestionAnswer));
            b.HasKey(a => a.Id);
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
