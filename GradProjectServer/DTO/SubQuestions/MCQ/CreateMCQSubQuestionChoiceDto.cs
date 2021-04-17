namespace GradProjectServer.DTO.SubQuestions
{
    //todo: if mcq is checkbox validate only one choice have weight > 0
    public class CreateMCQSubQuestionChoiceDto
    {
        public string Content { get; set; }
        /// <summary>
        /// [-1, 1]
        /// </summary>
        public float Weight { get; set; }
    }
}
