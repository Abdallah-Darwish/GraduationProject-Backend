using System;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Services.CheckersManagers;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.FilesManagers;

namespace GradProjectServer.Services.Exams
{
    /// <summary>
    /// All of the returned grades are percent of sub question weight and they'll be in [0, 1].
    /// </summary>
    public class ExamAttemptGrader
    {
        private readonly AppDbContext _dbContext;
        private readonly BlankCheckerManager _blankCheckerManager;
        private readonly ProgrammingCheckerManager _programmingCheckerManager;

        public ExamAttemptGrader(AppDbContext dbContext,
            BlankCheckerManager blankCheckerManager,
            ProgrammingCheckerManager programmingCheckerManager)
        {
            _dbContext = dbContext;
            _blankCheckerManager = blankCheckerManager;
            _programmingCheckerManager = programmingCheckerManager;
        }
        public async Task<float> Grade(MCQSubQuestionAnswer[] answers)
        {
            if (answers.Length == 0)
            {
                return 0.0f;
            }

            const float MinGrade = -1, MaxGrade = 1;
            float grade = 0;
            foreach (var answer in answers)
            {
                var choice = answer.Choice ??
                             await _dbContext.MCQSubQuestionsChoices.FindAsync(answer.ChoiceId).ConfigureAwait(false);
                grade += choice.Weight;
            }
            grade = Math.Min(MaxGrade, Math.Max(MinGrade, grade));
            grade = Math.Max(0, grade);
            return grade;
        }

        public async Task<BlankCheckerResult> Grade(BlankSubQuestionAnswer answer)
        {
            BlankCheckerResult result;
            var examSubQuestion = answer.ExamSubQuestion ??
                                  await _dbContext.ExamSubQuestions.FindAsync(answer.ExamSubQuestionId)
                                      .ConfigureAwait(false);
            BlankSubQuestion blankSubQuestion = await _dbContext.BlankSubQuestions
                .FindAsync(examSubQuestion.SubQuestionId)
                .ConfigureAwait(false);
            if (!blankSubQuestion.HasChecker)
            {
                result = new()
                {
                    Grade = answer.Answer == blankSubQuestion.Answer ? 1.0f : 0.0f
                };
            }
            else
            {
                result = await _blankCheckerManager.Check(answer).ConfigureAwait(false);
            }
            return result;
        }

        public async Task<ProgrammingCheckerResult> Grade(ProgrammingSubQuestionAnswer answer)
        {
            return await _programmingCheckerManager.Check(answer).ConfigureAwait(false);
        }
    }
}