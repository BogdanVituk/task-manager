using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Task
{

    public class TaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public StatusEnums? Status { get; set; }
    }
}
