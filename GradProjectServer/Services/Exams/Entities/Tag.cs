using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SubQuestionTag> SubQuestions { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Tag> b)
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Name)
                .IsRequired()
                .IsUnicode();
        }
    }
}
