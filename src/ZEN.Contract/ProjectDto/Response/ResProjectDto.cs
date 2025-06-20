using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZEN.Contract.ProjectDto.Request;

namespace ZEN.Contract.ProjectDto.Response
{
    public class ResProjectDto
    {
        public string? project_id { get; set; }
        public string? project_name { get; set; }
        public string? description { get; set; }
        public string? project_type { get; set; }
        public bool? is_Reality { get; set; }
        public string? url_project { get; set; }
        public string? url_demo { get; set; }
        public string? img_url { get; set; }
        public string? url_github { get; set; }
        public string? duration { get; set; }
        public string? from { get; set; }
        public string? to { get; set; }
        public string? url_contract { get; set; }
        public string? url_excel { get; set; }
        public List<TechDto>? teches { get; set; }
    }
}