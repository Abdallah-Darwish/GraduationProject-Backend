using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SubQuestionTag> SubQuestions { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<Tag> b)
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Name)
                .IsRequired()
                .IsUnicode();
        }

        private static Tag[]? _seed = null;

        public static Tag[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                _seed = new Tag[]
                {
                    new Tag {Name = "Loops"},
                    new Tag {Name = "Limits"},
                    new Tag {Name = "Arrays"},
                    new Tag {Name = "Two Pointers"},
                    new Tag {Name = "Matrices"},
                    new Tag {Name = "Gaussian Elimination"},
                    new Tag {Name = "Present Perfect"},
                    new Tag {Name = "DFA"},
                    new Tag {Name = "NFA"},
                    new Tag {Name = "Regular Expressions"},
                    new Tag {Name = "Trees"},
                    new Tag {Name = "Binary Search"},
                    new Tag {Name = "OOP"},
                    new Tag {Name = "Pointers"},
                };
                for (int i = 1; i <= _seed.Length; i++)
                {
                    _seed[i - 1].Id = i;
                }

                return _seed;
            }
        }
    }
}