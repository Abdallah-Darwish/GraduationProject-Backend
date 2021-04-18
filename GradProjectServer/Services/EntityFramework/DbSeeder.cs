
using GradProjectServer.Resources;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;

namespace GradProjectServer.Services.EntityFramework
{
    //works only with postgres
    public class DbSeeder
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;
        public DbSeeder(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        public async Task RecreateDb()
        {
            var postgreConString = _config.GetConnectionString("Postgers");
            using (var postgreCon = new NpgsqlConnection(postgreConString))
            {
                await postgreCon.OpenAsync().ConfigureAwait(false);
                using (var dropCommand = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_config.GetValue<string>("DbName")}\" WITH (FORCE);", postgreCon))
                {
                    await dropCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                using (var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{_config.GetValue<string>("DbName")}\";", postgreCon))
                {
                    await createCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            string dbScript = await ResourcesManager.GetText("DbInitScript.sql").ConfigureAwait(false);

            var dbConString = _config.GetConnectionString("Default");
            using (var dbCon = new NpgsqlConnection(dbConString))
            using (var initCommand = new NpgsqlCommand(dbScript, dbCon))
            {
                await dbCon.OpenAsync().ConfigureAwait(false);
                await initCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
        public async Task Seed()
        {
            await _dbContext.Courses.AddRangeAsync(Course.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.CoursesCategories.AddRangeAsync(CourseCategory.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Majors.AddRangeAsync(Major.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.StudyPlans.AddRangeAsync(StudyPlan.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Users.AddRangeAsync(User.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.StudyPlansCoursesCategories.AddRangeAsync(StudyPlanCourseCategory.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.StudyPlansCourses.AddRangeAsync(StudyPlanCourse.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.StudyPlansCoursesPrerequisites.AddRangeAsync(StudyPlanCoursePrerequisite.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Programs.AddRangeAsync(Program.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Tags.AddRangeAsync(Tag.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Questions.AddRangeAsync(Question.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.ProgrammingSubQuestions.AddRangeAsync(ProgrammingSubQuestion.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.BlankSubQuestions.AddRangeAsync(BlankSubQuestion.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.MCQSubQuestions.AddRangeAsync(MCQSubQuestion.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.MCQSubQuestionsChoices.AddRangeAsync(MCQSubQuestionChoice.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.SubQuestionsTags.AddRangeAsync(SubQuestionTag.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);


            await _dbContext.Exams.AddRangeAsync(Exam.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.ExamsQuestions.AddRangeAsync(ExamQuestion.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.ExamSubQuestions.AddRangeAsync(ExamSubQuestion.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);


        }
    }
}
