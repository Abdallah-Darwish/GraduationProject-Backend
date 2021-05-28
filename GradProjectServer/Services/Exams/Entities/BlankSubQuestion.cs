using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GradProjectServer.Common;
using GradProjectServer.Services.FilesManagers;
using Microsoft.Extensions.DependencyInjection;

namespace GradProjectServer.Services.Exams.Entities
{
    /// <summary>
    /// If both <see cref="Checker"/> and <see cref="Answer"/> exist then <see cref="Checker"/> will be used and <see cref="Answer"/> will be supplied to it.
    /// </summary>
    public class BlankSubQuestion : SubQuestion
    {
        public bool HasChecker { get; set; }
        public bool IsCheckerBuilt { get; set; }

        /// <summary>
        /// It can't be null because it will be used as a key answer on grading.
        /// </summary>
        public string Answer { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<BlankSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
                .ToTable(nameof(BlankSubQuestion));
            b.Property(q => q.Answer)
                .IsUnicode()
                .IsRequired();
            b.Property(q => q.HasChecker)
                .IsRequired();
            b.Property(q => q.IsCheckerBuilt)
                .IsRequired();
        }

        private static BlankSubQuestion[]? _seed = null;

        public new static BlankSubQuestion[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                Random rand = new();
                List<BlankSubQuestion> seed = new();
                //create at least one sub question for all questions to ensure that they have sub questions
                foreach (var question in Question.Seed)
                {
                    var blankCount = rand.Next(1, 3);
                    for (int i = 1; i <= blankCount; i++)
                    {
                        var blank = new BlankSubQuestion
                        {
                            QuestionId = question.Id,
                            Type = SubQuestionType.Blank,
                            HasChecker = rand.NextBool(),
                        };
                        SeedSubQuestion(blank);
                        blank.Answer = $"{blank.Id}{rand.NextText(rand.Next(1, 3))}";
                        blank.Content = $"Answer: \"{blank.Answer}\".{Environment.NewLine}{blank.Content}";

                        seed.Add(blank);
                    }
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }

        public static async Task CreateSeedFiles(IServiceProvider sp)
        {
            var fileManager = sp.GetRequiredService<BlankSubQuestionFileManager>();
            await using var checkerStream = new MemoryStream();
            using var checkerArchive = new ZipArchive(checkerStream, ZipArchiveMode.Update, true);
            foreach (var fileName in BlankSubQuestionFileManager.RequiredCheckerFiles)
            {
                var fileEntry = checkerArchive.CreateEntry(fileName);
                await using var entryStream = fileEntry.Open();
                await using var entryWriter = new StreamWriter(entryStream);
                if (!fileName.StartsWith("run"))
                {
                    await entryWriter.WriteLineAsync($"echo \"Hello from blank checker file: {fileName}\"")
                        .ConfigureAwait(false);
                }
                else
                {
                    await entryWriter.WriteLineAsync("python checker.py $*").ConfigureAwait(false);
                }
            }

            var pyCheckerEntry = checkerArchive.CreateEntry("checker.py");
            await using (var pyCheckerStream = pyCheckerEntry.Open())
            {
                await using var pyCheckerWriter = new StreamWriter(pyCheckerStream);
                await pyCheckerWriter.WriteAsync(@"import sys
print('{}
");
            }
            checkerArchive.Dispose();

            foreach (var blank in Seed.Where(b => b.HasChecker))
            {
                checkerStream.Position = 0;
                await fileManager.SaveCheckerSource(blank, checkerStream).ConfigureAwait(false);
            }
        }
    }
}