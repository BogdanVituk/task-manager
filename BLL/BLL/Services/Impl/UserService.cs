using AutoMapper;
using BLL.DTO;
using BLL.DTO.User;
using BLL.Services.Interfaces;
using DAL.Entities;
using DAL.Security;
using DAL.Security.Identity;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int PageSize = 10;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
        }


        public UserDto GetUserById(int id)
        {
            var currentUser = SecurityContext.GetUser();

            if (currentUser.Role != "Admin")
                throw new MethodAccessException("У вас нема доступу до користувачів.");

            var user = _unitOfWork.Users.Get(id);

            if (user == null)
                throw new Exception("Користувач не знайдений.");

            return _mapper.Map<UserDto>(user);
        }

        
        public IEnumerable<UserDto> GetAll()
        {
            var users = _unitOfWork.Users.GetAll();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public void CreateUser(CreateUserDto dto)
        {
            if (_unitOfWork.Users.GetByEmail(dto.Email) != null)
                throw new Exception("Користувач з таким email вже існує.");

            var user = _mapper.Map<UserEntity>(CreateUser);
            

            _unitOfWork.Users.Create(user);
            _unitOfWork.Save();
        }

    
        public void UpdateUser(int id, UpdateUserDto dto)
        {
            var user = _unitOfWork.Users.Get(id);
            if (user == null)
                throw new Exception("Користувач не знайдений.");

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = dto.Password; 

        

            _unitOfWork.Users.Update(user);
            _unitOfWork.Save();
        }

      
        public void DeleteUser(int id)
        {
            var user = _unitOfWork.Users.Get(id);
            if (user == null)
                throw new Exception("Користувач не знайдений.");

            _unitOfWork.Users.Delete(id);
            _unitOfWork.Save();
        }

   
    }
}
