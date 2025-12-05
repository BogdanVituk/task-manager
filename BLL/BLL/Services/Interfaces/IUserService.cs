using BLL.DTO.Task;
using BLL.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services.Interfaces
{
    internal interface IUserService
    {

        UserDto GetUserById(int id);
        IEnumerable<UserDto> GetAll();

        void CreateUser(CreateUserDto dto);
        void UpdateUser(int id, UpdateUserDto dto);
        void DeleteUser(int id);
    }
}
