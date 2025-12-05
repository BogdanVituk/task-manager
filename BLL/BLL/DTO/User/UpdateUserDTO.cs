using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BLL.DTO.User
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
