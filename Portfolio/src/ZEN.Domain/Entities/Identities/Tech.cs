using System.ComponentModel.DataAnnotations.Schema;
using CTCore.DynamicQuery.Core.Domain;
using Microsoft.AspNetCore.Http;
using ZEN.Contract.ProjectDto.Request;

namespace ZEN.Domain.Entities.Identities
{
    public class Tech : CTBaseEntity
    {

        public string? tech_name { get; set; }
        [ForeignKey(nameof(Project))]
        public string? project_id { get; set; }
        public virtual Project Project { get; set; } = default!;
        private Tech() { }
        private Tech(string tech_name, string project_id)
        {
            this.tech_name = tech_name;
            this.project_id = project_id;

            Validate();
        }
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(tech_name)) throw new BadHttpRequestException("Tech name is required!");
            if (string.IsNullOrWhiteSpace(project_id)) throw new BadHttpRequestException("Project id is required!");
        }
        public static Tech Create(string tech_name, string project_id)
        {
            return new Tech(tech_name, project_id);
        }
    }
}