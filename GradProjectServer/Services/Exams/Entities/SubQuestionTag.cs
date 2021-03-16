using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestionTag
    {
        public int SubQustionId { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public SubQuestion SubQuestion { get; set; }
    }
}
