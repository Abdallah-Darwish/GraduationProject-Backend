using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    /// <summary>
    /// If both <see cref="Checker"/> and <see cref="Answer"/> exist then <see cref="Checker"/> will be used and <see cref="Answer"/> will be supplied to it.
    /// </summary>
    public class BlankSubQuestion : SubQuestion
    {
        public int? CheckerId { get; set; }
        public Program? Checker { get; set; }
        public string? Answer { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<BlankSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
                .ToTable(nameof(BlankSubQuestion));
            b.Property(q => q.Answer)
                .IsUnicode();
            b.HasOne(q => q.Checker)
                .WithOne()
                .HasForeignKey<BlankSubQuestion>(q => q.CheckerId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasData(Seed);
        }

        private static BlankSubQuestion[]? _seed = null;

        public static new BlankSubQuestion[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }

                Random rand = new();
                List<BlankSubQuestion> seed = new();
                //create at least one subquestion for all questions to ensure that they have sub questions
                foreach (var question in Question.Seed)
                {
                    var blankCount = rand.Next(1, 3);
                    for (int i = 1; i <= blankCount; i++)
                    {
                        var blank = new BlankSubQuestion
                        {
                            QuestionId = question.Id,
                            Question = question,
                            Type = Common.SubQuestionType.Blank,
                        };
                        SeedSubQuestion(blank);
                        switch (rand.Next(1, 5))
                        {
                            //answer and no checker
                            case 1:
                                blank.Answer = $"{blank.Id}a";
                                break;
                            //answer and checker
                            case 2:
                                blank.Answer = $"{blank.Id}b";
                                blank.Checker = Program.AnswerComparerProgram;
                                break;
                            //no answer and all correct
                            case 3:
                                blank.Checker = Program.AllCorrectProgram;
                                break;
                            //no answer and all incorrect
                            case 4:
                                blank.Checker = Program.AllIncorrectProgram;
                                break;
                            default:
                                break;
                        }
                        blank.CheckerId = blank.Checker?.Id;
                        if (blank.Answer != null)
                        {
                            blank.Content = $"Answer: \"{blank.Answer}\".{Environment.NewLine}{blank.Content}";
                        }
                    }
                }
                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
