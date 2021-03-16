using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Services.Exams;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GradProjectServer.Services.EntityFramework
{
    public class MyContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        protected override void OnModelCreating(ModelBuilder b)
        {
            var subQuestionTag = b.Entity<SubQuestionTag>();
            subQuestionTag.HasKey(t => new { t.SubQustionId, t.TagId });
            subQuestionTag.HasOne(t => t.SubQuestion)
                .WithMany(q => q.Tags)
                .HasForeignKey(t => t.SubQustionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            subQuestionTag.HasOne(t => t.Tag)
                .WithMany(t => t.SubQuestions)
                .HasForeignKey(t => t.TagId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 
                
            var tag = b.Entity<Tag>();
                tag.HasKey(t => t.Id);
            tag.Property(t => t.Name)
            .IsRequired()
            .IsUnicode();
            

            var subQuestion = b.Entity<SubQuestion>();
            subQuestion.HasKey(q => q.Id);
            subQuestion.Property(q => q.Content)
                .IsRequired()
                .IsUnicode();
            subQuestion.Property(q => q.Type)
                .IsRequired()
                .HasConversion<byte>();
            subQuestion.HasOne(sq => sq.Question)
                .WithMany(q => q.SubQuestions)
                .HasForeignKey(sq => sq.QuestionId)
                .IsRequired();

            var question = b.Entity<Question>();
            question.ToTable(nameof(Question))
                .HasKey(q => q.Id);
            question.Property(q => q.Content)
                .IsUnicode()
                .IsRequired();
            question.Property(q => q.IsApproved)
                .HasDefaultValue(false)
                .IsRequired();

            var mcqSubQuestionChoice = b.Entity<MCQSubQuestionChoice>();
            mcqSubQuestionChoice.HasKey(m => new { m.Id, m.QuestionId });
            mcqSubQuestionChoice.Property(m => m.Content)
                .IsRequired()
                .IsUnicode();
            mcqSubQuestionChoice.Property(m => m.Weight)
                .IsRequired();
            mcqSubQuestionChoice.HasOne(m => m.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(m => m.QuestionId);
            
            mcqSubQuestionChoice.HasCheckConstraint("CK_MCQSubQuestionChoice_WEIGHT", $@"{nameof(MCQSubQuestionChoice.Weight)} >= -1 AND {nameof(MCQSubQuestionChoice.Weight)} <= 1");


            var mcqSubQuestion = b.Entity<MCQSubQuestion>();
            mcqSubQuestion
                .HasBaseType<SubQuestion>()
                .ToTable(nameof(MCQSubQuestion));
            mcqSubQuestion.Property(m => m.IsCheckBox)
                .IsRequired();


            

            var program = b.Entity<Program>();
            program.HasKey(p => p.Id);
            program.Property(p => p.Language)
             .IsRequired()
             .HasConversion<byte>();

            var blankSubQuestion = b.Entity<BlankSubQuestion>();
            blankSubQuestion.HasBaseType<SubQuestion>()
                .ToTable(nameof(BlankSubQuestion));
            blankSubQuestion.Property(q => q.Answer)
                .IsUnicode();
            blankSubQuestion.HasOne(q => q.Checker)
                .WithOne()
                .HasForeignKey<BlankSubQuestion>(q => q.CheckerId)
                .OnDelete(DeleteBehavior.Cascade);

            var exam = b.Entity<Exam>();
            exam.HasKey(e => e.Id);
            exam.Property(e => e.Name)
                .IsRequired()
                .IsUnicode();
            exam.Property(e => e.Year)
                .IsRequired()
                .HasConversion<byte>();
            exam.Property(e => e.Semester)
                .IsRequired()
                .HasConversion<byte>(); 
            exam.Property(e => e.Type)
                .IsRequired()
                .HasConversion<byte>();
                
            exam.Property(e => e.Duration)
                .IsRequired()
                .HasConversion(new TimeSpanToTicksConverter());
            exam.HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            exam.HasCheckConstraint("CK_EXAM_YEAR", $@"{nameof(Exam.Year)} >= 1");
            exam.HasCheckConstraint("CK_EXAM_DURATION", $@"{nameof(Exam.Duration)} > 0");

            var examSubQuestion = b.Entity<ExamSubQuestion>();
            examSubQuestion.HasKey(q => q.Id);
            examSubQuestion.Property(q => q.Weight)
                .IsRequired();
            examSubQuestion.HasOne(q => q.SubQuestion)
                .WithMany()
                .HasForeignKey(q => q.SubQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            examSubQuestion.HasOne(q => q.Exam)
                .WithMany(e => e.SubQuestions)
                .HasForeignKey(q => q.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            examSubQuestion.HasCheckConstraint("CK_EXAMSUBQUESTION_WEIGHT", $@"{nameof(ExamSubQuestion.Weight)} > 0");

            var course = b.Entity<Course>();
            course.HasKey(c => c.Id);
            course.Property(c => c.Name)
                .IsRequired()
                .IsUnicode();
            course.Property(c => c.CreditHours)
                .IsRequired();
            course.HasCheckConstraint("CK_COURSE_CREDITHOURS", $@"{nameof(Course.CreditHours)} > 0");

            var studyPlan = b.Entity<StudyPlan>();
            studyPlan.HasKey(p => p.Id);
            studyPlan.Property(p => p.Year)
                .IsRequired();
            studyPlan.HasOne(p => p.Major)
                .WithMany(m => m.StudyPlans)
                .HasForeignKey(p => p.MajorId)
                .IsRequired();

            studyPlan.HasCheckConstraint("CK_STUDYPLAN_YEAR", $@"{nameof(StudyPlan.Year)} > 0");

            var major = b.Entity<Major>();
            major.HasKey(m => m.Id);
            major.Property(m => m.Name)
                .IsUnicode()
                .IsRequired();

            var studyPlanCourse = b.Entity<StudyPlanCourse>();
            studyPlanCourse.HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .IsRequired();
            studyPlanCourse.HasOne(c => c.Category)
               .WithMany(ca => ca.Courses)
               .HasForeignKey(c => c.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            var studyPlanCoursePrerequisite = b.Entity<StudyPlanCoursePrerequisite>();
            studyPlanCoursePrerequisite.HasKey(p => new { p.PrerequisiteId, p.CourseId });
            studyPlanCoursePrerequisite.HasOne(p => p.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(p => p.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            studyPlanCoursePrerequisite.HasOne(p => p.Prerequisite)
               .WithMany()
               .HasForeignKey(p => p.PrerequisiteId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            var courseCategory = b.Entity<CourseCategory>();
            courseCategory.HasKey(c => c.Id);
            courseCategory.Property(c => c.Name)
                .IsUnicode()
                .IsRequired();
            courseCategory.Property(c => c.AllowedCreditHours)
                .IsRequired();
            courseCategory.HasOne(c => c.StudyPlan)
                .WithMany(p => p.Categories)
                .HasForeignKey(c => c.StudyPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            courseCategory.HasCheckConstraint("CK_COURSECATEGORY_ALLOWEDCREDITHOURS", $@"{nameof(CourseCategory.AllowedCreditHours)} > 0");

            var examAttempt = b.Entity<ExamAttempt>();
            examAttempt.HasKey(e => e.Id);
            examAttempt.Property(e => e.StartTime)
                .IsRequired();
            examAttempt.HasOne(a => a.Exam)
                .WithMany()
                .HasForeignKey(a => a.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var subQuestionAnswer = b.Entity<SubQuestionAnswer>();
            subQuestionAnswer.HasKey(a => new { a.AttemptId, a.SubQuestionId });
            subQuestionAnswer.Property(a => a.Answer)
                .IsUnicode();
            subQuestionAnswer.HasOne(a => a.SubQuestion)
                .WithMany()
                .HasForeignKey(a => a.SubQuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            subQuestionAnswer.HasOne(a => a.Attempt)
                .WithMany()
                .HasForeignKey(a => a.AttemptId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
