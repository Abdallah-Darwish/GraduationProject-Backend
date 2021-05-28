using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using DockerCommon;
using GradProjectServer.Common;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.FilesManagers.Temp;

namespace GradProjectServer.Services.CheckersManagers
{
    public class BlankCheckerManager
    {
        public const string AnswerFileName = "answer.txt";
        public const string ResultFileName = "result.txt";


        private readonly AppDbContext _dbContext;
        private readonly DockerBroker _broker;
        private readonly BlankSubQuestionFileManager _blankSubQuestionFileManager;
        private readonly TempDirectoryManager _tempDirectoryManager;

        public BlankCheckerManager(AppDbContext dbContext, DockerBroker broker,
            BlankSubQuestionFileManager blankSubQuestionFileManager, TempDirectoryManager tempDirectoryManager)
        {
            _dbContext = dbContext;
            _broker = broker;
            _blankSubQuestionFileManager = blankSubQuestionFileManager;
            _tempDirectoryManager = tempDirectoryManager;
        }


        public async Task Build(int subQuestionId)
        {
            var subQuestion =
                await _dbContext.BlankSubQuestions.FindAsync(subQuestionId).ConfigureAwait(false);
            if (!subQuestion.HasChecker)
            {
                throw new ArgumentException($"Blank sub question(Id: {subQuestion.Id}) doesn't have a checker.",
                    nameof(subQuestionId));
            }

            if (subQuestion.IsCheckerBuilt)
            {
                return;
            }

            var buildResult = await _broker.Build(
                PathUtility.MakeRelative(BlankSubQuestionFileManager.GetCheckerSourcePath(subQuestionId)),
                PathUtility.MakeRelative(
                    _blankSubQuestionFileManager.CreateCheckerBinaryDirectory(subQuestionId)));
            if (buildResult != JobResult.Done)
            {
                throw new Exception($"The broker didn't build successfully");
            }

            subQuestion.IsCheckerBuilt = true;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task Build(BlankSubQuestion subQuestion) => Build(subQuestion.Id);

        public async Task<BlankCheckerResult> Check(int answerId)
        {
            var answer = await _dbContext.BlankSubQuestionAnswers.FindAsync(answerId).ConfigureAwait(false);

            var submissionDir = _tempDirectoryManager.Create($"BlankAnswer{answerId}_Submission");
            await using FileStream answerFileStream =
                new(Path.Combine(submissionDir.Directory, AnswerFileName), FileMode.CreateNew, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
            await using StreamWriter answerWriter = new(answerFileStream);
            await answerWriter.WriteAsync(answer.Answer).ConfigureAwait(false);
            await answerWriter.FlushAsync().ConfigureAwait(false);
            try
            {
                await Build(answer.ExamSubQuestionId).ConfigureAwait(false);
            }
            catch
            {
                return new()
                {
                    Comment = "Couldn't build checker successfully",
                    Grade = 0,
                };
            }

            var resultDir = _tempDirectoryManager.Create($"BlankAnswer{answerId}_GradingResult");
            var checkResult = await _broker.Check(submissionDir.RelativeDirectory,
                PathUtility.MakeRelative(
                    BlankSubQuestionFileManager.GetCheckerBinaryDirectory(answer.ExamSubQuestionId)),
                resultDir.RelativeDirectory);

            if (checkResult != JobResult.Done)
            {
                return new()
                {
                    Comment = "Couldn't execute checker successfully",
                    Grade = 0,
                };
            }

            await using FileStream resultFileStream =
                new(Path.Combine(resultDir.Directory, ResultFileName), FileMode.Open, FileAccess.Read, FileShare
                    .ReadWrite);
            using StreamReader resultReader = new(resultFileStream);
            try
            {
                BlankCheckerResult result = new()
                {
                    Grade = float.Parse((await resultReader.ReadLineAsync().ConfigureAwait(false)).Trim()),
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
                };
            }
        }

        public Task<BlankCheckerResult> Check(BlankSubQuestionAnswer answer) => Check(answer.Id);
    }
}