namespace GradProjectServer.DTO.ExamAttempts.SubQuestionGrade
{
    public enum ProgrammingSubQuestionAnswerVerdict
    {
        /// <summary>
        /// The checker TLEed or made a runtime exception
        /// </summary>
        CheckerError,
        CompilationError,
        RuntimeError,
        TimeLimitExceeded,
        WrongAnswer,
        //Shoutout to Codeforces
        Accepted,
        PartialAccepted
    }
}