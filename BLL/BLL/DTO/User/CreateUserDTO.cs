using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.User
{
    public class CreateUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public RoleEnums Role { get; set; }
    }

}
