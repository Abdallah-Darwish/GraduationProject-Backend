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
        public static string CheckerSourceSaveDirectory { get; set; }
        public static string CheckerBinarySaveDirectory { get; set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "BlankSubQuestions");
            CheckerSourceSaveDirectory = Path.Combine(SaveDirectory, "CheckersSources");
            CheckerBinarySaveDirectory = Path.Combine(SaveDirectory, "CheckersBinaries");
            foreach (var dir in new string[] {SaveDirectory, CheckerSourceSaveDirectory, CheckerBinarySaveDirectory})
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        #region CheckerBinary

        public static string GetCheckerBinaryDirectory(int subQuestionId) =>
            Path.Combine(CheckerBinarySaveDirectory, subQuestionId.ToString());

        public static string GetCheckerBinaryDirectory(BlankSubQuestion subQuestion) =>
            GetCheckerBinaryDirectory(subQuestion.Id);
        
        public void DeleteCheckerBinary(int subQuestionId)
        {
            var checkerDir = GetCheckerBinaryDirectory(subQuestionId);
            if (Directory.Exists(checkerDir))
            {
                Directory.Delete(checkerDir, true);
            }
        }

        public void DeleteCheckerBinary(BlankSubQuestion subQuestion) => DeleteCheckerBinary(subQuestion.Id);

        public string CreateCheckerBinaryDirectory(int subQuestionId)
        {
            var checkerDir = GetCheckerBinaryDirectory(subQuestionId);
            if (!Directory.Exists(checkerDir))
            {
                Directory.CreateDirectory(checkerDir);
            }

            return checkerDir;
        }
        public string CreateCheckerBinaryDirectory(BlankSubQuestion subQuestion) => CreateCheckerBinaryDirectory(subQuestion.Id);

        #endregion

        #region CheckerSource
        public static string GetCheckerSourcePath(int subQuestionId) =>
            Path.Combine(CheckerSourceSaveDirectory, $"{subQuestionId}.zip");

        public static string GetCheckerSourcePath(BlankSubQuestion blank) => GetCheckerSourcePath(blank.Id);
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

        public Stream GetCheckerSource(int subQuestionId)
        {
            var checkerPath = GetCheckerSourcePath(subQuestionId);
            return new FileStream(checkerPath, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite);
        }

        public Stream GetCheckerSource(BlankSubQuestion subQuestion) => GetCheckerSource(subQuestion.Id);

        public async Task SaveCheckerSource(int subQuestionId, Stream checker)
        {
            var checkerPath = GetCheckerSourcePath(subQuestionId);
            await using var checkerFileStream = new FileStream(checkerPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            await checker.CopyToAsync(checkerFileStream).ConfigureAwait(false);
            await checkerFileStream.FlushAsync().ConfigureAwait(false);
            checkerFileStream.SetLength(checkerFileStream.Position);
        }

        public Task SaveCheckerSource(BlankSubQuestion subQuestion, Stream checker) =>
            SaveCheckerSource(subQuestion.Id, checker);

        public void DeleteCheckerSource(int subQuestionId)
        {
            var checkerPath = GetCheckerSourcePath(subQuestionId);
            if (File.Exists(checkerPath))
            {
                File.Delete(checkerPath);
            }
        }

        public void DeleteCheckerSource(BlankSubQuestion subQuestion) => DeleteCheckerSource(subQuestion.Id);

        #endregion
    }
}