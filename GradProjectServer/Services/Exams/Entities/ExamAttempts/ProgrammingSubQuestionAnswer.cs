using GradProjectServer.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities.ExamAttempts
{
    public class ProgrammingSubQuestionAnswer : SubQuestionAnswer
    {
        public string FileExtension { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<ProgrammingSubQuestionAnswer> b)
        {
            b.HasBaseType<SubQuestionAnswer>()
                .ToTable(nameof(ProgrammingSubQuestionAnswer));
            b.Property(e => e.FileExtension)
                .IsUnicode()
                .IsRequired();
            b.Property(e => e.ProgrammingLanguage)
                .IsRequired()
                .HasConversion<byte>();
        }
    }
}