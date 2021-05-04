using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Services.Exams.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services.FilesManagers
{
    public class ProgrammingSubQuestionFileManager
    {
        public static string SaveDirectory { get; private set; }
        public static string KeyAnswerSaveDirectory { get; private set; }
        public static string CheckerSaveDirectory { get; private set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "ProgrammingProgrammingSubQuestions");
            KeyAnswerSaveDirectory = Path.Combine(SaveDirectory, "KeyAnswers");
            CheckerSaveDirectory = Path.Combine(SaveDirectory, "Checkers");
            foreach (var dir in new string[] {SaveDirectory, KeyAnswerSaveDirectory, CheckerSaveDirectory})
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        public static string GetCheckerPath(int subQuestionId) =>
            Path.Combine(CheckerSaveDirectory, $"{subQuestionId}.zip");

        public static string GetCheckerPath(ProgrammingSubQuestion subQuestion) => GetCheckerPath(subQuestion.Id);

        public static string GetKeyAnswerPath(int subQuestionId, string extension) =>
            Path.Combine(KeyAnswerSaveDirectory, $"{subQuestionId}.{extension}");

        public static string GetKeyAnswerPath(ProgrammingSubQuestion subQuestion) =>
            GetKeyAnswerPath(subQuestion.Id, subQuestion.KeyAnswerFileExtension);

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

        public async Task SaveChecker(int subQuestionId, Stream checker)
        {
            var checkerPath = GetCheckerPath(subQuestionId);
            await using var checkerFileStream = new FileStream(checkerPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            await checker.CopyToAsync(checkerFileStream).ConfigureAwait(false);
            checkerFileStream.SetLength(checkerFileStream.Position);
        }

        public Task SaveChecker(ProgrammingSubQuestion subQuestion, Stream checker) =>
            SaveChecker(subQuestion.Id, checker);

        public Stream GetChecker(int subQuestionId)
        {
            var checkerPath = GetCheckerPath(subQuestionId);
            return new FileStream(checkerPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public Stream GetChecker(ProgrammingSubQuestion subQuestion) => GetChecker(subQuestion.Id);

        public void DeleteChecker(int subQuestionId)
        {
            var checkerPath = GetCheckerPath(subQuestionId);
            if (File.Exists(checkerPath))
            {
                File.Delete(checkerPath);
            }
        }

        public void DeleteChecker(ProgrammingSubQuestion subQuestion) => DeleteChecker(subQuestion.Id);

        public async Task SaveKeyAnswer(int subQuestionId, string extension, Stream keyAnswer)
        {
            var keyAnswerPath = GetKeyAnswerPath(subQuestionId, extension);
            await using var keyAnswerFileStream = new FileStream(keyAnswerPath, FileMode.OpenOrCreate,
                FileAccess.ReadWrite, FileShare.ReadWrite);
            await keyAnswer.CopyToAsync(keyAnswerFileStream).ConfigureAwait(false);
            keyAnswerFileStream.SetLength(keyAnswerFileStream.Position);
        }

        public Task SaveKeyAnswer(ProgrammingSubQuestion subQuestion, Stream keyAnswer) =>
            SaveKeyAnswer(subQuestion.Id, subQuestion.KeyAnswerFileExtension, keyAnswer);

        public Stream GetKeyAnswer(int subQuestionId, string extension)
        {
            var keyAnswerPath = GetKeyAnswerPath(subQuestionId, extension);
            return new FileStream(keyAnswerPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public Stream GetKeyAnswer(ProgrammingSubQuestion subQuestion) =>
            GetKeyAnswer(subQuestion.Id, subQuestion.KeyAnswerFileExtension);

        public void DeleteKeyAnswer(int subQuestionId, string extension)
        {
            var keyAnswerPath = GetKeyAnswerPath(subQuestionId, extension);
            if (File.Exists(keyAnswerPath))
            {
                File.Delete(keyAnswerPath);
            }
        }

        public void DeleteKeyAnswer(ProgrammingSubQuestion subQuestion) =>
            DeleteKeyAnswer(subQuestion.Id, subQuestion.KeyAnswerFileExtension);
    }
}