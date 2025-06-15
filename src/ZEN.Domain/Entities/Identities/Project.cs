using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using ZEN.Contract.ProjectDto.Request;
using ZEN.Domain.Interfaces;
namespace ZEN.Domain.Entities.Identities
{
    public class Project : CTBaseEntity, IAggregationRoot
    {
        public string project_name { get; set; } = default!;
        public string? description { get; set; }
        public string? project_type { get; set; }
        public bool is_Reality { get; set; }
        public string? url_project { get; set; }
        public string? url_demo { get; set; }
        public string? url_github { get; set; }
        public string? duration { get; set; }
        public string? from { get; set; }
        public string? to { get; set; }
        public string? img_url { get; set; }

        public virtual List<UserProject> UserProjects { get; set; } = [];
        public virtual List<Tech> Teches { get; set; } = [];

        private IReadOnlyCollection<UserProject> userProjects => UserProjects.AsReadOnly();
        private IReadOnlyCollection<Tech> teches => Teches.AsReadOnly();

        private Project() { }
        private Project(
        string project_name,
        string? description,
        string? project_type,
        bool is_Reality,
        string? url_project,
        string? url_demo,
        string? url_github,
        string? duration,
        string? from,
        string? to,
        string? img_url
    )
        {
            this.project_name = project_name;
            this.description = description;
            this.project_type = project_type;
            this.is_Reality = is_Reality;
            this.url_project = url_project;
            this.url_demo = url_demo;
            this.url_github = url_github;
            this.duration = duration;
            this.from = from;
            this.to = to;
            this.img_url = img_url;
            Validate();
        }
        private void Validate()
        {
            if (string.IsNullOrEmpty(project_name))
            {
                throw new ArgumentNullException("Project name can not be empty!");
            }

        }
        public static Project Create(
                string project_name,
                string? description,
                // string? tech,
                string? project_type,
                bool is_reality,
                string? url_project,
                string? url_demo,
                string? url_github,
                string? duration,
                string? from,
                string? to,
                string? img_url
            )
        {
            return new Project(project_name, description, project_type, is_reality, url_project, url_demo, url_github, duration, from, to, img_url);
        }

        public void Update(ReqUpdateProjectDto reqUpdateProjectDto, string updated_img)
        {
            this.project_name = reqUpdateProjectDto.project_name ?? this.project_name;
            this.description = reqUpdateProjectDto.description ?? this.description;
            this.project_type = reqUpdateProjectDto.project_type ?? this.project_type;
            this.is_Reality = reqUpdateProjectDto.is_Reality ?? this.is_Reality;
            this.url_project = reqUpdateProjectDto.url_project ?? this.url_project;
            this.url_demo = reqUpdateProjectDto.url_demo ?? this.url_demo;
            this.url_github = reqUpdateProjectDto.url_github ?? this.url_github;
            this.duration = reqUpdateProjectDto.duration ?? this.duration;
            this.from = reqUpdateProjectDto.from ?? this.from;
            this.to = reqUpdateProjectDto.to ?? this.to;
            this.img_url = updated_img ?? img_url;
        }

        public void AddUserProject(string user_id, string project_id)
        {
            if (UserProjects.Any(x => x.user_id == user_id && x.project_id == project_id))
                throw new BadHttpRequestException($"Project {project_id} was created by user {user_id}");

            var userProject = UserProject.Create(user_id, project_id);
            UserProjects.Add(userProject);
        }

        public void AddTechToProject(string tech_name, string project_id)
        {
            if (string.IsNullOrWhiteSpace(tech_name)) throw new BadHttpRequestException("Tech is required!");
            var teches = Tech.Create(tech_name, project_id);
            Teches.Add(teches);
        }
        public void CreateNewTech(List<TechDto> teches, string project_id)
        {
            if (teches == null || teches.Any(t => string.IsNullOrWhiteSpace(t.tech_name)))
            {
                throw new ArgumentNullException("Tech name is required for all techs.");
            }
            foreach (var techDto in teches)
            {
                var newTech = Tech.Create(techDto.tech_name, project_id);
                Teches.Add(newTech);
            }
        }
    }


}