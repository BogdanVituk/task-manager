using BLL.DTO;
using BLL.DTO.Task;
using DAL.Entities;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ITaskService
    {
        TaskDto GetTaskById(int id);
        IEnumerable<TaskDto> GetAll();
        Task<IEnumerable<TaskDto>> GetTasksByStatus(StatusEnums status);

        void CreateTask(TaskCreateDto dto);
        void UpdateTask(int id, TaskUpdateDto dto);
        void DeleteTask(int id);

    }
}
