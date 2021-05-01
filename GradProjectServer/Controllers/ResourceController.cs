using System;
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
using System.IO;
using GradProjectServer.Services.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        public static string ResourcesDirectory { get; private set; }

        public static string GetResourceFilePath(Resource res) =>
            Path.Combine(ResourcesDirectory, $"{res.Id}_{res.FileExtension}");

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            ResourcesDirectory = Path.Combine(appOptions.DataSaveDirectory, "Resources");
            if (!Directory.Exists(ResourcesDirectory))
            {
                Directory.CreateDirectory(ResourcesDirectory);
            }
        }

        private IQueryable<Resource> GetPreparedQueryable()
        {
            var q = _dbContext.Resources
                .Include(r => r.Course)
                .Include(r => r.Volunteer);
            return q;
        }

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
        /// <response code="403">Ids of the resources has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ResourceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public IActionResult Get([FromBody] int[] resourcesIds)
        {
            var resources = GetPreparedQueryable();
            var existingResources =  resources.Where(r => resourcesIds.Contains(r.Id));
            var nonExistingResources = resourcesIds.Except(existingResources.Select(r => r.Id)).ToArray();
            if (nonExistingResources.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following resources don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingResources"] = nonExistingResources}
                    });
            }

            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                var notOwnedResources =
                    existingResources.Where(r => r.VolunteerId != userId && !r.IsApproved).ToArray();
                if (notOwnedResources.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved resources.",
                            Data = new Dictionary<string, object> {["NotOwnedNotApprovedResources"] = notOwnedResources}
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
            var resources = GetPreparedQueryable();
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
            if ((filter.Extesnions?.Length ?? 0) > 0)
            {
                resources = resources.Where(e => filter.Extesnions!.Contains(e.FileExtension));
            }

            if ((filter.Types?.Length ?? 0) > 0)
            {
                resources = resources.Where(e => filter.Types!.Contains(e.Type));
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

        /// <summary>Creates a new resource.</summary>
        /// <response code="201">The newly created resource.</response>
        [LoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResourceDto>> Create([FromBody] CreateResourceDto data)
        {
            var user = this.GetUser()!;
            var resource = new Resource
            {
                Name = data.Name,
                CourseId = data.CourseId,
                VolunteerId = user.Id,
                CreationSemester = data.CreationSemester,
                CreationYear = data.CreationYear,
                FileExtension = data.FileExtension,
                IsApproved = false,
                Type =  data.Type
            };
            await _dbContext.Resources.AddAsync(resource).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            await using var resourceFileStream = new FileStream(GetResourceFilePath(resource), FileMode.Create,
                FileAccess.ReadWrite, FileShare.Read);
            await resourceFileStream.WriteAsync(Convert.FromBase64String(data.FileBase64)).ConfigureAwait(false);
            await resourceFileStream.FlushAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {resourcesIds = new int[] {resource.Id}}, _mapper.Map<ResourceDto>(resource));
        }
        /// <summary>
        /// Updates an resource.
        /// </summary>
        /// <remarks>
        /// A user can update:
        ///     HIS NOT approved resources.
        /// An admin can update:
        ///     All resources.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [LoggedInFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateResourceDto update)
        {
            var resource = await _dbContext.Resources.FindAsync(update.Id).ConfigureAwait(false);
            if (update.Name != null)
            {
                resource.Name = update.Name;
            }
            if (update.CreationSemester != null)
            {
                resource.CreationSemester = update.CreationSemester.Value;
            }
            if (update.CreationYear != null)
            {
                resource.CreationYear = update.CreationYear.Value;
            }
            if (update.IsApproved != null)
            {
                resource.IsApproved = update.IsApproved.Value;
            }
            if (update.FileExtension != null)
            {
                resource.FileExtension = update.FileExtension;
            }

            if (update.Type != null)
            {
                resource.Type = update.Type.Value;
            }
            if (update.FileBase64 != null)
            {
                await using var resourceFileStream = new FileStream(GetResourceFilePath(resource), FileMode.Open,
                    FileAccess.ReadWrite, FileShare.Read);
                await resourceFileStream.WriteAsync(Convert.FromBase64String(update.FileBase64)).ConfigureAwait(false);
                resourceFileStream.SetLength(resourceFileStream.Position);
                await resourceFileStream.FlushAsync().ConfigureAwait(false);
            }
            return Ok();
        }
        /// <remarks>
        /// Gets the resource file as stream of bytes with header Content-Type: application/octet-stream.
        /// A user can get file of:
        ///     1- All approved resources.
        ///     2- HIS not approved resources.
        /// An admin can get files of:
        ///     All resources.
        /// </remarks>
        /// <param name="resourceId">Id of the resource to get its file.</param>
        /// <response code="404">If there is no resource with this id.</response>
        /// <response code="403">If user has no access rights to this resource.</response>
        [HttpGet("GetResourceFile")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetResourceFile([FromQuery] int resourceId)
        {
            var resource = await _dbContext.Resources.FindAsync(resourceId).ConfigureAwait(false);
            if (resource == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "There is no resource with the specified Id.",
                        Data = new Dictionary<string, object> {["ResourceId"] = resourceId}
                    });
            }

            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false) && resource.IsApproved == false && resource.VolunteerId != (user?.Id ?? -1))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "User doesn't own this not approved resource.",
                        Data = new Dictionary<string, object> {["NotOwnedNotApprovedResource"] = resourceId}
                    });
            }

            var resourceFile = new FileStream(GetResourceFilePath(resource), FileMode.Open, FileAccess.ReadWrite,
                FileShare.Read);
            var result = File(resourceFile, "application/octet-stream");
            result.FileDownloadName = $"Resource_{resourceId}.{resource.FileExtension}";
            return result;
        }
    }
}