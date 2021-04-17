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
        private static Major[]? _seed = null;
        public static Major[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }
                _seed = new Major[]
                {
                    new Major
                    {
                        Id = 1,
                        Name = "Computer Science"
                    },
                    new Major
                    {
                        Id = 2,
                        Name = "Software Engineering"
                    },
                    new Major
                    {
                        Id = 3,
                        Name = "Computer Animation"
                    }
                };
                return _seed;
            }
        }
    }
}
