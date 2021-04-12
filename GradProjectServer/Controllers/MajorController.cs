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
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public MajorController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IActionResult Get([FromBody] int[] majorsIds, [FromQuery] bool metadata = false)
        {
            var existingMajors = _dbContext.Majors.Where(c => majorsIds.Contains(c.Id));
            var nonExistingMajors = majorsIds.Except(existingMajors.Select(c => c.Id)).ToArray();
            if (nonExistingMajors.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following majors don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingMajors"] = nonExistingMajors }
                        });
            }
            if (metadata)
            {
                return Ok(_mapper.ProjectTo<MajorMetadataDto>(existingMajors));
            }
            return Ok(_mapper.ProjectTo<MajorDto>(existingMajors));
        }
        [HttpDelete]
        [AdminFilter]
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
                            Data = new Dictionary<string, object> { ["NonExistingMajors"] = nonExistingMajors }
                        });
            }
            _dbContext.Majors.RemoveRange(existingMajors);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpPost]
        [AdminFilter]
        public async Task<ActionResult<MajorDto>> Create(CreateMajorDto info)
        {
            
            var newMajor = new Major
            {
                Name = info.Name,
            };
            await _dbContext.Majors.AddAsync(newMajor).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { majorsIds = new int[] { newMajor.Id }, metadata = false }, newMajor);
        }
        [HttpPatch]
        [AdminFilter]
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
        public IActionResult Search([FromBody] MajorSearchFilterDto filter)
        {
            var majors = _dbContext.Majors.AsQueryable();
            if(filter.NameMask != null)
            {
                majors = majors.Where(m => EF.Functions.Like(m.Name, filter.NameMask));
            }
            majors = majors.OrderBy(m => m.Name).Skip(filter.Offset).Take(filter.Count);
            return Ok(_mapper.ProjectTo<MajorDto>(majors));
        }
    }
}
