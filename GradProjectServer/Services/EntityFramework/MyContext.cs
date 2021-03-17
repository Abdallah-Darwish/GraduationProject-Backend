using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Reflection;

namespace GradProjectServer.Services.EntityFramework
{
    public class MyContext : DbContext
    {
        public const string EntityConfigurationMethodName = "ConfigureEntity";
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamAttempt> ExamsAttempts { get; set; }
        public DbSet<ExamSubQuestion> ExamsSubQuestions { get; set; }
        public DbSet<SubQuestionAnswer> SubQuestionsAnswers { get; set; }
        public DbSet<MCQSubQuestion> MCQSubQuestions { get; set; }
        public DbSet<MCQSubQuestionChoice> CQSubQuestionsChoices { get; set; }
        public DbSet<BlankSubQuestion> BlankSubQuestions { get; set; }
        public DbSet<ProgrammingSubQuestion> ProgrammingSubQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<SubQuestion> SubQuestions { get; set; }
        public DbSet<SubQuestionTag> SubQuestionsTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CoursesCategories { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<StudyPlan> StudyPlans { get; set; }
        public DbSet<StudyPlanCourse> StudyPlansCourses { get; set; }
        public DbSet<StudyPlanCoursePrerequisite> StudyPlansCoursesPrerequisites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            static MethodInfo? FindEntityConfigurationMethod(Type t)
            {
                var typeMethods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var m in typeMethods)
                {
                    if (m.Name != EntityConfigurationMethodName) { continue; }
                    if (m.ReturnType != typeof(void)) { continue; }
                    if (m.IsGenericMethod) { continue; }
                    var parameters = m.GetParameters();
                    if (parameters.Length != 1) { continue; }
                    if (!parameters[0].ParameterType.IsGenericType) { continue; }
                    var typeBuilder = typeof(EntityTypeBuilder<>).MakeGenericType(t);
                    if (parameters[0].ParameterType != typeBuilder) { continue; }
                    return m;
                }
                return null;
            }

            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetExportedTypes();

            var modelBuilderEntityMethod = modelBuilder
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(m => m.IsGenericMethod && m.Name == nameof(ModelBuilder.Entity));
            foreach (var type in types)
            {
                var configMethod = FindEntityConfigurationMethod(type);
                if (configMethod is null) { continue; }

                var modelBuilderEntityParameters = new object[1] { modelBuilderEntityMethod.MakeGenericMethod(type) };
                object entityTypeBuilder = modelBuilderEntityMethod.MakeGenericMethod(type).Invoke(modelBuilder, modelBuilderEntityParameters)!;
                configMethod.Invoke(null, new object[] { entityTypeBuilder });
            }
        }
    }
}
