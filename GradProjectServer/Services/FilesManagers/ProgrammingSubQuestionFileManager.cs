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
        public static string CheckerSourceSaveDirectory { get; private set; }
        public static string CheckerBinarySaveDirectory { get; private set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "ProgrammingSubQuestions");
            KeyAnswerSaveDirectory = Path.Combine(SaveDirectory, "KeyAnswers");
            CheckerSourceSaveDirectory = Path.Combine(SaveDirectory, "CheckersSources");
            CheckerBinarySaveDirectory = Path.Combine(SaveDirectory, "CheckersBinaries");
            foreach (var dir in new string[] {SaveDirectory, KeyAnswerSaveDirectory, CheckerSourceSaveDirectory, CheckerBinarySaveDirectory})
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        #region CheckerSource

        public static string GetCheckerSourcePath(int subQuestionId) =>
            Path.Combine(CheckerSourceSaveDirectory, $"{subQuestionId}.zip");

        public static string GetCheckerSourcePath(ProgrammingSubQuestion subQuestion) => GetCheckerSourcePath(subQuestion.Id);
        
        public static readonly IReadOnlyList<string> RequiredCheckerFiles = new[] {"build.sh", "init.sh", "run.sh"};

        public bool VerifyCheckerSource(Stream checker)
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

        public async Task SaveCheckerSource(int subQuestionId, Stream checker)
        {
            var checkerPath = GetCheckerSourcePath(subQuestionId);
            await using var checkerFileStream = new FileStream(checkerPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            await checker.CopyToAsync(checkerFileStream).ConfigureAwait(false);
            checkerFileStream.SetLength(checkerFileStream.Position);
        }

        public Task SaveCheckerSource(ProgrammingSubQuestion subQuestion, Stream checker) =>
            SaveCheckerSource(subQuestion.Id, checker);

        public Stream GetCheckerSource(int subQuestionId)
        {
            var checkerPath = GetCheckerSourcePath(subQuestionId);
            return new FileStream(checkerPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public Stream GetCheckerSource(ProgrammingSubQuestion subQuestion) => GetCheckerSource(subQuestion.Id);

        public void DeleteCheckerSource(int subQuestionId)
        {
            var checkerPath = GetCheckerSourcePath(subQuestionId);
            if (File.Exists(checkerPath))
            {
                File.Delete(checkerPath);
            }
        }

        public void DeleteCheckerSource(ProgrammingSubQuestion subQuestion) => DeleteCheckerSource(subQuestion.Id);

        #endregion

        #region CheckerBinary
        public static string GetCheckerBinaryDirectory(int subQuestionId) =>
            Path.Combine(CheckerBinarySaveDirectory, subQuestionId.ToString());

        public static string GetCheckerBinaryDirectory(ProgrammingSubQuestion subQuestion) =>
            GetCheckerBinaryDirectory(subQuestion.Id);

        public void DeleteCheckerBinary(int subQuestionId)
        {
            var checkerDir = GetCheckerBinaryDirectory(subQuestionId);
            if (Directory.Exists(checkerDir))
            {
                Directory.Delete(checkerDir, true);
            }
        }

        public void DeleteCheckerBinary(ProgrammingSubQuestion subQuestion) => DeleteCheckerBinary(subQuestion.Id);

        public string CreateCheckerBinaryDirectory(int subQuestionId)
        {
            var checkerDir = GetCheckerBinaryDirectory(subQuestionId);
            if (!Directory.Exists(checkerDir))
            {
                Directory.CreateDirectory(checkerDir);
            }

            return checkerDir;
        }
        public string CreateCheckerBinaryDirectory(ProgrammingSubQuestion subQuestion) => CreateCheckerBinaryDirectory(subQuestion.Id);
        #endregion

        #region KeyAnswer
        public static string GetKeyAnswerPath(int subQuestionId, string extension) =>
            Path.Combine(KeyAnswerSaveDirectory, $"{subQuestionId}.{extension}");

        public static string GetKeyAnswerPath(ProgrammingSubQuestion subQuestion) =>
            GetKeyAnswerPath(subQuestion.Id, subQuestion.KeyAnswerFileExtension);

       
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
        

        #endregion
    }
}