using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class UserEntity
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleEnums Role { get; set; }

        public IEnumerable<TaskEntity> TasksAsExecutor { get; set; }
        public IEnumerable<TaskEntity> TasksAsManager { get; set; }
        //public IEnumerable<Notification> Notifications { get; set; }
    }
}
