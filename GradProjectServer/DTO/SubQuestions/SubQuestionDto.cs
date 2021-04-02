using GradProjectServer.DTO.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class SubQuestionDto : SubQuestionMetadataDto
    {
        public string Content { get; set; }
        public TagDto[] Tags { get; set; }
    }
}
