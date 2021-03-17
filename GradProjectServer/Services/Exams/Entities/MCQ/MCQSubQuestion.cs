using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class MCQSubQuestion : SubQuestion
    {
        public bool IsCheckBox { get; set; }
        public ICollection<MCQSubQuestionChoice> Choices { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<MCQSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
             .ToTable(nameof(MCQSubQuestion));
            b.Property(m => m.IsCheckBox)
                .IsRequired();
        }
    }
}
