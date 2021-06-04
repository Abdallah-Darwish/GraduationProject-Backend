using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestionTag
    {
        public int SubQuestionId { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public SubQuestion SubQuestion { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<SubQuestionTag> b)
        {
            b.HasKey(t => new {t.SubQuestionId, t.TagId});
            b.HasOne(t => t.SubQuestion)
                .WithMany(q => q.Tags)
                .HasForeignKey(t => t.SubQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(t => t.Tag)
                .WithMany(t => t.SubQuestions)
                .HasForeignKey(t => t.TagId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static SubQuestionTag[]? _seed = null;

        public static SubQuestionTag[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                Random rand = new();
                List<SubQuestionTag> seed = new();
                var tags = Tag.Seed.ToArray();
                var maxTagsCount = Math.Min(5, tags.Length);
                int lastTagIndex;
                foreach (var subQuestion in SubQuestion.Seed)
                {
                    var tagCount = rand.Next(maxTagsCount);
                    lastTagIndex = tags.Length - 1;
                    for (int i = 0; i < tagCount; i++)
                    {
                        var tag = rand.NextElementAndSwap(tags, lastTagIndex--);
                        var subQuestionTag = new SubQuestionTag
                        {
                            SubQuestionId = subQuestion.Id,
                            TagId = tag.Id,
                        };

                        seed.Add(subQuestionTag);
                    }
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}