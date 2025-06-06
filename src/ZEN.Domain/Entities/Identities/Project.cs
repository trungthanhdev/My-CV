using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;
using CTCore.DynamicQuery.Core.Primitives;
using ZEN.Contract.ProjectDto.Request;

namespace ZEN.Domain.Entities.Identities
{
    public class Project : CTBaseEntity
    {
        public string project_name { get; set; } = default!;
        public string? description { get; set; }
        public string? tech { get; set; }
        public string? project_type { get; set; }
        public bool is_Reality { get; set; }

        public string? url_project { get; set; }
        public string? url_demo { get; set; }
        public string? url_github { get; set; }
        public string? duration { get; set; }

        public virtual List<UserProject> UserProjects { get; set; } = [];

        private Project() { }
        private Project(
        string project_name,
        string? description,
        string? tech,
        string? project_type,
        bool is_Reality,
        string? url_project,
        string? url_demo,
        string? url_github,
        string? duration
    )
        {
            this.project_name = project_name;
            this.description = description;
            this.tech = tech;
            this.project_type = project_type;
            this.is_Reality = is_Reality;
            this.url_project = url_project;
            this.url_demo = url_demo;
            this.url_github = url_github;
            this.duration = duration;

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
                string? tech,
                string? project_type,
                bool is_reality,
                string? url_project,
                string? url_demo,
                string? url_github,
                string? duration
            )
        {
            return new Project(project_name, description, tech, project_type, is_reality, url_project, url_demo, url_github, duration);
        }

        public void Update(ReqUpdateProjectDto reqUpdateProjectDto)
        {
            this.project_name = reqUpdateProjectDto.project_name ?? this.project_name;
            this.description = reqUpdateProjectDto.description ?? this.description;
            this.tech = reqUpdateProjectDto.tech ?? this.tech;
            this.project_type = reqUpdateProjectDto.project_type ?? this.project_type;
            this.is_Reality = reqUpdateProjectDto.is_Reality ?? this.is_Reality;
            this.url_project = reqUpdateProjectDto.url_project ?? this.url_project;
            this.url_demo = reqUpdateProjectDto.url_demo ?? this.url_demo;
            this.url_github = reqUpdateProjectDto.url_github ?? this.url_github;
            this.duration = reqUpdateProjectDto.duration ?? this.duration;
        }
    }
}