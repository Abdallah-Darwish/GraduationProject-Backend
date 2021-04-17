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
            b.HasCheckConstraint("CK_COURSE_CREDITHOURS", $"\"{nameof(CreditHours)}\" >= 0");
        }
        private static Course[]? _seed = null;
        public static Course[] Seed
        {
            get
            {
                if(_seed != null) { return _seed; }
                _seed = new Course[]
                {
                    new Course
                    {
                        Id = 1,
                        CreditHours = 3,
                        Name = "Arabic Language"
                    },
                    new Course
                    {
                        Id = 2,
                        CreditHours = 3,
                        Name = "National Education"
                    },
                    new Course
                    {
                        Id = 3,
                        CreditHours = 3,
                        Name = "Military Science"
                    },
                    new Course
                    {
                        Id = 4,
                        CreditHours = 3,
                        Name = "Introduction to Computer Science"
                    },
                    new Course
                    {
                        Id = 5,
                        CreditHours = 3,
                        Name = "Structured Programming"
                    },
                    new Course
                    {
                        Id = 6,
                        CreditHours = 1,
                        Name = "Structured ProgrammingLab"
                    },
                    new Course
                    {
                        Id = 7,
                        CreditHours = 3,
                        Name = "Calculus (1)"
                    },
                    new Course
                    {
                        Id = 8,
                        CreditHours = 3,
                        Name = "Calculus (2)"
                    },
                    new Course
                    {
                        Id = 9,
                        CreditHours = 3,
                        Name = "Theory of Computation"
                    },
                    new Course
                    {
                        Id = 10,
                        CreditHours = 3,
                        Name = "Database Systems"
                    },
                    new Course
                    {
                        Id = 11,
                        CreditHours = 3,
                        Name = "Software Engineering"
                    },
                    new Course
                    {
                        Id = 12,
                        CreditHours = 2,
                        Name = "Graduation Project 2"
                    },
                    new Course
                    {
                        Id = 13,
                        CreditHours = 3,
                        Name = "Digital Logic Design"
                    },
                    new Course
                    {
                        Id = 14,
                        CreditHours = 1,
                        Name = "Graduation Project 1"
                    },
                    new Course
                    {
                        Id = 15,
                        CreditHours = 3,
                        Name = "Wireless Networks and Applications"
                    },
                    new Course
                    {
                        Id = 16,
                        CreditHours = 3,
                        Name = "Computer Graphics"
                    },
                    new Course
                    {
                        Id = 17,
                        CreditHours = 3,
                        Name = "Operations Research"
                    },
                    new Course
                    {
                        Id = 18,
                        CreditHours = 3,
                        Name = "Mobile Application Development"
                    },
                    new Course
                    {
                        Id = 19,
                        CreditHours = 3,
                        Name = "Multimedia Systems"
                    },
                    new Course
                    {
                        Id = 20,
                        CreditHours = 3,
                        Name = "History of Science"
                    },
                    new Course
                    {
                        Id = 21,
                        CreditHours = 3,
                        Name = "Arab Islamic Scientific Heritage"
                    },
                    new Course
                    {
                        Id = 22,
                        CreditHours = 3,
                        Name = "Sports and Health"
                    },
                    new Course
                    {
                        Id = 23,
                        CreditHours = 3,
                        Name = "Arabic Literature"
                    },
                    new Course
                    {
                        Id = 24,
                        CreditHours = 3,
                        Name = "Foreign languages"
                    }, new Course
                    {
                        Id = 25,
                        CreditHours = 3,
                        Name = "Entrepreneurship for Business"
                    }, new Course
                    {
                        Id = 26,
                        CreditHours = 3,
                        Name = "Scientific Research Methods"
                    }, new Course
                    {
                        Id = 27,
                        CreditHours = 3,
                        Name = "Business Skills"
                    }
                };
                return _seed;
            }
        }
    }
}
