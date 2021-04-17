using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ExamQuestion
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public ICollection<ExamSubQuestion> ExamSubQuestions { get; set; }
        public Exam Exam { get; set; }
        public Question Question  { get;set;}
        public static void ConfigureEntity(EntityTypeBuilder<ExamQuestion> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Order)
                .IsRequired();
            b.HasOne(q => q.Exam)
                .WithMany(e => e.Questions)
                .HasForeignKey(q => q.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(q => q.Question)
                .WithMany()
                .HasForeignKey(q => q.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static ExamQuestion[]? _seed = null;
        public static ExamQuestion[] Seed
        {
            get
            {
                if(_seed != null) { return _seed; }

                Random rand = new();
                List<ExamQuestion> seed = new();
                var questionsByCourse = Question.Seed
                    .GroupBy(q => q.Course)
                    .ToDictionary(g => g.Key, g => g.ToArray());
                foreach (var exam in Exam.Seed)
                {
                    var questions = questionsByCourse[exam.Course];
                    int questionsLastIndex = questions.Length - 1;
                    var questionsCount = rand.Next(1, questions.Length);
                    for (int i = 0; i < questionsCount; i++)
                    {
                        var question = rand.NextElementAndSwap(questions, questionsLastIndex--);
                        var examQuestion = new ExamQuestion
                        {
                            ExamId = exam.Id,
                            Order = rand.Next(1, 100),
                            QuestionId = question.Id,
                        };
                        seed.Add(examQuestion);
                    }
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
