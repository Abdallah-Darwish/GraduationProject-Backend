using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Common;
using GradProjectServer.Resources;
using GradProjectServer.Services.FilesManagers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace GradProjectServer.Services.Exams.Entities
{
    //todo: maybe we should add "Allowed Answer Programming Languages"
    public class ProgrammingSubQuestion : SubQuestion
    {
        public string KeyAnswerFileExtension { get; set; }
        public bool IsCheckerBuilt { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<ProgrammingSubQuestion> b)
        {
            b.HasBaseType<SubQuestion>()
                .ToTable(nameof(ProgrammingSubQuestion));
            b.Property(q => q.KeyAnswerFileExtension)
                .IsUnicode()
                .IsRequired();
            b.Property(q => q.IsCheckerBuilt)
                .IsRequired()
                .HasDefaultValue(false);
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
                    for (int i = 1, e = rand.Next(1, 2); i <= e; i++)
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
                await using var entryWriter = new StreamWriter(entryStream);
                entryWriter.NewLine = "\n";
                if (fileName.StartsWith("run", StringComparison.OrdinalIgnoreCase))
                {
                    await entryWriter.WriteAsync(await AppResourcesManager.GetText("Run.sh").ConfigureAwait(false)).ConfigureAwait(false);
                }
                else if (fileName.StartsWith("build", StringComparison.OrdinalIgnoreCase))
                {
                    await entryWriter.WriteAsync(await AppResourcesManager.GetText("Build.sh").ConfigureAwait(false)).ConfigureAwait(false);
                }
                else
                {
                    await entryWriter.WriteLineAsync("#!/bin/bash").ConfigureAwait(false);
                    await entryWriter.WriteLineAsync($"echo \"Hello from programming checker file: {fileName}\"")
                        .ConfigureAwait(false);
                }
            }

            var pyCheckerEntry = checkerArchive.CreateEntry("checker.py");
            await using (var pyCheckerStream = pyCheckerEntry.Open())
            {
                await using var pyChecker = AppResourcesManager.GetStream("ProgrammingChecker.py");
                await pyChecker.CopyToAsync(pyCheckerStream).ConfigureAwait(false);
            }

            checkerArchive.Dispose();
            await using MemoryStream keyAnswerStream = new();
            Random rand = new();
            foreach (var pro in Seed)
            {
                checkerStream.Position = 0;
                await fileManager.SaveCheckerSource(pro, checkerStream).ConfigureAwait(false);

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