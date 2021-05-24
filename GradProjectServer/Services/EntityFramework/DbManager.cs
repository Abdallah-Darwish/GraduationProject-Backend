using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GradProjectServer.Resources;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;
using GradProjectServer.Controllers;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace GradProjectServer.Services.EntityFramework
{
    //works only with postgres
    public class DbManager
    {
        private readonly AppDbContext _dbContext;
        private readonly AppOptions _appOptions;
        private readonly IServiceProvider _serviceProvider;

        public DbManager(AppDbContext dbContext, IOptions<AppOptions> appOptions, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _appOptions = appOptions.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task RecreateDb()
        {
            var dirs = new[]
            {
                BlankSubQuestionFileManager.SaveDirectory,
                ProgrammingSubQuestionFileManager.CheckerSourceSaveDirectory,
                ProgrammingSubQuestionFileManager.CheckerBinarySaveDirectory,
                ProgrammingSubQuestionFileManager.KeyAnswerSaveDirectory,
                ProgrammingSubQuestionAnswerFileManager.SaveDirectory,
                ResourceFileManager.SaveDirectory,
                UserFileManager.SaveDirectory
            };
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }

                Directory.CreateDirectory(dir);
            }

            using (var postgreCon = new NpgsqlConnection(_appOptions.BuildPostgresConnectionString()))
            {
                await postgreCon.OpenAsync().ConfigureAwait(false);
                using (var dropCommand =
                    new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_appOptions.DbName}\" WITH (FORCE);", postgreCon))
                {
                    await dropCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                using (var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{_appOptions.DbName}\";", postgreCon))
                {
                    await createCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }

            NpgsqlConnection.ClearAllPools();
            string dbScript = await ResourcesManager.GetText("DbInitScript.sql").ConfigureAwait(false);

            using (var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString()))
            using (var initCommand = new NpgsqlCommand(dbScript, dbCon))
            {
                await dbCon.OpenAsync().ConfigureAwait(false);
                await initCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task EnsureDb()
        {
            try
            {
                await using var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString());
                await dbCon.OpenAsync().ConfigureAwait(false);
            }
            catch
            {
                await RecreateDb().ConfigureAwait(false);
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

            await _dbContext.StudyPlansCoursesCategories.AddRangeAsync(StudyPlanCourseCategory.Seed)
                .ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.StudyPlansCourses.AddRangeAsync(StudyPlanCourse.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.StudyPlansCoursesPrerequisites.AddRangeAsync(StudyPlanCoursePrerequisite.Seed)
                .ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Tags.AddRangeAsync(Tag.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Questions.AddRangeAsync(Question.Seed).ConfigureAwait(false);
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

            await _dbContext.Resources.AddRangeAsync(Resources.Resource.Seed).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await User.CreateSeedFiles(_serviceProvider).ConfigureAwait(false);
            await Resource.CreateSeedFiles(_serviceProvider).ConfigureAwait(false);
            await BlankSubQuestion.CreateSeedFiles(_serviceProvider).ConfigureAwait(false);
            await ProgrammingSubQuestion.CreateSeedFiles(_serviceProvider).ConfigureAwait(false);

            List<string> tablesNames = new();

            await using (var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString()))
            await using (var listCommand =
                new NpgsqlCommand(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';",
                    dbCon))
            {
                await dbCon.OpenAsync().ConfigureAwait(false);
                await using var reader = await listCommand.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    tablesNames.Add(reader.GetString(0));
                }
            }

            foreach (var table in tablesNames)
            {
                var conString = _appOptions.BuildAppConnectionString();
                try
                {
                    await using var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString());
                    await using var resetCommand =
                        new NpgsqlCommand(
                            $"SELECT setval(pg_get_serial_sequence('\"{table}\"', 'Id'), coalesce(max(\"Id\"),0) + 1, false) FROM \"{table}\";",
                            dbCon);
                    await dbCon.OpenAsync().ConfigureAwait(false);
                    await resetCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (NpgsqlException)
                {
                }
            }
        }
    }
}