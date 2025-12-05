using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Security.Identity
{
    public class Director: User
    {
        public Director(int userId, string fullName, string email) : base(userId, fullName, email, nameof(Director))
        {

        }
    }
}
