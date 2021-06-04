using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using DockerCommon;
using GradProjectServer.Common;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.FilesManagers.Temp;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Services.CheckersManagers
{
    public class ProgrammingCheckerManager
    {
        public const string MetadataFilename = "metadata.txt";
        public const string ResultFileName = "result.txt";
        /// <summary>
        /// will be used if the answer is only one non-zip file or if the zip file can't be extracted
        /// </summary>
        public const string AnswerFileName = "answer";
        public const string AnswerDirectoryName = "answer";

        private static readonly ImmutableDictionary<string, ProgrammingSubQuestionAnswerVerdict> VerdictsMapping =
            ImmutableDictionary.CreateRange(
                new[]
                {
                    new KeyValuePair<string, ProgrammingSubQuestionAnswerVerdict>("compilation_error",
                        ProgrammingSubQuestionAnswerVerdict.CompilationError),
                    new KeyValuePair<string, ProgrammingSubQuestionAnswerVerdict>("runtime_error",
                        ProgrammingSubQuestionAnswerVerdict.RuntimeError),
                    new KeyValuePair<string, ProgrammingSubQuestionAnswerVerdict>("time_limit_exceeded",
                        ProgrammingSubQuestionAnswerVerdict.TimeLimitExceeded),
                    new KeyValuePair<string, ProgrammingSubQuestionAnswerVerdict>("wrong_answer",
                        ProgrammingSubQuestionAnswerVerdict.WrongAnswer),
                    new KeyValuePair<string, ProgrammingSubQuestionAnswerVerdict>("partial_accepted",
                        ProgrammingSubQuestionAnswerVerdict.PartialAccepted),
                    new KeyValuePair<string, ProgrammingSubQuestionAnswerVerdict>("accepted",
                        ProgrammingSubQuestionAnswerVerdict.Accepted),
                });
        
        private readonly AppDbContext _dbContext;
        private readonly DockerBroker _broker;
        private readonly ProgrammingSubQuestionFileManager _programmingSubQuestionFileManager;
        private readonly ProgrammingSubQuestionAnswerFileManager _answerFileManager;
        private readonly TempDirectoryManager _tempDirectoryManager;

        public ProgrammingCheckerManager(AppDbContext dbContext, DockerBroker broker,
            ProgrammingSubQuestionFileManager programmingSubQuestionFileManager,
            ProgrammingSubQuestionAnswerFileManager answerFileManager, TempDirectoryManager tempDirectoryManager)
        {
            _dbContext = dbContext;
            _broker = broker;
            _programmingSubQuestionFileManager = programmingSubQuestionFileManager;
            _answerFileManager = answerFileManager;
            _tempDirectoryManager = tempDirectoryManager;
        }


        public async Task Build(int subQuestionId)
        {
            var subQuestion =
                await _dbContext.ProgrammingSubQuestions.FindAsync(subQuestionId).ConfigureAwait(false);
            if (subQuestion.IsCheckerBuilt)
            {
                return;
            }

            var buildResult = await _broker.Build(
                PathUtility.MakeRelative(ProgrammingSubQuestionFileManager.GetCheckerSourcePath(subQuestionId)),
                PathUtility.MakeRelative(
                    _programmingSubQuestionFileManager.CreateCheckerBinaryDirectory(subQuestionId)));
            if (buildResult != JobResult.Done)
            {
                throw new DockerBrokerException(buildResult, $"The broker didn't build successfully");
            }

            subQuestion.IsCheckerBuilt = true;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task Build(ProgrammingSubQuestion subQuestion) => Build(subQuestion.Id);

        public async Task<ProgrammingCheckerResult> Check(int answerId)
        {
            var answer = await _dbContext.ProgrammingSubQuestionAnswers
                .Include(e => e.ExamSubQuestion)
                .FirstAsync(a => a.Id == answerId)
                .ConfigureAwait(false);

            var submissionDir = _tempDirectoryManager.Create($"ProgrammingAnswer{answerId}_Submission");
            await using FileStream metadataFileStream =
                new(Path.Combine(submissionDir.Directory, MetadataFilename), FileMode.CreateNew, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
            await using StreamWriter metadataWriter = new(metadataFileStream);
            await metadataWriter.WriteLineAsync(answer.ProgrammingLanguage.ToString().ToLowerInvariant())
                .ConfigureAwait(false);

            var answerDir = Path.Combine(submissionDir.Directory, AnswerDirectoryName);
            Directory.CreateDirectory(answerDir);
            await using var answerFileStream = _answerFileManager.GetAnswer(answer);
            if (answer.FileExtension.ToLowerInvariant() == "zip")
            {
                try
                {
                    using ZipArchive answerArchive = new(answerFileStream, ZipArchiveMode.Read);
                    answerArchive.ExtractToDirectory(answerDir);
                }
                catch
                {
                    //to ensure that its empty in case it failed mid extraction 
                    if (Directory.Exists(answerDir))
                    {
                        Directory.Delete(answerDir, true);
                    }

                    Directory.CreateDirectory(answerDir);
                    await using FileStream tempAnswerFileStream =
                        new(Path.Combine(answerDir, $"{AnswerFileName}.zip"), FileMode.CreateNew, FileAccess.ReadWrite, FileShare
                            .ReadWrite);
                    await answerFileStream.CopyToAsync(tempAnswerFileStream).ConfigureAwait(false);
                    await tempAnswerFileStream.FlushAsync().ConfigureAwait(false);
                }
            }
            else
            {
                await using FileStream tempAnswerFileStream =
                    new(Path.Combine(answerDir, $"{AnswerFileName}.{answer.FileExtension}"), FileMode.CreateNew, FileAccess.ReadWrite, FileShare
                        .ReadWrite);
                await answerFileStream.CopyToAsync(tempAnswerFileStream).ConfigureAwait(false);
                await tempAnswerFileStream.FlushAsync().ConfigureAwait(false);
            }

            try
            {
                await Build(answer.ExamSubQuestion!.SubQuestionId).ConfigureAwait(false);
            }
            catch
            {
                return new()
                {
                    Comment = "Couldn't build checker successfully",
                    Grade = 0,
                    Verdict = ProgrammingSubQuestionAnswerVerdict.CheckerError
                };
            }

            var resultDir = _tempDirectoryManager.Create($"ProgrammingAnswer{answerId}_GradingResult");
            var checkResult = await _broker.Check(submissionDir.RelativeDirectory,
                PathUtility.MakeRelative(ProgrammingSubQuestionFileManager.GetCheckerBinaryDirectory(answer.ExamSubQuestion.SubQuestionId)),
                resultDir.RelativeDirectory);

            if (checkResult != JobResult.Done)
            {
                return new()
                {
                    Comment = "Couldn't execute checker successfully",
                    Grade = 0,
                    Verdict = ProgrammingSubQuestionAnswerVerdict.CheckerError
                };
            }

            await using FileStream resultFileStream =
                new(Path.Combine(resultDir.Directory, ResultFileName), FileMode.Open, FileAccess.Read, FileShare
                    .ReadWrite);
            using StreamReader resultReader = new(resultFileStream);
            try
            {
                ProgrammingCheckerResult result = new()
                {
                    Grade = float.Parse((await resultReader.ReadLineAsync().ConfigureAwait(false)).Trim()),
                    Verdict = VerdictsMapping[(await resultReader.ReadLineAsync().ConfigureAwait(false)).Trim()],
                    Comment = await resultReader.ReadToEndAsync().ConfigureAwait(false)
                };
                if (string.IsNullOrWhiteSpace(result.Comment))
                {
                    result.Comment = null;
                }

                return result;
            }
            catch
            {
                return new()
                {
                    Comment = "Incorrect checker result file format.",
                    Grade = 0,
                    Verdict = ProgrammingSubQuestionAnswerVerdict.CheckerError
                };
            }
        }

        public Task<ProgrammingCheckerResult> Check(ProgrammingSubQuestionAnswer answer) => Check(answer.Id);
    }
}