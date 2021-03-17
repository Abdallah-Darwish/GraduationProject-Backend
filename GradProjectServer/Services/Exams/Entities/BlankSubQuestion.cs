using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        }
    }
}
