
using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.EntityFramework;
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
    public class UserController : ControllerBase
    {
        //todo: implement me
        public static string HashPassword(string password) { return password; }
        public static string GenerateToken(User user) { return $"{user.Id}:{DateTime.UtcNow.Ticks}"; }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserProfilePictureRepository _profilePictureRepo;
        public UserController(AppDbContext dbContext, IMapper mapper, IUserProfilePictureRepository profilePictureRepo)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _profilePictureRepo = profilePictureRepo;
        }
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.Users.Skip(info.Offset).Take(info.Count).Select(u => u.Id));
        }
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<UserMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public IActionResult Get([FromBody] int[] usersIds, bool metadata = false)
        {
            var existingUsers = _dbContext.Users.Where(e => usersIds.Contains(e.Id));
            var nonExistingUsers = usersIds.Except(existingUsers.Select(e => e.Id)).ToArray();
            if (nonExistingUsers.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following users don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingUsers"] = nonExistingUsers }
                        });
            }
            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false) && !metadata)
            {
                var userId = user?.Id ?? -1;
                //any requested users that are not the currently logged in user
                var notAllowedToGetUsers = existingUsers.Where(e => e.Id != userId).ToArray();
                if (notAllowedToGetUsers.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User can't get the following users.",
                            Data = new Dictionary<string, object> { ["NotAllowedToGetUsers"] = notAllowedToGetUsers }
                        });
                }
            }
            if (metadata)
            {
                return Ok(_mapper.ProjectTo<UserMetadataDto>(existingUsers));
            }
            return Ok(_mapper.ProjectTo<UserDto>(existingUsers));
        }
        [NotLoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<UserDto>> Create([FromBody] SignUpDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                IsAdmin = false,
                Name = dto.Name,
                PasswordHash = HashPassword(dto.Password),
                Token = null,
                StudyPlanId = dto.StudyPlanId
            };
            await _dbContext.Users.AddAsync(user).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            if (dto.ProfilePictureJpgBase64 != null)
            {
                await _profilePictureRepo.Save(user.Id, dto.ProfilePictureJpgBase64);
            }
            return CreatedAtAction(nameof(Get), new { usersIds = new int[] { user.Id }, metadata = false }, _mapper.Map<UserDto>(user));
        }
        [LoggedInFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto update)
        {
            var user = await _dbContext.Users.FindAsync(update.Id).ConfigureAwait(false);
            if (update.IsAdmin.HasValue)
            {
                user.IsAdmin = update.IsAdmin.Value;
            }
            if (update.Password != null)
            {
                user.PasswordHash = HashPassword(update.Password);
            }
            if (update.StudyPlanId.HasValue)
            {
                user.StudyPlanId = update.StudyPlanId.Value;
            }
            if (update.ProfilePictureJpgBase64 != null)
            {
                await _profilePictureRepo.Save(user.Id, update.ProfilePictureJpgBase64);
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [NotLoggedInFilter]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]

        public async Task<IActionResult> Login([FromBody] LoginDto info)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email.ToLowerInvariant() == info.Email.ToLowerInvariant());
            if (user == null || user.PasswordHash != HashPassword(info.Password))
            {
                return Unauthorized("Invalid login credentials.");
            }
            user.Token = GenerateToken(user);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(24),
                IsEssential = true,
                HttpOnly = false,
                Secure = false,
            };
            Response.Cookies.Append("Token", user.Token, cookieOptions);
            return Ok(_mapper.Map<UserDto>(user));
        }
        [LoggedInFilter]
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var user = await _dbContext.Users.FindAsync(this.GetUser()!.Id).ConfigureAwait(false);
            user.Token = null;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [NonAction]
        public void GetProfilePicture(int userId) { }
    }
}
