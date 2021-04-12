using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace GradProjectServer.Services.Infrastructure
{
    public class Program
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public ICollection<ProgramDependency> Dependencies { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Program> b)
        {
            b.HasKey(p => p.Id);

            b.Property(p => p.FileName)
                .IsUnicode()
                .IsRequired();
        }
        //init script
        //init code(creating db)
        //companion code(opening tcp connection)
        //post execution code(validating files content)
        //previous three stages can be combined in one with custom command line args
        //binary files like dbs 
        //no need for clean script since the whole container will be deleted
    }
}
