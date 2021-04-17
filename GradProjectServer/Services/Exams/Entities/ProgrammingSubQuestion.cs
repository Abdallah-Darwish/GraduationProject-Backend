using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ProgrammingSubQuestion : SubQuestion
    {
        public int CheckerId { get; set; }
        public Program Checker { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ProgrammingSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
                .ToTable(nameof(ProgrammingSubQuestion));
            b.HasOne(q => q.Checker)
                .WithMany()
                .HasForeignKey(q => q.CheckerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static ProgrammingSubQuestion[]? _seed = null;
        public static new ProgrammingSubQuestion[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }

                Random rand = new();
                List<ProgrammingSubQuestion> seed = new();

                foreach (var question in Question.Seed)
                {
                    if (!rand.NextBool()) { continue; }
                    var proCount = rand.Next(1, 3);
                    for (int i = 0; i < proCount; i++)
                    {
                        var pro = new ProgrammingSubQuestion
                        {
                            QuestionId = question.Id,
                            Type = Common.SubQuestionType.Programming,
                        };
                        SeedSubQuestion(pro);
                        if (rand.NextBool())
                        {
                            pro.CheckerId = Program.AllCorrectProgram.Id;
                            pro.Content += $"All correct.{Environment.NewLine}{pro.Content}";
                        }
                        else
                        {
                            pro.CheckerId = Program.AllIncorrectProgram.Id;
                            pro.Content = $"All incorrect.{Environment.NewLine}{pro.Content}";
                        }
                        seed.Add(pro);
                    }
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
