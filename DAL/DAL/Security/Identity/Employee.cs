using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Security.Identity
{
    public class Employee : User
    {
        public Employee(int userId, string fullName, string email) : base(userId, fullName, email, nameof(Employee))
        {

        }
    }
}
