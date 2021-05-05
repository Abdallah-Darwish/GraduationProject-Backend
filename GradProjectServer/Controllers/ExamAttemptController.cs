using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.ExamAttempts;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Controllers
{
    //todo: delete finished attempts after sometime
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
        private IQueryable<ExamAttempt> GetPreparedQueryable()
        {
            IQueryable<ExamAttempt> q = _dbContext.ExamsAttempts
                .Include(e => e.Exam)
                .Include(e => e.Owner);

            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamAttemptController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Ids of all exam attempts in data base ordered by exam attempt start time.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        [AdminFilter]
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.ExamsAttempts
                .OrderBy(c => c.StartTime)
                .Skip(info.Offset)
                .Take(info.Count)
                .Select(c => c.Id));
        }

        ///<remarks>
        /// Admin only.
        /// Normal users can use GetCurrent method.
        /// </remarks>
        /// <param name="examAttemptsIds">Ids of the courses to get.</param>
        /// <response code="404">Ids of the non existing courses.</response>
        [AdminFilter]
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<ExamAttemptDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<ExamAttemptDto>> Get([FromBody] int[] examAttemptsIds)
        {
            var examAttempts = GetPreparedQueryable();
            var existingExamAttempts = examAttempts.Where(c => examAttemptsIds.Contains(c.Id));
            var nonExistingExamAttempts = examAttemptsIds.Except(existingExamAttempts.Select(c => c.Id)).ToArray();
            if (nonExistingExamAttempts.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exam attempts don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingExamAttempts"] = nonExistingExamAttempts}
                    });
            }

            return Ok(_mapper.ProjectTo<IEnumerable<ExamAttemptDto>>(existingExamAttempts));
        }

        ///<remarks>
        /// Returns the user currently active exam attempt.
        /// It might be over but still saved for some time for the user to grade it.
        /// </remarks>
        /// <response code="200">The user has an active attempt.</response>
        /// <response code="204">The user has NO active attempts.</response>
        [LoggedInFilter]
        [HttpGet("GetActive")]
        [ProducesResponseType(typeof(ExamAttemptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExamAttemptDto), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetActive()
        {
            var user = this.GetUser()!;
            var examAttempts = GetPreparedQueryable();
            var attempt = await examAttempts.FirstOrDefaultAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            if (attempt == null)
            {
                return NoContent();
            }

            return Ok(_mapper.Map<ExamAttemptDto>(attempt));
        }
        [NonAction]
        public async Task<IActionResult> Create([FromQuery] int examId)
        {
            var user = this.GetUser()!;
            var exam = await _dbContext.Exams.FindAsync(examId).ConfigureAwait(false);
            if (exam == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "There is no exam with the specified id.",
                        Data = new() {["ExamId"] = examId}
                    });
            }

            var activeAttempt = await _dbContext.ExamsAttempts.FirstOrDefaultAsync(e => e.OwnerId == user.Id)
                .ConfigureAwait(false);
            if (activeAttempt != null)
            {
                if (activeAttempt.IsOver)
                {
                    _dbContext.ExamsAttempts.Remove(activeAttempt);
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "Please finish your currently active attempt first.",
                            Data = new() {["ActiveAttemptId"] = activeAttempt.Id}
                        });
                }
            }

            ExamAttempt newAttempt = new()
            {
                ExamId = examId,
                OwnerId = user.Id,
                StartTime = DateTimeOffset.Now
            };
            await _dbContext.ExamsAttempts.AddAsync(newAttempt).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var newAttemptEntity =
                await GetPreparedQueryable().FirstAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created, _mapper.Map<ExamAttemptDto>(newAttemptEntity));
        }
    }
}