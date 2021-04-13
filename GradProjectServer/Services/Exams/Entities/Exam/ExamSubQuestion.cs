using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ExamSubQuestion
    {
        public int Id { get; set; }
        public int SubQuestionId { get; set; }
        public float Weight { get; set; }
        public int ExamQuestionId { get; set; }
        public SubQuestion SubQuestion { get; set; }
        public ExamQuestion ExamQuestion { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ExamSubQuestion> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Weight)
                .IsRequired();
            b.HasOne(q => q.SubQuestion)
                .WithMany()
                .HasForeignKey(q => q.SubQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(q => q.ExamQuestion)
                .WithMany(e => e.ExamSubQuestions)
                .HasForeignKey(q => q.ExamQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasCheckConstraint("CK_EXAMSUBQUESTION_WEIGHT", $@"{nameof(Weight)} > 0");
        }
    }
}
