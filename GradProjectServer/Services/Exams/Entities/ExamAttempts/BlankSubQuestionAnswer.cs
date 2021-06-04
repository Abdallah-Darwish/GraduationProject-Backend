using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities.ExamAttempts
{
    public class BlankSubQuestionAnswer : SubQuestionAnswer
    {
        public string Answer { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<BlankSubQuestionAnswer> b)
        {
            b.HasBaseType<SubQuestionAnswer>()
                .ToTable(nameof(BlankSubQuestionAnswer));
            b.Property(e => e.Answer)
                .IsUnicode()
                .IsRequired();
        }
    }
}