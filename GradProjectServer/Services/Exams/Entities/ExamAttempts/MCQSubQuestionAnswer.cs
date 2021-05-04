using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities.ExamAttempts
{
    public class MCQSubQuestionAnswer : SubQuestionAnswer
    {
        public int ChoiceId { get; set; }
        public MCQSubQuestionChoice Choice { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<MCQSubQuestionAnswer> b)
        {
            b.HasBaseType<SubQuestionAnswer>()
                .ToTable(nameof(MCQSubQuestionAnswer));
            b.HasOne(e => e.Choice)
                .WithMany()
                .HasForeignKey(e => e.ChoiceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}