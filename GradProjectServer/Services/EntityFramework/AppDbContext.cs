using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Reflection;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.Resources;

namespace GradProjectServer.Services.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public const string EntityConfigurationMethodName = "ConfigureEntity";
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamAttempt> ExamsAttempts { get; set; }
        public DbSet<ExamSubQuestion> ExamSubQuestions { get; set; }
        public DbSet<ExamQuestion> ExamsQuestions { get; set; }
        public DbSet<SubQuestionAnswer> SubQuestionsAnswers { get; set; }
        public DbSet<MCQSubQuestion> MCQSubQuestions { get; set; }
        public DbSet<MCQSubQuestionChoice> MCQSubQuestionsChoices { get; set; }
        public DbSet<BlankSubQuestion> BlankSubQuestions { get; set; }
        public DbSet<ProgrammingSubQuestion> ProgrammingSubQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<SubQuestion> SubQuestions { get; set; }
        public DbSet<SubQuestionTag> SubQuestionsTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CoursesCategories { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<StudyPlan> StudyPlans { get; set; }
        public DbSet<StudyPlanCourse> StudyPlansCourses { get; set; }
        public DbSet<StudyPlanCoursePrerequisite> StudyPlansCoursesPrerequisites { get; set; }
        public DbSet<StudyPlanCourseCategory> StudyPlansCoursesCategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }

        public DbSet<SubQuestionAnswer> SubQuestionAnswers { get; set; }
        public DbSet<BlankSubQuestionAnswer> BlankSubQuestionAnswers { get; set; }
        public DbSet<MCQSubQuestionAnswer> MCQSubQuestionAnswers { get; set; }

        private static MethodInfo? FindEntityConfigurationMethod(Type t)
        {
            var typeMethods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in typeMethods)
            {
                if (m.Name != EntityConfigurationMethodName)
                {
                    continue;
                }

                if (m.ReturnType != typeof(void))
                {
                    continue;
                }

                if (m.IsGenericMethod)
                {
                    continue;
                }

                if (!m.IsStatic)
                {
                    continue;
                }

                if (!m.IsPublic)
                {
                    continue;
                }

                var parameters = m.GetParameters();
                if (parameters.Length != 1)
                {
                    continue;
                }

                if (!parameters[0].ParameterType.IsGenericType)
                {
                    continue;
                }

                var typeBuilder = typeof(EntityTypeBuilder<>).MakeGenericType(t);
                if (parameters[0].ParameterType != typeBuilder)
                {
                    continue;
                }

                return m;
            }

            return null;
        }

        public static bool IsEntity(Type t) => FindEntityConfigurationMethod(t) != null;

        public static Type[] GetAllEntitiesInAssembly(Assembly asm)
        {
            var types = asm.GetExportedTypes();
            return types.Where(IsEntity).ToArray();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var modelBuilderEntityMethod = modelBuilder
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(m => m.IsGenericMethod && m.Name == nameof(ModelBuilder.Entity));

            var entitiesTypes = GetAllEntitiesInAssembly(Assembly.GetExecutingAssembly());
            foreach (var type in entitiesTypes)
            {
                var configMethod = FindEntityConfigurationMethod(type)!;

                object entityTypeBuilder = modelBuilderEntityMethod.MakeGenericMethod(type).Invoke(modelBuilder, null)!;
                configMethod.Invoke(null, new object[] {entityTypeBuilder});
            }
        }
    }
}