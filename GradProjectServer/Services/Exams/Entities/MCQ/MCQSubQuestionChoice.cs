using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities
{
    public class MCQSubQuestionChoice
    {
        public byte Id { get; set; }
        public int QuestionId { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Range [-1, 1]
        /// Where negative means its a wrong answer, it will be considered only if <see cref="MCQSubQuestion.IsCheckBox"/> is true.
        /// If <see cref="MCQSubQuestion.IsCheckBox"/> is false only one <see cref="Weight"/> can be > 0.
        /// </summary>
        public float Weight { get; set; }
        public MCQSubQuestion Question { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<MCQSubQuestionChoice> b)
        {
            b.HasKey(m => new { m.Id, m.QuestionId });
            b.Property(m => m.Content)
                .IsRequired()
                .IsUnicode();
            b.Property(m => m.Weight)
                .IsRequired();
            b.HasOne(m => m.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(m => m.QuestionId);

            b.HasCheckConstraint("CK_MCQSubQuestionChoice_WEIGHT", $@"{nameof(MCQSubQuestionChoice.Weight)} >= -1 AND {nameof(MCQSubQuestionChoice.Weight)} <= 1");

        }
    }
}
