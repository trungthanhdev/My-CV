using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;

namespace ZEN.Domain.Entities.Identities
{
    public class MyTask : CTBaseEntity
    {
        [ForeignKey(nameof(WorkExperience))]
        public string? we_id { get; set; }
        public WorkExperience? WorkExperience { get; set; }
        public string? task_description { get; set; }
        private MyTask() { }
        private MyTask(string we_id, string task_description)
        {
            this.we_id = we_id;
            this.task_description = task_description;
        }
        public static MyTask Create(string we_id, string task_description)
        {
            return new MyTask(we_id, task_description);
        }
        public void Update(string task_description)
        {
            this.task_description = task_description ?? task_description;
        }
    }
}