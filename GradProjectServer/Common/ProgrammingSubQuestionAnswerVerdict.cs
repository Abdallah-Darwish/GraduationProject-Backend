namespace GradProjectServer.Common
{
    public enum ProgrammingSubQuestionAnswerVerdict
    {
        /// <summary>
        /// The checker TLEed or made a runtime error
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