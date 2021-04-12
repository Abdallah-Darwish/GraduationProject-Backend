using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class CourseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<CourseCategory> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name)
                 .IsUnicode()
                 .IsRequired();
        }
    }
}
