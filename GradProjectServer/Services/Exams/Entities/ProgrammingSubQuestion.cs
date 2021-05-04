using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Common;
using GradProjectServer.Services.FilesManagers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ProgrammingSubQuestion : SubQuestion
    {
        public string KeyAnswerFileExtension { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<ProgrammingSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
                .ToTable(nameof(ProgrammingSubQuestion));
            b.Property(q => q.KeyAnswerFileExtension)
                .IsUnicode()
                .IsRequired();
        }

        private static ProgrammingSubQuestion[]? _seed = null;

        public static new ProgrammingSubQuestion[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                Random rand = new();
                List<ProgrammingSubQuestion> seed = new();
                foreach (var question in Question.Seed)
                {
                    if (!rand.NextBool())
                    {
                        continue;
                    }

                    for (int i = 0; i < rand.Next(1, 3); i++)
                    {
                        ProgrammingSubQuestion sq = new()
                        {
                            QuestionId = question.Id,
                            KeyAnswerFileExtension = "txt",
                            Type = SubQuestionType.Programming
                        };
                        SeedSubQuestion(sq);
                        seed.Add(sq);
                    }
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }

        public static async Task CreateSeedFiles(IServiceProvider sp)
        {
            var fileManager = sp.GetRequiredService<ProgrammingSubQuestionFileManager>();
            await using MemoryStream checkerStream = new();
            using ZipArchive checkerArchive = new(checkerStream, ZipArchiveMode.Update, true);
            foreach (var fileName in ProgrammingSubQuestionFileManager.RequiredCheckerFiles)
            {
                var fileEntry = checkerArchive.CreateEntry(fileName);
                await using var entryStream = fileEntry.Open();
                await using StreamWriter entryWriter = new(entryStream);
                await entryWriter.WriteLineAsync($"Hello programming checker file: {fileName}").ConfigureAwait(false);
            }

            checkerArchive.Dispose();
            await using MemoryStream keyAnswerStream = new();
            Random rand = new();
            foreach (var pro in Seed.Where(q => q.Type == SubQuestionType.Programming))
            {
                checkerStream.Position = 0;
                await fileManager.SaveChecker(pro, checkerStream).ConfigureAwait(false);

                keyAnswerStream.Position = 0;
                await using (StreamWriter keyAnswerWriter = new(keyAnswerStream, leaveOpen: true))
                {
                    await keyAnswerWriter.WriteLineAsync($"Programming Question sub question {pro.Id} key answer.")
                        .ConfigureAwait(false);
                    await keyAnswerWriter.WriteLineAsync(rand.NextText()).ConfigureAwait(false);
                }

                keyAnswerStream.SetLength(keyAnswerStream.Position);
                keyAnswerStream.Position = 0;
                await fileManager.SaveKeyAnswer(pro, keyAnswerStream);
            }
        }
    }
}