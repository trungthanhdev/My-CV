using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Contract.ProjectDto.Response
{
    public class ResProjectDto
    {
        public string? project_id { get; set; }
        public string? project_name { get; set; }
        public string? description { get; set; }
        public string? tech { get; set; }
        public string? project_type { get; set; }
        public bool? is_Reality { get; set; }
        public string? url_project { get; set; }
        public string? url_demo { get; set; }
        public string? url_github { get; set; }
        public string? duration { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
    }
}