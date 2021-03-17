using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestionTag
    {
        public int SubQustionId { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public SubQuestion SubQuestion { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<SubQuestionTag> b)
        {
            b.HasKey(t => new { t.SubQustionId, t.TagId });
            b.HasOne(t => t.SubQuestion)
                .WithMany(q => q.Tags)
                .HasForeignKey(t => t.SubQustionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(t => t.Tag)
                .WithMany(t => t.SubQuestions)
                .HasForeignKey(t => t.TagId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
