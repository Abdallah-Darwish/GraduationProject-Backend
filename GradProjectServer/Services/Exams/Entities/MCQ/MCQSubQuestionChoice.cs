using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class MCQSubQuestionChoice
    {
        public int Id { get; set; }
        public int SubQuestionId { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Range [-1, 1]
        /// Where negative means its a wrong answer, it will be considered only if <see cref="MCQSubQuestion.IsCheckBox"/> is true.
        /// If <see cref="MCQSubQuestion.IsCheckBox"/> is false only one <see cref="Weight"/> can be > 0.
        /// </summary>
        public float Weight { get; set; }
        public MCQSubQuestion SubQuestion { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<MCQSubQuestionChoice> b)
        {
            b.HasKey(m => new { m.Id, m.SubQuestionId });
            b.Property(m => m.Content)
                .IsRequired()
                .IsUnicode();
            b.Property(m => m.Weight)
                .IsRequired();
            b.HasOne(m => m.SubQuestion)
                .WithMany(q => q.Choices)
                .HasForeignKey(m => m.SubQuestionId);

            b.HasCheckConstraint("CK_MCQSubQuestionChoice_WEIGHT", $@"{nameof(Weight)} >= -1 AND {nameof(Weight)} <= 1");

        }
        private static MCQSubQuestionChoice[]? _seed = null;
        public static MCQSubQuestionChoice[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }
                Random rand = new();
                List<MCQSubQuestionChoice> seed = new();
                List<MCQSubQuestionChoice> choices = new();
                foreach (var mcq in MCQSubQuestion.Seed)
                {
                    choices.Clear();
                    var choicesCount = rand.Next(2, 5);
                    for (int i = 0; i < choicesCount; i++)
                    {
                        var choice = new MCQSubQuestionChoice
                        {
                            SubQuestionId = mcq.Id,
                            SubQuestion = mcq,
                            Content = $"Choice {i}, Subquestion {mcq.Id}, Question {mcq.QuestionId}"
                        };
                        choices.Add(choice);
                    }
                    if (mcq.IsCheckBox)
                    {
                        foreach (var choice in choices)
                        {
                            choice.Weight = (float)rand.NextDouble();
                            if (rand.NextBool())
                            {
                                choice.Weight *= -1;
                            }
                        }
                    }
                    else
                    {
                        rand.NextElement(choices).Weight = 1;
                    }
                    seed.AddRange(choices);
                }
                for (int i = 1; i <= seed.Count; i++)
                {
                    seed[i - 1].Id = i;
                }
                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
