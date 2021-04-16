using GradProjectServer.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestion
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public SubQuestionType Type { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public ICollection<SubQuestionTag> Tags { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<SubQuestion> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Content)
                .IsRequired()
                .IsUnicode();
            b.Property(q => q.Type)
                .IsRequired()
                .HasConversion<byte>();
            b.HasOne(sq => sq.Question)
                .WithMany(q => q.SubQuestions)
                .HasForeignKey(sq => sq.QuestionId)
                .IsRequired();
        }
        private static int _seedId = 1;
        private static readonly Random _seedRand = new();
        /// <summary>
        /// Fills <see cref="SubQuestion.Id"/> and <see cref="SubQuestion.Content"/>.
        /// <see cref="SubQuestion.QuestionId"/> and <see cref="SubQuestion.Question"/> must be already initialized.
        /// </summary>
        /// <param name="subQuestion"></param>
        protected static void SeedSubQuestion(SubQuestion subQuestion)
        {
            _seed.Add(subQuestion);
            subQuestion.Id = _seedId++;
            subQuestion.Content = $"Question {subQuestion.Question.Id}, SubQuestion {subQuestion.Id}. {_seedRand.NextText(_seedRand.Next(40, 200))} .";
        }
        private static readonly List<SubQuestion> _seed = new();

        public static IReadOnlyList<SubQuestion> Seed => _seed;
    }
}
