using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Infrastructure
{
    public enum ProgramLanguage
    {
        CSharp, CPP, Python, Java, Assembly
    }
    public class Program
    {
        public int Id { get; set; }
        public ProgramLanguage Language { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Program> b)
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Language)
             .IsRequired()
             .HasConversion<byte>();
        }
        //init script
        //init code(creating db)
        //companion code(opening tcp connection)
        //post execution code(validating files content)
        //previous three stages can be compined in one with custom command line args
        //binary files like dbs 
        //no need for clean script since the whole container will be deleted
    }
}
