using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateMCQSubQuestionDto : UpdateSubQuestionDto
    {
        public bool? IsCheckBox { get; set; }
        public CreateMCQSubQuestionChoiceDto[]? ChoicesToAdd { get; set; }
        public int[]? ChoicesToDelete { get; set; }
        public UpdateMCQSubQuestionChoiceDto[]? ChoicesToUpdate { get; set; }
    }
}
