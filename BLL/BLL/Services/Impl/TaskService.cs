using Appccelerate.MappingEventBroker;
using AutoMapper;
using BLL.DTO;
using BLL.DTO.Task;
using BLL.Services.Interfaces;
using DAL.Entities;
using DAL.Enums;
using DAL.Security;
using DAL.Security.Identity;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BLL.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 10;
        private readonly AutoMapper.IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

 
        public TaskDto GetTaskById(int id)
        {
           
            var user = SecurityContext.GetUser();
          

            if (user.Role != "Admin" && user.Role != "Director")
                throw new MethodAccessException("У вас нема доступу до списку задач.");

            int userId = user.UserId;

        
            var tasksEntities = _unitOfWork.Tasks.Get(id);

            return _mapper.Map<TaskDto>(tasksEntities);
        }

        public IEnumerable<TaskDto> GetAll()
        {

            var tasks = _unitOfWork.Tasks.GetAll();

            var res = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            return res;
        }

        public void CreateTask(TaskCreateDto taskDto)
        {
            var user = _unitOfWork.Tasks.Get(taskDto.EmployeeId);

            if (user == null) {
                throw new Exception("User does not exist.");
            }

            var task = new TaskEntity
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                ExecutorId = taskDto.EmployeeId,
                Status = StatusEnums.InProgress,
                Deadline = taskDto.Deadline
            };

            _unitOfWork.Tasks.Create(task);
            _unitOfWork.Save();

        }

        public void UpdateTask(int id,TaskUpdateDto dto)
        {
            var task = _unitOfWork.Tasks.Get(id);

            if (task == null)
            {
                throw new Exception("task does not exist.");
            }

            if (!string.IsNullOrEmpty(dto.Title))
                task.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                task.Description = dto.Description;

            //if (!dto.Status)
            //    task.Status = dto.Status;

           
            _unitOfWork.Tasks.Update(task);
            _unitOfWork.Save();

        }

        public void DeleteTask(int id)
        {
            var task = _unitOfWork.Tasks.Get(id);

            if (task == null)
            {
                throw new Exception("task does not exist.");
            }

            _unitOfWork.Tasks.Delete(id);
            _unitOfWork.Save();

        }

        public  void CheckDeadlines()
        {
            var tasks =  _unitOfWork.Tasks.GetByStatus(StatusEnums.InProgress);
            var now = DateTime.UtcNow;

            foreach (var task in tasks)
            {
                if (now > task.Deadline)
                {
                    task.Status = StatusEnums.Overdue;
                    _unitOfWork.Tasks.Update(task);
                   
                    
                }
            }

             _unitOfWork.Save();
        }

        public Task<IEnumerable<TaskDto>> GetTasksByStatus(StatusEnums status)
        {
            var tasks = _unitOfWork.Tasks.GetByStatus(status);

            return _mapper.Map<Task<IEnumerable<TaskDto>>>(tasks);
        }



       
    }
}
