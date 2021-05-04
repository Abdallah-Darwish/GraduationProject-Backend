namespace GradProjectServer.DTO.SubQuestions
{
    public class OwnedMCQSubQuestionChoiceDto : MCQSubQuestionChoiceDto
    {
        /// <summary>
        /// Range [-1, 1]
        /// Where negative means its a wrong answer, it will be considered only if <see cref="MCQSubQuestionDto.IsCheckBox"/> is true.
        /// If <see cref="MCQSubQuestionDto.IsCheckBox"/> is false only one <see cref="Weight"/> can be > 0.
        /// </summary>
        public float Weight { get; set; }
    }
}