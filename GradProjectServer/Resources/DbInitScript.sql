﻿CREATE TABLE "Courses" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "CreditHours" integer NOT NULL,
    CONSTRAINT "PK_Courses" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_COURSE_CREDITHOURS" CHECK ("CreditHours" >= 0)
);

CREATE TABLE "CoursesCategories" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_CoursesCategories" PRIMARY KEY ("Id")
);

CREATE TABLE "Dependencies" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Dependencies" PRIMARY KEY ("Id")
);

CREATE TABLE "Majors" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Majors" PRIMARY KEY ("Id")
);

CREATE TABLE "Programs" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "FileName" text NOT NULL,
    CONSTRAINT "PK_Programs" PRIMARY KEY ("Id")
);

CREATE TABLE "Tags" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Tags" PRIMARY KEY ("Id")
);

CREATE TABLE "StudyPlans" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Year" integer NOT NULL,
    "MajorId" integer NOT NULL,
    CONSTRAINT "PK_StudyPlans" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_STUDYPLAN_YEAR" CHECK ("Year" > 0),
    CONSTRAINT "FK_StudyPlans_Majors_MajorId" FOREIGN KEY ("MajorId") REFERENCES "Majors" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProgramsDependencies" (
    "ProgramId" integer NOT NULL,
    "DependecyId" integer NOT NULL,
    CONSTRAINT "PK_ProgramsDependencies" PRIMARY KEY ("ProgramId", "DependecyId"),
    CONSTRAINT "FK_ProgramsDependencies_Dependencies_DependecyId" FOREIGN KEY ("DependecyId") REFERENCES "Dependencies" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProgramsDependencies_Programs_ProgramId" FOREIGN KEY ("ProgramId") REFERENCES "Programs" ("Id") ON DELETE CASCADE
);

CREATE TABLE "StudyPlansCoursesCategories" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "CategoryId" integer NOT NULL,
    "AllowedCreditHours" integer NOT NULL,
    "StudyPlanId" integer NOT NULL,
    CONSTRAINT "PK_StudyPlansCoursesCategories" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_STUDYPLANCOURSECATEGORY_ALLOWEDCREDITHOURS" CHECK ("AllowedCreditHours" > 0),
    CONSTRAINT "FK_StudyPlansCoursesCategories_CoursesCategories_StudyPlanId" FOREIGN KEY ("StudyPlanId") REFERENCES "CoursesCategories" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StudyPlansCoursesCategories_StudyPlans_StudyPlanId" FOREIGN KEY ("StudyPlanId") REFERENCES "StudyPlans" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Users" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Email" text NOT NULL,
    "Name" text NOT NULL,
    "IsAdmin" boolean NOT NULL,
    "Token" text NULL,
    "PasswordHash" text NOT NULL,
    "StudyPlanId" integer NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Users_StudyPlans_StudyPlanId" FOREIGN KEY ("StudyPlanId") REFERENCES "StudyPlans" ("Id") ON DELETE CASCADE
);

CREATE TABLE "StudyPlansCourses" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "CourseId" integer NOT NULL,
    "CategoryId" integer NOT NULL,
    "StudyPlanId" integer NULL,
    CONSTRAINT "PK_StudyPlansCourses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StudyPlansCourses_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StudyPlansCourses_StudyPlans_StudyPlanId" FOREIGN KEY ("StudyPlanId") REFERENCES "StudyPlans" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_StudyPlansCourses_StudyPlansCoursesCategories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "StudyPlansCoursesCategories" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Exams" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Year" integer NOT NULL,
    "IsApproved" boolean NOT NULL DEFAULT FALSE,
    "Type" smallint NOT NULL,
    "Semester" smallint NOT NULL,
    "Name" text NOT NULL,
    "Duration" bigint NOT NULL,
    "CourseId" integer NOT NULL,
    "VolunteerId" integer NOT NULL,
    CONSTRAINT "PK_Exams" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_EXAM_YEAR" CHECK ("Year" >= 1),
    CONSTRAINT "CK_EXAM_DURATION" CHECK ("Duration" > 0),
    CONSTRAINT "FK_Exams_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Exams_Users_VolunteerId" FOREIGN KEY ("VolunteerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Question" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Content" text NOT NULL,
    "IsApproved" boolean NOT NULL DEFAULT FALSE,
    "Title" text NOT NULL,
    "CourseId" integer NOT NULL,
    "VolunteerId" integer NOT NULL,
    CONSTRAINT "PK_Question" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Question_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Question_Users_VolunteerId" FOREIGN KEY ("VolunteerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "StudyPlansCoursesPrerequisites" (
    "PrerequisiteId" integer NOT NULL,
    "CourseId" integer NOT NULL,
    CONSTRAINT "PK_StudyPlansCoursesPrerequisites" PRIMARY KEY ("PrerequisiteId", "CourseId"),
    CONSTRAINT "FK_StudyPlansCoursesPrerequisites_StudyPlansCourses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "StudyPlansCourses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StudyPlansCoursesPrerequisites_StudyPlansCourses_Prerequisi~" FOREIGN KEY ("PrerequisiteId") REFERENCES "StudyPlansCourses" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ExamsAttempts" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "ExamId" integer NOT NULL,
    "StartTime" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ExamsAttempts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ExamsAttempts_Exams_ExamId" FOREIGN KEY ("ExamId") REFERENCES "Exams" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ExamsQuestions" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Order" integer NOT NULL,
    "QuestionId" integer NOT NULL,
    "ExamId" integer NOT NULL,
    CONSTRAINT "PK_ExamsQuestions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ExamsQuestions_Exams_ExamId" FOREIGN KEY ("ExamId") REFERENCES "Exams" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ExamsQuestions_Question_QuestionId" FOREIGN KEY ("QuestionId") REFERENCES "Question" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SubQuestions" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Content" text NOT NULL,
    "Type" smallint NOT NULL,
    "QuestionId" integer NOT NULL,
    CONSTRAINT "PK_SubQuestions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_SubQuestions_Question_QuestionId" FOREIGN KEY ("QuestionId") REFERENCES "Question" ("Id") ON DELETE CASCADE
);

CREATE TABLE "BlankSubQuestion" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "CheckerId" integer NULL,
    "Answer" text NULL,
    CONSTRAINT "PK_BlankSubQuestion" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_BlankSubQuestion_Programs_CheckerId" FOREIGN KEY ("CheckerId") REFERENCES "Programs" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BlankSubQuestion_SubQuestions_Id" FOREIGN KEY ("Id") REFERENCES "SubQuestions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ExamSubQuestions" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "SubQuestionId" integer NOT NULL,
    "Weight" real NOT NULL,
    "ExamQuestionId" integer NOT NULL,
    "Order" integer NOT NULL,
    CONSTRAINT "PK_ExamSubQuestions" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_EXAMSUBQUESTION_WEIGHT" CHECK ("Weight" > 0),
    CONSTRAINT "CK_EXAMSUBQUESTION_ORDER" CHECK ("Order" >= 0),
    CONSTRAINT "FK_ExamSubQuestions_ExamsQuestions_ExamQuestionId" FOREIGN KEY ("ExamQuestionId") REFERENCES "ExamsQuestions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ExamSubQuestions_SubQuestions_SubQuestionId" FOREIGN KEY ("SubQuestionId") REFERENCES "SubQuestions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MCQSubQuestion" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "IsCheckBox" boolean NOT NULL,
    CONSTRAINT "PK_MCQSubQuestion" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MCQSubQuestion_SubQuestions_Id" FOREIGN KEY ("Id") REFERENCES "SubQuestions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProgrammingSubQuestion" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "CheckerId" integer NOT NULL,
    CONSTRAINT "PK_ProgrammingSubQuestion" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProgrammingSubQuestion_Programs_CheckerId" FOREIGN KEY ("CheckerId") REFERENCES "Programs" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProgrammingSubQuestion_SubQuestions_Id" FOREIGN KEY ("Id") REFERENCES "SubQuestions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SubQuestionsTags" (
    "SubQuestionId" integer NOT NULL,
    "TagId" integer NOT NULL,
    CONSTRAINT "PK_SubQuestionsTags" PRIMARY KEY ("SubQuestionId", "TagId"),
    CONSTRAINT "FK_SubQuestionsTags_SubQuestions_SubQuestionId" FOREIGN KEY ("SubQuestionId") REFERENCES "SubQuestions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SubQuestionsTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SubQuestionsAnswers" (
    "AttemptId" integer NOT NULL,
    "SubQuestionId" integer NOT NULL,
    "Answer" text NULL,
    CONSTRAINT "PK_SubQuestionsAnswers" PRIMARY KEY ("AttemptId", "SubQuestionId"),
    CONSTRAINT "FK_SubQuestionsAnswers_ExamsAttempts_AttemptId" FOREIGN KEY ("AttemptId") REFERENCES "ExamsAttempts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SubQuestionsAnswers_ExamSubQuestions_SubQuestionId" FOREIGN KEY ("SubQuestionId") REFERENCES "ExamSubQuestions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MCQSubQuestionsChoices" (
    "Id" integer NOT NULL,
    "SubQuestionId" integer NOT NULL,
    "Content" text NOT NULL,
    "Weight" real NOT NULL,
    CONSTRAINT "PK_MCQSubQuestionsChoices" PRIMARY KEY ("Id", "SubQuestionId"),
    CONSTRAINT "CK_MCQSubQuestionChoice_WEIGHT" CHECK ("Weight" >= -1 AND "Weight" <= 1),
    CONSTRAINT "FK_MCQSubQuestionsChoices_MCQSubQuestion_SubQuestionId" FOREIGN KEY ("SubQuestionId") REFERENCES "MCQSubQuestion" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_BlankSubQuestion_CheckerId" ON "BlankSubQuestion" ("CheckerId");

CREATE INDEX "IX_Exams_CourseId" ON "Exams" ("CourseId");

CREATE INDEX "IX_Exams_VolunteerId" ON "Exams" ("VolunteerId");

CREATE INDEX "IX_ExamsAttempts_ExamId" ON "ExamsAttempts" ("ExamId");

CREATE INDEX "IX_ExamsQuestions_ExamId" ON "ExamsQuestions" ("ExamId");

CREATE INDEX "IX_ExamsQuestions_QuestionId" ON "ExamsQuestions" ("QuestionId");

CREATE INDEX "IX_ExamSubQuestions_ExamQuestionId" ON "ExamSubQuestions" ("ExamQuestionId");

CREATE INDEX "IX_ExamSubQuestions_SubQuestionId" ON "ExamSubQuestions" ("SubQuestionId");

CREATE INDEX "IX_MCQSubQuestionsChoices_SubQuestionId" ON "MCQSubQuestionsChoices" ("SubQuestionId");

CREATE INDEX "IX_ProgrammingSubQuestion_CheckerId" ON "ProgrammingSubQuestion" ("CheckerId");

CREATE INDEX "IX_ProgramsDependencies_DependecyId" ON "ProgramsDependencies" ("DependecyId");

CREATE INDEX "IX_Question_CourseId" ON "Question" ("CourseId");

CREATE INDEX "IX_Question_VolunteerId" ON "Question" ("VolunteerId");

CREATE UNIQUE INDEX "IX_StudyPlans_MajorId_Year" ON "StudyPlans" ("MajorId", "Year");

CREATE INDEX "IX_StudyPlansCourses_CategoryId" ON "StudyPlansCourses" ("CategoryId");

CREATE UNIQUE INDEX "IX_StudyPlansCourses_CourseId_CategoryId" ON "StudyPlansCourses" ("CourseId", "CategoryId");

CREATE INDEX "IX_StudyPlansCourses_StudyPlanId" ON "StudyPlansCourses" ("StudyPlanId");

CREATE UNIQUE INDEX "IX_StudyPlansCoursesCategories_StudyPlanId_CategoryId" ON "StudyPlansCoursesCategories" ("StudyPlanId", "CategoryId");

CREATE INDEX "IX_StudyPlansCoursesPrerequisites_CourseId" ON "StudyPlansCoursesPrerequisites" ("CourseId");

CREATE INDEX "IX_SubQuestions_QuestionId" ON "SubQuestions" ("QuestionId");

CREATE INDEX "IX_SubQuestionsAnswers_SubQuestionId" ON "SubQuestionsAnswers" ("SubQuestionId");

CREATE INDEX "IX_SubQuestionsTags_TagId" ON "SubQuestionsTags" ("TagId");

CREATE INDEX "IX_Users_StudyPlanId" ON "Users" ("StudyPlanId");