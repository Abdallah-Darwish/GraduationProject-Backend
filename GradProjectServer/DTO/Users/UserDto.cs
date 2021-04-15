using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Users
{
    //todo: fill me
    public class UserDto : UserMetadataDto
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
