using System.Threading.Tasks;
using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Exams;
using GradProjectServer.DTO.Resources;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ResourceController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <remarks>
        /// Result is ordered by year descending then semester ascending then name ascending.
        /// A user has access to:
        ///     1- All approved resources.
        ///     2- HIS not approved resources.
        /// An admin has access to:
        ///     All resources.
        /// </remarks>
        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<ResourceDto> GetAll([FromBody] GetAllDto info)
        {
            var user = this.GetUser();
            var resources = _dbContext.Resources.AsQueryable();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                resources = resources.Where(r => r.VolunteerId == userId || r.IsApproved);
            }
            resources = resources
                .OrderByDescending(r => r.CreationYear)
                .ThenBy(r => r.CreationSemester)
                .ThenBy(r => r.Name);
            return Ok(resources.Skip(info.Offset).Take(info.Count).Select(e => e.Id));
        }
        /// <param name="resourcesIds">Ids of the resources to get.</param>
        /// <remarks>
        /// A user can get:
        ///     1- All approved resources.
        ///     2- HIS not approved resources.
        /// An admin can get:
        ///     All resources.
        /// </remarks>
        /// <response code="404">Ids of the non existing resources.</response>
        /// <response code="403">Ids of exams the resources has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ResourceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public IActionResult Get([FromBody] int[] resourcesIds)
        {
            var existingResources = _dbContext.Resources.Where(r => resourcesIds.Contains(r.Id));
            var nonExistingResources = resourcesIds.Except(existingResources.Select(r => r.Id)).ToArray();
            if (nonExistingResources.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following resources don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingResources"] = nonExistingResources }
                        });
            }
            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                var notOwnedResources = existingResources.Where(r => r.VolunteerId != userId && !r.IsApproved).ToArray();
                if (notOwnedResources.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved resources.",
                            Data = new Dictionary<string, object> { ["NotOwnedNotApprovedResources"] = notOwnedResources }
                        });
                }
            }
            return Ok(_mapper.ProjectTo<ResourceDto>(existingResources));
        }
        /// <summary>
        /// Returns resources that satisfy the filters ordered by year descending then semester ascending then name ascending.
        /// </summary>
        /// <param name="filter">The filters to apply, null property means it won't be applied.</param>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<ResourceDto>), StatusCodes.Status200OK)]
        public IActionResult Search([FromBody] ResourceSearchFilterDto filter)
        {
            var resources = _dbContext.Resources.AsQueryable();
            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                resources = resources.Where(r => r.IsApproved || r.VolunteerId == userId);
            }
            if (filter.NameMask != null)
            {
                resources = resources.Where(e => EF.Functions.Like(e.Name, filter.NameMask));
            }
            if ((filter.Courses?.Length ?? 0) > 0)
            {
                resources = resources.Where(e => filter.Courses!.Contains(e.CourseId));
            }
            if ((filter.CreationSemesters?.Length ?? 0) > 0)
            {
                resources = resources.Where(e => filter.CreationSemesters!.Contains(e.CreationSemester));
            }
            if (filter.MinCreationYear != null)
            {
                resources = resources.Where(e => e.CreationYear >= filter.MinCreationYear);
            }
            if (filter.MaxCreationYear != null)
            {
                resources = resources.Where(e => e.CreationYear <= filter.MaxCreationYear);
            }
            if ((filter.Volunteers?.Length ?? 0) > 0)
            {
                resources = resources.Where(e => filter.Volunteers!.Contains(e.VolunteerId));
            }
            
            resources = resources
                .OrderByDescending(r => r.CreationYear)
                .ThenBy(r => r.CreationSemester)
                .ThenBy(r => r.Name);
            var result = resources.Skip(filter.Offset).Take(filter.Count);
           
            return Ok(_mapper.ProjectTo<ExamDto>(result));
        }
        /// <summary>Deletes the specified resources.</summary>
        /// <param name="resourcesIds">Ids of the resources to delete.</param>
        /// <remarks>
        /// A user can delete:
        ///     HIS NOT approved resources.
        /// An admin can delete:
        ///     All resources.
        /// </remarks>
        /// <response code="404">Ids of the non existing resources.</response>
        /// <response code="403">Ids of the resources user can't modify.</response>
        [LoggedInFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([FromBody] int[] resourcesIds)
        {
            var existingResources = _dbContext.Resources.Where(e => resourcesIds.Contains(e.Id));
            var nonExistingResources = resourcesIds.Except(existingResources.Select(e => e.Id)).ToArray();
            if (nonExistingResources.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following resources don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingResources"] = nonExistingResources}
                    });
            }

            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var notOwnedResources = existingResources
                    .Where(e => e.VolunteerId != user.Id)
                    .Select(r => r.Id)
                    .ToArray();
                if (notOwnedResources.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following resources so he can't modify them.",
                            Data = new Dictionary<string, object> {["NotOwnedResources"] = notOwnedResources}
                        });
                }

                var approvedResources = existingResources
                    .Where(e => e.IsApproved)
                    .Select(r => r.Id)
                    .ToArray();
                if (approvedResources.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "The following resources are already approved so the user can't modify them.",
                            Data = new Dictionary<string, object> {["ApprovedResources"] = approvedResources}
                        });
                }
            }

            _dbContext.Resources.RemoveRange(existingResources);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}