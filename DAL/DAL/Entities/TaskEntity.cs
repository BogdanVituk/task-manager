using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class TaskEntity
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public StatusEnums Status { get; set; } 

        public int ExecutorId { get; set; }
        public UserEntity Executor { get; set; }

        public int ManagerId { get; set; }
        public UserEntity Manager { get; set; }

    }
}
