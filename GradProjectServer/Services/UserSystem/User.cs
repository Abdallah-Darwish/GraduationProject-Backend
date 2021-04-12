using GradProjectServer.Services.Exams.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public string Token { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<Exam> VolunteeredExams { get; set; }
        public ICollection<Question> VolunteeredQuestions { get; set; }
        //todo: can volunteer
        //todo: points
    }
}
