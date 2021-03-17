using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<SubQuestion> SubQuestions { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        //Tags, Course, volunteer
        public static void ConfigureEntity(EntityTypeBuilder<Question> b)
        {
            b.ToTable(nameof(Question))
               .HasKey(q => q.Id);
            b.Property(q => q.Content)
                .IsUnicode()
                .IsRequired();
            b.Property(q => q.IsApproved)
                .HasDefaultValue(false)
                .IsRequired();
            b.HasOne(q => q.Course)
                .WithMany()
                .HasForeignKey(q => q.CourseId)
                .IsRequired();

        }
    }
}
