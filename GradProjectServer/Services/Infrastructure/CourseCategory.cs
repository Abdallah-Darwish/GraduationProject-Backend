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

            b.HasData(Seed);
        }
        private static CourseCategory[]? _seed = null;
        public static CourseCategory[] Seed
        {
            get
            {
                if(_seed != null) { return _seed; }
                _seed = new CourseCategory[]
                {
                    new CourseCategory
                    {
                        Id = 1,
                        Name = "Compulsory University Requirements"
                    },
                    new CourseCategory
                    {
                        Id = 2,
                        Name = "Compulsory School Requirements"
                    },
                    new CourseCategory
                    {
                        Id = 3,
                        Name = "Compulsory Program Requirements"
                    },
                    new CourseCategory
                    {
                        Id = 4,
                        Name = "Elective Program Requirements"
                    },
                    new CourseCategory
                    {
                        Id = 5,
                        Name = "Elective University Requirements (General)"
                    },
                    new CourseCategory
                    {
                        Id = 6,
                        Name = "Elective University Requirements (Scientific, Practical)"
                    }
                };
                return _seed;
            }
        }
    }
}
