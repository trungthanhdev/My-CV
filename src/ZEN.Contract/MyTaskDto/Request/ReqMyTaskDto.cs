using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Contract.MyTaskDto.Request
{
    public class ReqMyTaskDto
    {
        public string? myTask_id { get; set; }
        public string? task_description { get; set; }
    }
}