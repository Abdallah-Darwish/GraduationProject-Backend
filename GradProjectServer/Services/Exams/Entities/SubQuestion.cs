using GradProjectServer.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestion
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public SubQuestionType Type { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public ICollection<SubQuestionTag> Tags { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<SubQuestion> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Content)
                .IsRequired()
                .IsUnicode();
            b.Property(q => q.Type)
                .IsRequired()
                .HasConversion<byte>();
            b.HasOne(sq => sq.Question)
                .WithMany(q => q.SubQuestions)
                .HasForeignKey(sq => sq.QuestionId)
                .IsRequired();
        }
    }
}
