﻿// <auto-generated />
using System;
using GradProjectServer.Services.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GradProjectServer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Exam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CourseId")
                        .HasColumnType("integer");

                    b.Property<long>("Duration")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsApproved")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<byte>("Semester")
                        .HasColumnType("smallint");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint");

                    b.Property<int>("VolunteerId")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("VolunteerId");

                    b.ToTable("Exams");

                    b.HasCheckConstraint("CK_EXAM_YEAR", "\"Year\" >= 1");

                    b.HasCheckConstraint("CK_EXAM_DURATION", "\"Duration\" > 0");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamAttempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ExamId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.ToTable("ExamsAttempts");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ExamId")
                        .HasColumnType("integer");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<int>("QuestionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.HasIndex("QuestionId");

                    b.ToTable("ExamsQuestions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamSubQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ExamQuestionId")
                        .HasColumnType("integer");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<int>("SubQuestionId")
                        .HasColumnType("integer");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ExamQuestionId");

                    b.HasIndex("SubQuestionId");

                    b.ToTable("ExamSubQuestions");

                    b.HasCheckConstraint("CK_EXAMSUBQUESTION_WEIGHT", "\"Weight\" > 0");

                    b.HasCheckConstraint("CK_EXAMSUBQUESTION_ORDER", "\"Order\" >= 0");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.MCQSubQuestionChoice", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<int>("SubQuestionId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.HasKey("Id", "SubQuestionId");

                    b.HasIndex("SubQuestionId");

                    b.ToTable("MCQSubQuestionsChoices");

                    b.HasCheckConstraint("CK_MCQSubQuestionChoice_WEIGHT", "\"Weight\" >= -1 AND \"Weight\" <= 1");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<int>("CourseId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsApproved")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("Title")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<int>("VolunteerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("VolunteerId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<int>("QuestionId")
                        .HasColumnType("integer");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("SubQuestions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestionAnswer", b =>
                {
                    b.Property<int>("AttemptId")
                        .HasColumnType("integer");

                    b.Property<int>("SubQuestionId")
                        .HasColumnType("integer");

                    b.Property<string>("Answer")
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("AttemptId", "SubQuestionId");

                    b.HasIndex("SubQuestionId");

                    b.ToTable("SubQuestionsAnswers");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestionTag", b =>
                {
                    b.Property<int>("SubQuestionId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.HasKey("SubQuestionId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("SubQuestionsTags");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CreditHours")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Courses");

                    b.HasCheckConstraint("CK_COURSE_CREDITHOURS", "\"CreditHours\" >= 0");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.CourseCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CoursesCategories");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Dependency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Dependencies");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Major", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Majors");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Program", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FileName")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.ProgramDependency", b =>
                {
                    b.Property<int>("ProgramId")
                        .HasColumnType("integer");

                    b.Property<int>("DependecyId")
                        .HasColumnType("integer");

                    b.HasKey("ProgramId", "DependecyId");

                    b.HasIndex("DependecyId");

                    b.ToTable("ProgramsDependencies");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("MajorId")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MajorId", "Year")
                        .IsUnique();

                    b.ToTable("StudyPlans");

                    b.HasCheckConstraint("CK_STUDYPLAN_YEAR", "\"Year\" > 0");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("CourseId")
                        .HasColumnType("integer");

                    b.Property<int?>("StudyPlanId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("StudyPlanId");

                    b.HasIndex("CourseId", "CategoryId")
                        .IsUnique();

                    b.ToTable("StudyPlansCourses");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCourseCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AllowedCreditHours")
                        .HasColumnType("integer");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("StudyPlanId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("StudyPlanId", "CategoryId")
                        .IsUnique();

                    b.ToTable("StudyPlansCoursesCategories");

                    b.HasCheckConstraint("CK_STUDYPLANCOURSECATEGORY_ALLOWEDCREDITHOURS", "\"AllowedCreditHours\" > 0");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCoursePrerequisite", b =>
                {
                    b.Property<int>("PrerequisiteId")
                        .HasColumnType("integer");

                    b.Property<int>("CourseId")
                        .HasColumnType("integer");

                    b.HasKey("PrerequisiteId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("StudyPlansCoursesPrerequisites");
                });

            modelBuilder.Entity("GradProjectServer.Services.UserSystem.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StudyPlanId")
                        .HasColumnType("integer");

                    b.Property<string>("Token")
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("StudyPlanId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.BlankSubQuestion", b =>
                {
                    b.HasBaseType("GradProjectServer.Services.Exams.Entities.SubQuestion");

                    b.Property<string>("Answer")
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<int?>("CheckerId")
                        .HasColumnType("integer");

                    b.HasIndex("CheckerId");

                    b.ToTable("BlankSubQuestion");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.MCQSubQuestion", b =>
                {
                    b.HasBaseType("GradProjectServer.Services.Exams.Entities.SubQuestion");

                    b.Property<bool>("IsCheckBox")
                        .HasColumnType("boolean");

                    b.ToTable("MCQSubQuestion");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ProgrammingSubQuestion", b =>
                {
                    b.HasBaseType("GradProjectServer.Services.Exams.Entities.SubQuestion");

                    b.Property<int>("CheckerId")
                        .HasColumnType("integer");

                    b.HasIndex("CheckerId");

                    b.ToTable("ProgrammingSubQuestion");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Exam", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.Course", "Course")
                        .WithMany("Exams")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.UserSystem.User", "Volunteer")
                        .WithMany("VolunteeredExams")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Volunteer");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamAttempt", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.Exam", "Exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamQuestion", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.Exam", "Exam")
                        .WithMany("Questions")
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Exams.Entities.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamSubQuestion", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.ExamQuestion", "ExamQuestion")
                        .WithMany("ExamSubQuestions")
                        .HasForeignKey("ExamQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Exams.Entities.SubQuestion", "SubQuestion")
                        .WithMany()
                        .HasForeignKey("SubQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExamQuestion");

                    b.Navigation("SubQuestion");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.MCQSubQuestionChoice", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.MCQSubQuestion", "SubQuestion")
                        .WithMany("Choices")
                        .HasForeignKey("SubQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubQuestion");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Question", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.UserSystem.User", "Volunteer")
                        .WithMany("VolunteeredQuestions")
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Volunteer");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestion", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.Question", "Question")
                        .WithMany("SubQuestions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestionAnswer", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.ExamAttempt", "Attempt")
                        .WithMany()
                        .HasForeignKey("AttemptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Exams.Entities.ExamSubQuestion", "SubQuestion")
                        .WithMany()
                        .HasForeignKey("SubQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attempt");

                    b.Navigation("SubQuestion");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestionTag", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.SubQuestion", "SubQuestion")
                        .WithMany("Tags")
                        .HasForeignKey("SubQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Exams.Entities.Tag", "Tag")
                        .WithMany("SubQuestions")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubQuestion");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.ProgramDependency", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.Dependency", "Dependency")
                        .WithMany()
                        .HasForeignKey("DependecyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Infrastructure.Program", "Program")
                        .WithMany("Dependencies")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dependency");

                    b.Navigation("Program");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlan", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.Major", "Major")
                        .WithMany("StudyPlans")
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Major");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCourse", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.StudyPlanCourseCategory", "Category")
                        .WithMany("Courses")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Infrastructure.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Infrastructure.StudyPlan", null)
                        .WithMany("Courses")
                        .HasForeignKey("StudyPlanId");

                    b.Navigation("Category");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCourseCategory", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.CourseCategory", "Category")
                        .WithMany()
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Infrastructure.StudyPlan", "StudyPlan")
                        .WithMany("Categories")
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("StudyPlan");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCoursePrerequisite", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.StudyPlanCourse", "Course")
                        .WithMany("Prerequisites")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Infrastructure.StudyPlanCourse", "Prerequisite")
                        .WithMany()
                        .HasForeignKey("PrerequisiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Prerequisite");
                });

            modelBuilder.Entity("GradProjectServer.Services.UserSystem.User", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.StudyPlan", "StudyPlan")
                        .WithMany()
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StudyPlan");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.BlankSubQuestion", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.Program", "Checker")
                        .WithMany()
                        .HasForeignKey("CheckerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GradProjectServer.Services.Exams.Entities.SubQuestion", null)
                        .WithOne()
                        .HasForeignKey("GradProjectServer.Services.Exams.Entities.BlankSubQuestion", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Checker");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.MCQSubQuestion", b =>
                {
                    b.HasOne("GradProjectServer.Services.Exams.Entities.SubQuestion", null)
                        .WithOne()
                        .HasForeignKey("GradProjectServer.Services.Exams.Entities.MCQSubQuestion", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ProgrammingSubQuestion", b =>
                {
                    b.HasOne("GradProjectServer.Services.Infrastructure.Program", "Checker")
                        .WithMany()
                        .HasForeignKey("CheckerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GradProjectServer.Services.Exams.Entities.SubQuestion", null)
                        .WithOne()
                        .HasForeignKey("GradProjectServer.Services.Exams.Entities.ProgrammingSubQuestion", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Checker");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Exam", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.ExamQuestion", b =>
                {
                    b.Navigation("ExamSubQuestions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Question", b =>
                {
                    b.Navigation("SubQuestions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.SubQuestion", b =>
                {
                    b.Navigation("Tags");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.Tag", b =>
                {
                    b.Navigation("SubQuestions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Course", b =>
                {
                    b.Navigation("Exams");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Major", b =>
                {
                    b.Navigation("StudyPlans");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.Program", b =>
                {
                    b.Navigation("Dependencies");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlan", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Courses");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCourse", b =>
                {
                    b.Navigation("Prerequisites");
                });

            modelBuilder.Entity("GradProjectServer.Services.Infrastructure.StudyPlanCourseCategory", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("GradProjectServer.Services.UserSystem.User", b =>
                {
                    b.Navigation("VolunteeredExams");

                    b.Navigation("VolunteeredQuestions");
                });

            modelBuilder.Entity("GradProjectServer.Services.Exams.Entities.MCQSubQuestion", b =>
                {
                    b.Navigation("Choices");
                });
#pragma warning restore 612, 618
        }
    }
}
