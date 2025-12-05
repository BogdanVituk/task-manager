using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Security.Identity
{
    public class Admin : User
    {
        public Admin(int userId, string fullName, string email) : base(userId, fullName, email, nameof(Admin))
        {
                
        }
    }
}
