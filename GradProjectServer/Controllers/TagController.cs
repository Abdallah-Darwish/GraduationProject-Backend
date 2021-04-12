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
        [HttpGet]
        public ActionResult<IEnumerable<TagDto>> Get(int[] tagsIds)
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
        [HttpPost]
        [AdminFilter]
        public async Task<ActionResult<TagDto>> Create(CreateTagDto info)
        {
            var newTag = new Tag
            {
                Name = info.Name,
            };
            await _dbContext.Tags.AddAsync(newTag).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { tagsIds = new int[] { newTag.Id } }, newTag);
        }
        [HttpPatch]
        [AdminFilter]
        public async Task<IActionResult> Update(UpdateTagDto update)
        {
            var tag = await _dbContext.Tags.FindAsync(update.Id).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(update.Name))
            {
                tag.Name = update.Name;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpDelete]
        [AdminFilter]
        public async Task<IActionResult> Delete(int[] tagsIds)
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
        [HttpGet]
        public ActionResult<TagDto[]> Search(TagSearchFilterDto filter)
        {
            var tags = _dbContext.Tags.AsQueryable();
            if (filter.NameMask != null)
            {
                tags = tags.Where(t => EF.Functions.Like(t.Name, filter.NameMask));
            }
            var result = tags.OrderBy(t => t.Name).Skip(filter.Offset).Take(filter.Count);
            return Ok( _mapper.ProjectTo<TagDto>(tags));
        }
    }
}
