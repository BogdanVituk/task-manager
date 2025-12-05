using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Security.Identity
{
    public abstract class User
    {
        public User(int userId, string fullName, string email, string role)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
            Role = role;
        }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
