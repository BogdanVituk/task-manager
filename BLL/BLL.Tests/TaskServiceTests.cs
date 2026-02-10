using AutoMapper;
using BLL.DTO;
using BLL.DTO.Task;
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

public class TaskServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _taskService = new TaskService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    

    [Fact]
    public void Constructor_NullUnitOfWork_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TaskService(null, _mapperMock.Object));
    }

    

    [Fact]
    public void GetTaskById_UserNotAdminOrDirector_Throws()
    {
        User user = new Employee(1, "Oleg Ivanov", "sfdsfs@gmail.com");
        SecurityContext.SetUser(user);

        Assert.Throws<MethodAccessException>(() => _taskService.GetTaskById(1));

        _unitOfWorkMock.Verify(u => u.Tasks.Get(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetTaskById_ValidUser_ReturnsDto()
    {
        User user = new Admin(1, "Anton Ivanov", "sfdsfs@gmail.com");

        SecurityContext.SetUser(user);

        var entity = new TaskEntity { TaskId = 1, Title = "Test" };
        var dto = new TaskDto { Id = 1, Title = "Test" };

        _unitOfWorkMock.Setup(u => u.Tasks.Get(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<TaskDto>(entity)).Returns(dto);

        var result = _taskService.GetTaskById(1);

        Assert.Equal(1, result.Id);

        _unitOfWorkMock.Verify(u => u.Tasks.Get(1), Times.Once);
        _mapperMock.Verify(m => m.Map<TaskDto>(entity), Times.Once);
    }

    

    [Fact]
    public void GetAll_ReturnsMappedDtos()
    {
        var entities = new List<TaskEntity>
        {
            new TaskEntity{ TaskId = 1 },
            new TaskEntity{ TaskId = 2 }
        };

        var dtos = new List<TaskDto>
        {
            new TaskDto{ Id = 1 },
            new TaskDto{ Id = 2 }
        };

        _unitOfWorkMock.Setup(u => u.Tasks.GetAll()).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(entities)).Returns(dtos);

        var result = _taskService.GetAll();

        Assert.Equal(2, result.Count());

        _unitOfWorkMock.Verify(u => u.Tasks.GetAll(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<TaskDto>>(entities), Times.Once);
    }

    

    [Fact]
    public void CreateTask_UserNotFound_Throws()
    {
        var dto = new TaskCreateDto { EmployeeId = 10 };

        _unitOfWorkMock.Setup(u => u.Tasks.Get(10)).Returns((TaskEntity)null);

        Assert.Throws<Exception>(() => _taskService.CreateTask(dto));

        _unitOfWorkMock.Verify(u => u.Tasks.Create(It.IsAny<TaskEntity>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public void CreateTask_Valid_AddsTask()
    {
        var dto = new TaskCreateDto
        {
            Title = "Task",
            Description = "Desc",
            EmployeeId = 1,
            Deadline = DateTime.UtcNow
        };

        _unitOfWorkMock.Setup(u => u.Tasks.Get(1)).Returns(new TaskEntity());

        _taskService.CreateTask(dto);

        _unitOfWorkMock.Verify(u => u.Tasks.Create(It.IsAny<TaskEntity>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

    

    [Fact]
    public void UpdateTask_TaskNotFound_Throws()
    {
        _unitOfWorkMock.Setup(u => u.Tasks.Get(1)).Returns((TaskEntity)null);

        Assert.Throws<Exception>(() => _taskService.UpdateTask(1, new TaskUpdateDto()));

        _unitOfWorkMock.Verify(u => u.Tasks.Update(It.IsAny<TaskEntity>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public void UpdateTask_Valid_UpdatesFields()
    {
        var task = new TaskEntity { TaskId = 1, Status = StatusEnums.Overdue };

        _unitOfWorkMock.Setup(u => u.Tasks.Get(1)).Returns(task);

        var dto = new TaskUpdateDto
        {
            Title = "NewTitle",
            Status = StatusEnums.Done
        };

        _taskService.UpdateTask(1, dto);

        Assert.Equal("NewTitle", task.Title);
        Assert.Equal(StatusEnums.Done, task.Status);

        _unitOfWorkMock.Verify(u => u.Tasks.Update(task), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

    

    [Fact]
    public void DeleteTask_NotFound_Throws()
    {
        _unitOfWorkMock.Setup(u => u.Tasks.Get(1)).Returns((TaskEntity)null);

        Assert.Throws<Exception>(() => _taskService.DeleteTask(1));

        _unitOfWorkMock.Verify(u => u.Tasks.Delete(It.IsAny<int>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Never);
    }

    [Fact]
    public void DeleteTask_Valid_CallsDelete()
    {
        _unitOfWorkMock.Setup(u => u.Tasks.Get(1)).Returns(new TaskEntity());

        _taskService.DeleteTask(1);

        _unitOfWorkMock.Verify(u => u.Tasks.Delete(1), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

    // ---------------------- CheckDeadlines ------------------------

    [Fact]
    public void CheckDeadlines_OverdueTasksAreUpdated()
    {
        var oldDate = DateTime.UtcNow.AddDays(-3);

        var tasks = new List<TaskEntity>
        {
            new TaskEntity { TaskId = 1, Deadline = oldDate, Status = StatusEnums.InProgress }
        };

        _unitOfWorkMock.Setup(u => u.Tasks.GetByStatus(StatusEnums.InProgress))
                       .Returns(tasks);

        _taskService.CheckDeadlines();

        Assert.Equal(StatusEnums.Overdue, tasks[0].Status);

        _unitOfWorkMock.Verify(u => u.Tasks.Update(tasks[0]), Times.Once);
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
    }

    // ---------------------- GetTasksByStatus ------------------------

    [Fact]
    public void GetTasksByStatus_ReturnsMapped()
    {
        var entities = new List<TaskEntity>
        {
            new TaskEntity { TaskId = 1 }
        };

        var dtos = new List<TaskDto>
        {
            new TaskDto { Id = 1 }
        };

        _unitOfWorkMock.Setup(u => u.Tasks.GetByStatus(StatusEnums.Done))
                       .Returns(entities);

        _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(entities)).Returns(dtos);

        var result = _taskService.GetTasksByStatus(StatusEnums.Done).Result;

        Assert.Single(result);

        _unitOfWorkMock.Verify(u => u.Tasks.GetByStatus(StatusEnums.Done), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<TaskDto>>(entities), Times.Once);
    }
}
