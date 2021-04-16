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

            b.HasData(Seed);
        }
        //todo: create zip files for programs
        public static Program AllCorrectProgram { get; private set; }
        public static Program AllIncorrectProgram { get; private set; }
        /// <summary>
        /// Expects student answer and author answer, it will return 1 if they match and 0 otherwise
        /// </summary>
        public static Program AnswerComparerProgram { get; private set; }

        private static Program[]? _seed = null;
        public static Program[] Seed
        {
            get
            {
                if(_seed != null) { return _seed; }
                //for now I'll ignore dependecies since I intent to seed them from Configuration even in Production
                _seed = new Program[]
                {
                    AllCorrectProgram = new Program
                    {
                        Id = 1,
                        FileName = "AllCorrect.zip"
                    },
                    AllIncorrectProgram = new Program
                    {
                        Id = 1,
                        FileName = "AllIncorrect.zip"
                    },
                };

                return _seed;
            }
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
