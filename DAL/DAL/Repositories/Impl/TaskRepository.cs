using DAL.Entities;
using DAL.Enums;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Impl
{
    public class TaskRepository : BaseRepository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(DbContext context) : base(context) { }

        public IEnumerable<TaskEntity> GetOverdueTasks()
        {
            return _set.Where(t => t.Status == Enums.StatusEnums.InProgress && t.Deadline < DateTime.Now)
                       .ToList();
        }

        public IEnumerable<TaskEntity> GetByStatus(StatusEnums status)
        {
            return _set.Where(t => t.Status == status).ToList();
        }
    }
}
