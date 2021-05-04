using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services.FilesManagers
{
    public class BlankSubQuestionFileManager
    {
        public static string SaveDirectory { get; private set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "BlankSubQuestionsCheckers");
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        public static string GetResourceFilePath(int subQuestionId) =>
            Path.Combine(SaveDirectory, $"{subQuestionId}.zip");

        public static string GetResourceFilePath(BlankSubQuestion blank) => GetResourceFilePath(blank.Id);
        public static readonly IReadOnlyList<string> RequiredCheckerFiles = new[] {"Build.sh", "Init.sh", "Run.sh"};

        public bool VerifyChecker(Stream checker)
        {
            try
            {
                using var zip = new ZipArchive(checker, ZipArchiveMode.Read, true);
                var entriesNames = zip.Entries.Select(e => e.FullName).ToArray();
                return !RequiredCheckerFiles.Except(entriesNames).Any();
            }
            catch
            {
                return false;
            }
        }

        public Stream GetChecker(int subQuestionId)
        {
            var checkerPath = GetResourceFilePath(subQuestionId);
            return new FileStream(checkerPath, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite);
        }

        public Stream GetChecker(BlankSubQuestion subQuestion) => GetChecker(subQuestion.Id);

        public async Task SaveChecker(int subQuestionId, Stream checker)
        {
            var checkerPath = GetResourceFilePath(subQuestionId);
            await using var checkerFileStream = new FileStream(checkerPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            await checker.CopyToAsync(checkerFileStream).ConfigureAwait(false);
            await checkerFileStream.FlushAsync().ConfigureAwait(false);
            checkerFileStream.SetLength(checkerFileStream.Position);
        }

        public Task SaveChecker(BlankSubQuestion subQuestion, Stream checker) => SaveChecker(subQuestion.Id, checker);

        public void DeleteChecker(int subQuestionId)
        {
            var checkerPath = GetResourceFilePath(subQuestionId);
            if (File.Exists(checkerPath))
            {
                File.Delete(checkerPath);
            }
        }

        public void DeleteChecker(BlankSubQuestion subQuestion) => DeleteChecker(subQuestion.Id);
    }
}