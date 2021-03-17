using GradProjectServer.Services.Exams.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreditHours { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Course> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name)
                .IsRequired()
                .IsUnicode();
            b.Property(c => c.CreditHours)
                .IsRequired();
            b.HasCheckConstraint("CK_COURSE_CREDITHOURS", $@"{nameof(Course.CreditHours)} > 0");
        }
    }
}
