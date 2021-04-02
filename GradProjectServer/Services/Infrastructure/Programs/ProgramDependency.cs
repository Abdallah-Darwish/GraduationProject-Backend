using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class ProgramDependency
    {
        public int ProgramId { get; set; }
        public int DependecyId { get; set; }
        public Program Program { get; set; }
        public Dependency Dependency { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<ProgramDependency> b)
        {
            b.HasKey(pd => new { pd.ProgramId, pd.DependecyId });
            b.HasOne(pd => pd.Program)
                .WithMany(p => p.Dependencies)
                .HasForeignKey(p => p.ProgramId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(pd => pd.Dependency)
                .WithMany()
                .HasForeignKey(p => p.DependecyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
