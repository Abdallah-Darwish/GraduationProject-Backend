using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ProgrammingSubQuestion : SubQuestion
    {
        public int CheckerId { get; set; }
        public Program Checker { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ProgrammingSubQuestion> b)
        {
            b.HasOne(q => q.Checker)
                .WithOne()
                .HasForeignKey<ProgrammingSubQuestion>(q => q.CheckerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
