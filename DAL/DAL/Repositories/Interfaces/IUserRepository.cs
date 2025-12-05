using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        UserEntity GetByEmail(string email);
    }
}
