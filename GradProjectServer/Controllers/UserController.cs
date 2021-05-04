using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProjectServer.Common;
using GradProjectServer.Services.FilesManagers;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IQueryable<User> GetPreparedQueryable(bool metadata = false)
        {
            var q = _dbContext.Users.AsQueryable();
            if (!metadata)
            {
                q = q.Include(u => u.StudyPlan)
                    .ThenInclude(s => s.Major);
            }

            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager _userManager;
        private readonly UserFileManager _userFileManager;

        public UserController(AppDbContext dbContext, IMapper mapper, UserManager userManager,
            UserFileManager userFileManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _userFileManager = userFileManager;
        }

        /// <summary>
        /// Ids of users ordered by email.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        [AdminFilter]
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.Users.Skip(info.Offset).Take(info.Count).Select(u => u.Id));
        }

        /// <param name="usersIds">Ids of the users to get.</param>
        /// <param name="metadata">Whether to return UserMetadataDto or UserDto.</param>
        /// <remarks>
        /// A user can get:
        ///     1- UserDto of HIMSELF only.
        ///     2- UserMetadataDto of all users.
        /// An admin can get:
        ///     All users.
        /// </remarks>
        /// <response code="404">Ids of the non existing users.</response>
        /// <response code="403">Ids of user the user has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<UserMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public IActionResult Get([FromBody] int[] usersIds, bool metadata = false)
        {
            var users = GetPreparedQueryable(metadata);
            var existingUsers = users.Where(e => usersIds.Contains(e.Id));
            var nonExistingUsers = usersIds.Except(existingUsers.Select(e => e.Id)).ToArray();
            if (nonExistingUsers.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following users don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingUsers"] = nonExistingUsers}
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
                            Data = new Dictionary<string, object> {["NotAllowedToGetUsers"] = notAllowedToGetUsers}
                        });
                }
            }

            if (metadata)
            {
                return Ok(_mapper.ProjectTo<UserMetadataDto>(existingUsers));
            }

            return Ok(_mapper.ProjectTo<UserDto>(existingUsers));
        }

        /// <summary>Creates/Signsup a new user.</summary>
        /// <response code="201">Metadata of the newly user exam.</response>
        [NotLoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<UserDto>> Create([FromBody] SignUpDto dto)
        {
            var user = await _userManager.SignUp(dto.Email, dto.Password, dto.Name, dto.StudyPlanId)
                .ConfigureAwait(false);

            if (dto.ProfilePictureJpgBase64 != null)
            {
                await using var pic =
                    await Utility.DecodeBase64Async(dto.ProfilePictureJpgBase64).ConfigureAwait(false);
                await _userFileManager.SaveProfilePicture(user, pic).ConfigureAwait(false);
            }

            return CreatedAtAction(nameof(Get), new {usersIds = new[] {user.Id}, metadata = false},
                _mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <remarks>
        /// A user can update only himself.
        /// An admin can update any user.
        /// IsAdmin will be considered only if the caller is an admin.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [LoggedInFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto update)
        {
            var user = await _dbContext.Users.FindAsync(update.Id).ConfigureAwait(false);
            var loggedInUser = this.GetUser()!;
            await _userManager.UpdateUser(user.Id, update.Password, update.Name,
                loggedInUser.IsAdmin && update.IsAdmin.HasValue ? update.IsAdmin.Value : (bool?) null,
                update.StudyPlanId).ConfigureAwait(false);
            if (update.ProfilePictureJpgBase64 != null)
            {
                await using var pic =
                    await Utility.DecodeBase64Async(update.ProfilePictureJpgBase64).ConfigureAwait(false);
                await _userFileManager.SaveProfilePicture(user, pic).ConfigureAwait(false);
            }

            return Ok();
        }

        /// <summary>
        /// Generates login cookie for a user to be used in subsequent requests.
        /// </summary>
        /// <remarks>
        /// A user can't be logged in before calling this method.
        /// </remarks>
        /// <response code="200">Successful login, will return the token and Create a set session cookie.</response>
        /// <response code="401">Invalid login credentials.</response>
        [NotLoggedInFilter]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto info)
        {
            var user = await _userManager.Login(info.Email, info.Password, Response.Cookies).ConfigureAwait(false);
            if (user == null)
            {
                return Unauthorized("Invalid login credentials.");
            }

            LoginResultDto result = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = user.Token!
            };
            return Ok(result);
        }

        /// <summary>
        /// Logs out the current user and removes his cookie.
        /// </summary>
        [LoggedInFilter]
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var user = this.GetUser()!;
            await _userManager.Logout(user, Response.Cookies).ConfigureAwait(false);
            return Ok();
        }

        /// <remarks>
        /// Gets the user profile picture as stream of bytes with header Content-Type: image/jpeg.
        /// </remarks>
        /// <param name="userId">Id of the user to get his profile picture.</param>
        /// <response code="404">If there is no user with this id.</response>
        /// <response code="204">If the user doesn't have a profile picture.</response>
        [HttpGet("GetProfilePicture")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfilePicture([FromQuery] int userId)
        {
            if ((await _dbContext.Users.FindAsync(userId).ConfigureAwait(false)) == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "There is no user with the following Id.",
                        Data = new Dictionary<string, object> {["UserId"] = userId}
                    });
            }

            var userImage = _userFileManager.GetProfilePicture(userId);
            if (userImage == null)
            {
                return NoContent();
            }

            var result = File(userImage, "image/jpeg");
            result.FileDownloadName = $"{userId}_ProfilePicture.jpg";
            return result;
        }

        [LoggedInFilter]
        [HttpGet("GetLoggedIn")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public ActionResult<UserDto> GetLoggedIn()
        {
            var user = this.GetUser()!;
            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}