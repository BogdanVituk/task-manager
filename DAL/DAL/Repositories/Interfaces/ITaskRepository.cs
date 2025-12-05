using DAL.Entities;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
    public interface ITaskRepository : IRepository<TaskEntity>
    {
        IEnumerable<TaskEntity> GetOverdueTasks();

        IEnumerable<TaskEntity> GetByStatus(StatusEnums status);
    }
}
