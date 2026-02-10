using AutoMapper;
using BLL.DTO;
using BLL.DTO.User;
using BLL.Services.Impl;
using DAL.Entities;
using DAL.Enums;
using DAL.Security;
using DAL.Security.Identity;
using DAL.UnitOfWork;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

   
    [Fact]
    public void Constructor_NullUnitOfWork_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UserService(null, _mapperMock.Object));
    }

    
    [Fact]
    public void GetUserById_UserNotAdmin_Throws()
    {
        User user = new Employee(1, "sdfsdfs", "sdfsd@gmail.com");
        SecurityContext.SetUser(user);

        Assert.Throws<MethodAccessException>(() => _userService.GetUserById(1));
    }

    [Fact]
    public void GetUserById_Admin_ReturnsDto()
    {
        User user = new Admin(1, "sdfsdfs", "sdfsd@gmail.com");

        SecurityContext.SetUser(user);

        var entity = new UserEntity { UserId = 1, FullName = "Test User", Email = "test@test.com" };
        var dto = new UserDto { Id = 1, FullName = "Test User", Email = "test@test.com" };

        _unitOfWorkMock.Setup(u => u.Users.Get(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<UserDto>(entity)).Returns(dto);

        var result = _userService.GetUserById(1);

        Assert.Equal(1, result.Id);
        Assert.Equal("Test User", result.FullName);
        Assert.Equal("test@test.com", result.Email);
    }

    
    [Fact]
    public void GetAll_ReturnsMappedDtos()
    {
        var entities = new List<UserEntity>
        {
            new UserEntity { UserId = 1 },
            new UserEntity { UserId = 2 }
        };

        var dtos = new List<UserDto>
        {
            new UserDto { Id = 1 },
            new UserDto { Id = 2 }
        };

        _unitOfWorkMock.Setup(u => u.Users.GetAll()).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(entities)).Returns(dtos);

        var result = _userService.GetAll();

        Assert.Equal(2, result.Count());
    }

    
    [Fact]
    public void CreateUser_EmailAlreadyExists_Throws()
    {
        var dto = new CreateUserDto { Email = "existing@test.com" };
        _unitOfWorkMock.Setup(u => u.Users.GetByEmail("existing@test.com"))
                       .Returns(new UserEntity());

        Assert.Throws<Exception>(() => _userService.CreateUser(dto));
    }

    [Fact]
    public void CreateUser_Valid_AddsUser()
    {
        var dto = new CreateUserDto
        {
            FullName = "New User",
            Email = "new@test.com",
            Password = "pass",
            Role = RoleEnums.Employee
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByEmail("new@test.com")).Returns((UserEntity)null);

        _userService.CreateUser(dto);

        _unitOfWorkMock.Verify(u => u.Users.Create(It.IsAny<UserEntity>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

    
    [Fact]
    public void UpdateUser_UserNotFound_Throws()
    {
        _unitOfWorkMock.Setup(u => u.Users.Get(1)).Returns((UserEntity)null);

        Assert.Throws<Exception>(() => _userService.UpdateUser(1, new UpdateUserDto()));
    }

    [Fact]
    public void UpdateUser_Valid_UpdatesFields()
    {
        var user = new UserEntity { UserId = 1, FullName = "Old Name", Email = "old@test.com", Role = RoleEnums.Admin };
        _unitOfWorkMock.Setup(u => u.Users.Get(1)).Returns(user);

        var dto = new UpdateUserDto { FullName = "New Name", Email = "new@test.com", Role = "Admin" };

        _userService.UpdateUser(1, dto);

        Assert.Equal("New Name", user.FullName);
        Assert.Equal("new@test.com", user.Email);
        Assert.Equal(RoleEnums.Admin, user.Role);

        _unitOfWorkMock.Verify(u => u.Users.Update(user), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

   
    [Fact]
    public void DeleteUser_UserNotFound_Throws()
    {
        _unitOfWorkMock.Setup(u => u.Users.Get(1)).Returns((UserEntity)null);

        Assert.Throws<Exception>(() => _userService.DeleteUser(1));
    }

    [Fact]
    public void DeleteUser_Valid_CallsDelete()
    {
        _unitOfWorkMock.Setup(u => u.Users.Get(1)).Returns(new UserEntity());

        _userService.DeleteUser(1);

        _unitOfWorkMock.Verify(u => u.Users.Delete(1), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

 
}
