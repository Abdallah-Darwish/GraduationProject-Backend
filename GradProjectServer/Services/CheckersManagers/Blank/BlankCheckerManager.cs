using System;
using System.Threading.Tasks;
using DockerCommon;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.FilesManagers;

namespace GradProjectServer.Services.CheckersManagers
{
    public class BlankCheckerManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly DockerBroker _broker;
        private readonly BlankSubQuestionFileManager _blankSubQuestionFileManager;

        public BlankCheckerManager(AppDbContext appDbContext, DockerBroker broker,
            BlankSubQuestionFileManager blankSubQuestionFileManager)
        {
            _appDbContext = appDbContext;
            _broker = broker;
            _blankSubQuestionFileManager = blankSubQuestionFileManager;
        }
        

        public async Task Build(int subQuestionId)
        {
            var subQuestion =
                await _appDbContext.BlankSubQuestions.FindAsync(subQuestionId).ConfigureAwait(false);
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
            await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task Build(BlankSubQuestion subQuestion) => Build(subQuestion.Id);

        public Task Check(int subQuestionId, int answerId)
        {
            throw new NotImplementedException();
        }
    }
}