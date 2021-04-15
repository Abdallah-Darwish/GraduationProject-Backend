using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    //todo: Configure
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public string? Token { get; set; }
        public string PasswordHash { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
        public ICollection<Exam> VolunteeredExams { get; set; }
        public ICollection<Question> VolunteeredQuestions { get; set; }
        //todo: can volunteer
        //todo: points
    }
}
