using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Majors;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MajorController : ControllerBase
    {
        private IQueryable<Major> GetPreparedQueryable(bool metadata = false)
        {
            IQueryable<Major> q = _dbContext.Majors;

            if (!metadata)
            {
                q = q
                    .Include(e => e.StudyPlans)
                    .ThenInclude(q => q.Major);
            }

            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public MajorController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <reamarks>Result is ordered by name.</reamarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.Majors.OrderBy(m => m.Name).Skip(info.Offset).Take(info.Count).Select(m => m.Id));
        }

        /// <param name="majorsIds">Ids of the majors to get.</param>
        /// <param name="metadata">Whether to return MajorMetadataDto or MajorDto.</param>
        /// <response code="404">Ids of the non existing majors.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<MajorMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<MajorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public IActionResult Get([FromBody] int[] majorsIds, [FromQuery] bool metadata = false)
        {
            var majors = GetPreparedQueryable(metadata);
            var existingMajors = majors.Where(c => majorsIds.Contains(c.Id));
            var nonExistingMajors = majorsIds.Except(existingMajors.Select(c => c.Id)).ToArray();
            if (nonExistingMajors.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following majors don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingMajors"] = nonExistingMajors}
                    });
            }

            if (metadata)
            {
                return Ok(_mapper.ProjectTo<MajorMetadataDto>(existingMajors));
            }

            return Ok(_mapper.ProjectTo<MajorDto>(existingMajors));
        }

        /// <summary>
        /// Deletes the specified majors.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="majorsIds">Ids of the majors to delete.</param>
        /// <response code="404">Ids of the non existing majors.</response>
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] majorsIds)
        {
            var existingMajors = _dbContext.Majors.Where(c => majorsIds.Contains(c.Id));
            var nonExistingMajors = majorsIds.Except(existingMajors.Select(c => c.Id)).ToArray();
            if (nonExistingMajors.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following majors don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingMajors"] = nonExistingMajors}
                    });
            }

            _dbContext.Majors.RemoveRange(existingMajors);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Creates a new major.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <reponse code="201">Metadata of the newly created major.</reponse>
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(MajorMetadataDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<MajorMetadataDto>> Create(CreateMajorDto info)
        {
            var newMajor = new Major
            {
                Name = info.Name,
            };
            await _dbContext.Majors.AddAsync(newMajor).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {majorsIds = new int[] {newMajor.Id}, metadata = true},
                _mapper.Map<MajorMetadataDto>(newMajor));
        }

        /// <summary>
        /// Updates a major.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateMajorDto update)
        {
            var major = await _dbContext.Majors.FindAsync(update.Id).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(update.Name))
            {
                major.Name = update.Name;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Returns majors that satisfy the filters ordered by name.
        /// </summary>
        /// <param name="filter">The filters to apply, null property means it won't be applied.</param>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<MajorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<MajorMetadataDto>), StatusCodes.Status200OK)]
        public IActionResult Search([FromBody] MajorSearchFilterDto filter)
        {
            var majors = _dbContext.Majors.AsQueryable();
            if (filter.NameMask != null)
            {
                majors = majors.Where(m => EF.Functions.Like(m.Name, filter.NameMask));
            }

            majors = majors.OrderBy(m => m.Name).Skip(filter.Offset).Take(filter.Count);
            if (filter.Metadata)
            {
                return Ok(_mapper.ProjectTo<MajorMetadataDto>(majors));
            }

            return Ok(_mapper.ProjectTo<MajorDto>(majors));
        }
    }
}