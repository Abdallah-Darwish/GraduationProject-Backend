using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GradProjectServer.Migrations
{
    public partial class IntialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreditHours = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.CheckConstraint("CK_COURSE_CREDITHOURS", "\"CreditHours\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "CoursesCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    MajorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.Id);
                    table.CheckConstraint("CK_STUDYPLAN_YEAR", "\"Year\" > 0");
                    table.ForeignKey(
                        name: "FK_StudyPlans_Majors_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Majors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlansCoursesCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    AllowedCreditHours = table.Column<int>(type: "integer", nullable: false),
                    StudyPlanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlansCoursesCategories", x => x.Id);
                    table.CheckConstraint("CK_STUDYPLANCOURSECATEGORY_ALLOWEDCREDITHOURS", "\"AllowedCreditHours\" > 0");
                    table.ForeignKey(
                        name: "FK_StudyPlansCoursesCategories_CoursesCategories_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalTable: "CoursesCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlansCoursesCategories_StudyPlans_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    StudyPlanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_StudyPlans_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlansCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    StudyPlanId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlansCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPlansCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlansCourses_StudyPlans_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyPlansCourses_StudyPlansCoursesCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StudyPlansCoursesCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Semester = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    VolunteerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.CheckConstraint("CK_EXAM_YEAR", "\"Year\" >= 1");
                    table.CheckConstraint("CK_EXAM_DURATION", "\"Duration\" > 0");
                    table.ForeignKey(
                        name: "FK_Exams_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exams_Users_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    VolunteerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Question_Users_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    CreationYear = table.Column<int>(type: "integer", nullable: false),
                    CreationSemester = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    VolunteerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.CheckConstraint("CK_RESOURCE_CREATIONYEAR", "\"CreationYear\" >= 1900");
                    table.ForeignKey(
                        name: "FK_Resources_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resources_Users_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlansCoursesPrerequisites",
                columns: table => new
                {
                    PrerequisiteId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlansCoursesPrerequisites", x => new { x.PrerequisiteId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_StudyPlansCoursesPrerequisites_StudyPlansCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "StudyPlansCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlansCoursesPrerequisites_StudyPlansCourses_Prerequisi~",
                        column: x => x.PrerequisiteId,
                        principalTable: "StudyPlansCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamsAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamsAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamsAttempts_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamsAttempts_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamsQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    ExamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamsQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamsQuestions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamsQuestions_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubQuestions_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlankSubQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HasChecker = table.Column<bool>(type: "boolean", nullable: false),
                    IsCheckerBuilt = table.Column<bool>(type: "boolean", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlankSubQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlankSubQuestion_SubQuestions_Id",
                        column: x => x.Id,
                        principalTable: "SubQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamSubQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubQuestionId = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    ExamQuestionId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSubQuestions", x => x.Id);
                    table.CheckConstraint("CK_EXAMSUBQUESTION_WEIGHT", "\"Weight\" > 0");
                    table.CheckConstraint("CK_EXAMSUBQUESTION_ORDER", "\"Order\" >= 0");
                    table.ForeignKey(
                        name: "FK_ExamSubQuestions_ExamsQuestions_ExamQuestionId",
                        column: x => x.ExamQuestionId,
                        principalTable: "ExamsQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamSubQuestions_SubQuestions_SubQuestionId",
                        column: x => x.SubQuestionId,
                        principalTable: "SubQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MCQSubQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsCheckBox = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCQSubQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MCQSubQuestion_SubQuestions_Id",
                        column: x => x.Id,
                        principalTable: "SubQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammingSubQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyAnswerFileExtension = table.Column<string>(type: "text", nullable: false),
                    IsCheckerBuilt = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingSubQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammingSubQuestion_SubQuestions_Id",
                        column: x => x.Id,
                        principalTable: "SubQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubQuestionsTags",
                columns: table => new
                {
                    SubQuestionId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubQuestionsTags", x => new { x.SubQuestionId, x.TagId });
                    table.ForeignKey(
                        name: "FK_SubQuestionsTags_SubQuestions_SubQuestionId",
                        column: x => x.SubQuestionId,
                        principalTable: "SubQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubQuestionsTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttemptId = table.Column<int>(type: "integer", nullable: false),
                    ExamSubQuestionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubQuestionAnswer_ExamsAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "ExamsAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubQuestionAnswer_ExamSubQuestions_ExamSubQuestionId",
                        column: x => x.ExamSubQuestionId,
                        principalTable: "ExamSubQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MCQSubQuestionsChoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubQuestionId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCQSubQuestionsChoices", x => x.Id);
                    table.CheckConstraint("CK_MCQSubQuestionChoice_WEIGHT", "\"Weight\" >= -1 AND \"Weight\" <= 1");
                    table.ForeignKey(
                        name: "FK_MCQSubQuestionsChoices_MCQSubQuestion_SubQuestionId",
                        column: x => x.SubQuestionId,
                        principalTable: "MCQSubQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlankSubQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Answer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlankSubQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlankSubQuestionAnswer_SubQuestionAnswer_Id",
                        column: x => x.Id,
                        principalTable: "SubQuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammingSubQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    ProgrammingLanguage = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingSubQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammingSubQuestionAnswer_SubQuestionAnswer_Id",
                        column: x => x.Id,
                        principalTable: "SubQuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MCQSubQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChoiceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCQSubQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MCQSubQuestionAnswer_MCQSubQuestionsChoices_ChoiceId",
                        column: x => x.ChoiceId,
                        principalTable: "MCQSubQuestionsChoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MCQSubQuestionAnswer_SubQuestionAnswer_Id",
                        column: x => x.Id,
                        principalTable: "SubQuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseId",
                table: "Exams",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_VolunteerId",
                table: "Exams",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamsAttempts_ExamId",
                table: "ExamsAttempts",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamsAttempts_OwnerId",
                table: "ExamsAttempts",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamsQuestions_ExamId",
                table: "ExamsQuestions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamsQuestions_QuestionId",
                table: "ExamsQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubQuestions_ExamQuestionId",
                table: "ExamSubQuestions",
                column: "ExamQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubQuestions_SubQuestionId",
                table: "ExamSubQuestions",
                column: "SubQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MCQSubQuestionAnswer_ChoiceId",
                table: "MCQSubQuestionAnswer",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_MCQSubQuestionsChoices_SubQuestionId",
                table: "MCQSubQuestionsChoices",
                column: "SubQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CourseId",
                table: "Question",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_VolunteerId",
                table: "Question",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_CourseId",
                table: "Resources",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_VolunteerId",
                table: "Resources",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_MajorId_Year",
                table: "StudyPlans",
                columns: new[] { "MajorId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlansCourses_CategoryId",
                table: "StudyPlansCourses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlansCourses_CourseId_CategoryId",
                table: "StudyPlansCourses",
                columns: new[] { "CourseId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlansCourses_StudyPlanId",
                table: "StudyPlansCourses",
                column: "StudyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlansCoursesCategories_StudyPlanId_CategoryId",
                table: "StudyPlansCoursesCategories",
                columns: new[] { "StudyPlanId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlansCoursesPrerequisites_CourseId",
                table: "StudyPlansCoursesPrerequisites",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuestionAnswer_AttemptId",
                table: "SubQuestionAnswer",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuestionAnswer_ExamSubQuestionId",
                table: "SubQuestionAnswer",
                column: "ExamSubQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuestions_QuestionId",
                table: "SubQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubQuestionsTags_TagId",
                table: "SubQuestionsTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudyPlanId",
                table: "Users",
                column: "StudyPlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlankSubQuestion");

            migrationBuilder.DropTable(
                name: "BlankSubQuestionAnswer");

            migrationBuilder.DropTable(
                name: "MCQSubQuestionAnswer");

            migrationBuilder.DropTable(
                name: "ProgrammingSubQuestion");

            migrationBuilder.DropTable(
                name: "ProgrammingSubQuestionAnswer");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "StudyPlansCoursesPrerequisites");

            migrationBuilder.DropTable(
                name: "SubQuestionsTags");

            migrationBuilder.DropTable(
                name: "MCQSubQuestionsChoices");

            migrationBuilder.DropTable(
                name: "SubQuestionAnswer");

            migrationBuilder.DropTable(
                name: "StudyPlansCourses");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "MCQSubQuestion");

            migrationBuilder.DropTable(
                name: "ExamsAttempts");

            migrationBuilder.DropTable(
                name: "ExamSubQuestions");

            migrationBuilder.DropTable(
                name: "StudyPlansCoursesCategories");

            migrationBuilder.DropTable(
                name: "ExamsQuestions");

            migrationBuilder.DropTable(
                name: "SubQuestions");

            migrationBuilder.DropTable(
                name: "CoursesCategories");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "StudyPlans");

            migrationBuilder.DropTable(
                name: "Majors");
        }
    }
}
