using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class Major
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<StudyPlan> StudyPlans { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Major> b)
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Name)
                .IsUnicode()
                .IsRequired();
        }
    }
}
