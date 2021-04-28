using AutoMapper;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExamAttemptController : ControllerBase
    {
        /*
         Create: attempt exam, but should check that there is no another attempts for this student.
         Get: to be used by admins only
         GetCurrent: for student in case he disconnected
         Finish: to grade the exam and return feedback
         No need for update
         */
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamAttemptController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}