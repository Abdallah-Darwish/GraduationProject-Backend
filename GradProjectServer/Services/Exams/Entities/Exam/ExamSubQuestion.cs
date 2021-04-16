using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ExamSubQuestion
    {
        public int Id { get; set; }
        public int SubQuestionId { get; set; }
        public float Weight { get; set; }
        public int ExamQuestionId { get; set; }
        public int Order { get; set; }
        public SubQuestion SubQuestion { get; set; }
        public ExamQuestion ExamQuestion { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ExamSubQuestion> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Weight)
                .IsRequired();
            b.Property(q => q.Order)
                .IsRequired();
            b.HasOne(q => q.SubQuestion)
                .WithMany()
                .HasForeignKey(q => q.SubQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(q => q.ExamQuestion)
                .WithMany(e => e.ExamSubQuestions)
                .HasForeignKey(q => q.ExamQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasCheckConstraint("CK_EXAMSUBQUESTION_WEIGHT", $@"{nameof(Weight)} > 0");
            b.HasCheckConstraint("CK_EXAMSUBQUESTION_ORDER", $@"{nameof(Order)} >= 0");
        }
        private static ExamSubQuestion[]? _seed = null;
        public static ExamSubQuestion[] Seed
        {
            get
            {
                if (_seed != null) { return _seed; }
                var subQuestionsByQuestion = SubQuestion.Seed
                    .GroupBy(sq => sq.Question)
                    .ToDictionary(g => g.Key, g => g.ToArray());
                Random rand = new();
                List<ExamSubQuestion> seed = new();
                foreach (var examQuestion in ExamQuestion.Seed)
                {
                    var subQuestions = subQuestionsByQuestion[examQuestion.Question];
                    var lastSubQuestionIdx = subQuestions.Length - 1;
                    var examSubQuestionsCount = rand.Next(1, subQuestions.Length);
                    for (int i = 0; i < examSubQuestionsCount; i++)
                    {
                        var subQuestion = rand.NextElementAndSwap(subQuestions, lastSubQuestionIdx--);
                        var examSubQuestion = new ExamSubQuestion
                        {
                            ExamQuestionId = examQuestion.Id,
                            ExamQuestion = examQuestion,
                            Order = rand.Next(1, 100),
                            SubQuestionId = subQuestion.Id,
                            SubQuestion = subQuestion,
                            Weight = (float)rand.NextDouble() * (rand.NextBool() ? 1.0f : -1.0f)
                        };
                        seed.Add(examSubQuestion);
                    }
                }
                for(int i = 1;i<=seed.Count;i++)
                {
                    seed[i - 1].Id = i;
                }
                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}
