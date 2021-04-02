using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Services.Exams.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<SubQuestion> SubQuestions { get; set; }
        public string Title { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }
        public IEnumerable<Tag> Tags => SubQuestions.SelectMany(s => s.Tags.Select(t => t.Tag)).Distinct();
        public static void ConfigureEntity(EntityTypeBuilder<Question> b)
        {
            b.ToTable(nameof(Question))
               .HasKey(q => q.Id);
            b.Property(q => q.Title)
                .IsUnicode()
                .IsRequired();
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
            b.HasOne(b => b.Volunteer)
                .WithMany(b => b.VolunteeredQuestions)
                .HasForeignKey(b => b.VolunteerId)
                .IsRequired();
        }
    }
}
