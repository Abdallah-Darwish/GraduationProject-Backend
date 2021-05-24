using GradProjectServer.Common;

namespace GradProjectServer.Services.CheckersManagers
{
    public class ProgrammingCheckerResult
    {
        public float Grade { get; set; }
        public ProgrammingSubQuestionAnswerVerdict Verdict { get; set; }
        public string? Comment { get; set; }
    }
}