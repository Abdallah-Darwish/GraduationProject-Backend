﻿namespace GradProjectServer.DTO.ExamSubQuestions
{
    //todo: validate exam is not approved or is admin
    //todo: validate exam is owned by user or is admin
    public class CreateExamSubQuestionDto
    {
        public int ExamQuestionId { get; set; }
        public int SubQuestionId { get; set; }
        public float Weight { get; set; }
        public int Order { get; set; }
    }
}
