using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain;
using Microsoft.AspNetCore.Http;
using ZEN.Domain.DTO.WorkExperienceDto.Request;
using ZEN.Domain.Interfaces;

namespace ZEN.Domain.Entities.Identities
{
    public class WorkExperience : CTBaseEntity, IAggregationRoot
    {
        [ForeignKey(nameof(AspUser))]
        public string? user_id { get; set; }
        public virtual AspUser AspUser { get; set; } = default!;

        public string company_name { get; set; } = default!;
        public string? position { get; set; }
        public string? duration { get; set; }
        public string? description { get; set; }
        public string? project_id { get; set; }
        public virtual List<MyTask> MyTasks { get; set; } = [];
        private IReadOnlyCollection<MyTask> myTask => MyTasks.AsReadOnly();
        private WorkExperience() { }
        private WorkExperience(
            string user_id,
            string company_name,
            string? position,
            string? duration,
            string? description,
            string? project_id)
        {
            this.user_id = user_id;
            this.company_name = company_name;
            this.position = position;
            this.duration = duration;
            this.description = description;
            this.project_id = project_id;

            Validate();
        }
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(company_name) || string.IsNullOrWhiteSpace(user_id))
                throw new BadHttpRequestException("Company name and User_id are required!");
        }

        public static WorkExperience Create(
            string user_id,
            string company_name,
            string? position,
            string? duration,
            string? description,
            string? project_id)
        {
            return new WorkExperience(user_id, company_name, position, duration, description, project_id);
        }

        public void Update(ReqWorkExperienceDto dto)
        {
            this.company_name = dto.company_name ?? company_name;
            this.description = dto.description ?? description;
            this.duration = dto.duration ?? duration;
            this.position = dto.position ?? position;
            this.project_id = dto.project_id ?? project_id;
        }

        public void AddMyTask(string task_description)
        {
            var myTask = MyTask.Create(this.Id, task_description);
            MyTasks.Add(myTask);
        }
        public void UpdateMyTask(string myTask_id, string updateMyTask)
        {
            var currentTask = MyTasks.FirstOrDefault(x => x.Id == myTask_id);
            if (currentTask is null) throw new NotFoundException("Task not found!");
            currentTask.Update(updateMyTask);
        }
    }
}