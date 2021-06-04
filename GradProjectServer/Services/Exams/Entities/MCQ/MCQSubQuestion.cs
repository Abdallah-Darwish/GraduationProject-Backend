using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class MCQSubQuestion : SubQuestion
    {
        public bool IsCheckBox { get; set; }
        public ICollection<MCQSubQuestionChoice> Choices { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<MCQSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
                .ToTable(nameof(MCQSubQuestion));
            b.Property(m => m.IsCheckBox)
                .IsRequired();
        }

        private static MCQSubQuestion[]? _seed = null;

        public static new MCQSubQuestion[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                Random rand = new();
                List<MCQSubQuestion> seed = new();
                foreach (var question in Question.Seed)
                {
                    for (int i = 1, e = rand.Next(1, 3); i <= e; i++)
                    {
                        var sq = new MCQSubQuestion
                        {
                            QuestionId = question.Id,
                            IsCheckBox = rand.NextBool(),
                            Type = Common.SubQuestionType.MultipleChoice
                        };
                        SeedSubQuestion(sq);
                        seed.Add(sq);
                    }
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}