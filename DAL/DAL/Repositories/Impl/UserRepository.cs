using DAL.Entities;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Impl
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }

        public UserEntity GetByEmail(string email)
        {
            return _set.FirstOrDefault(t => t.Email == email);
        }
    }
}
