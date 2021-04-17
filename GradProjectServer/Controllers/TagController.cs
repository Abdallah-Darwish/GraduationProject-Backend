using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Tags;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
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
    public class TagController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public TagController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.Tags.Skip(info.Offset).Take(info.Count).Select(t => t.Id));
        }
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<TagDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TagDto>> Get([FromBody] int[] tagsIds)
        {
            var existingTags = _dbContext.Tags.Where(c => tagsIds.Contains(c.Id));
            var nonExistingTags = tagsIds.Except(existingTags.Select(c => c.Id)).ToArray();
            if (nonExistingTags.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following tags don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingTags"] = nonExistingTags }
                        });
            }
            return Ok(_mapper.ProjectTo<TagDto>(existingTags));
        }
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto info)
        {
            var newTag = new Tag
            {
                Name = info.Name,
            };
            await _dbContext.Tags.AddAsync(newTag).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { tagsIds = new int[] { newTag.Id } }, newTag);
        }
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateTagDto update)
        {
            var tag = await _dbContext.Tags.FindAsync(update.Id).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(update.Name))
            {
                tag.Name = update.Name;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] tagsIds)
        {
            var existingTags = _dbContext.Tags.Where(c => tagsIds.Contains(c.Id));
            var nonExistingTags = tagsIds.Except(existingTags.Select(c => c.Id)).ToArray();
            if (nonExistingTags.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following tags don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingTags"] = nonExistingTags }
                        });
            }
            _dbContext.Tags.RemoveRange(existingTags);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<TagDto>), StatusCodes.Status200OK)]
        public ActionResult<TagDto[]> Search([FromBody] TagSearchFilterDto filter)
        {
            var tags = _dbContext.Tags.AsQueryable();
            if (filter.NameMask != null)
            {
                tags = tags.Where(t => EF.Functions.Like(t.Name, filter.NameMask));
            }
            var result = tags.OrderBy(t => t.Name).Skip(filter.Offset).Take(filter.Count);
            return Ok(_mapper.ProjectTo<TagDto>(tags));
        }
    }
}
