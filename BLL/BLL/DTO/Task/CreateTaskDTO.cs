using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Task
{

    public class TaskCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
