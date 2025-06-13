using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Contract.ProjectDto.Request
{
    public class ReqUpdateProjectDto
    {
        public string? project_name { get; set; }
        public string? description { get; set; }
        public List<TechDto>? tech { get; set; }
        public string? project_type { get; set; }
        public bool? is_Reality { get; set; } = false;
        public string? url_project { get; set; }
        public string? url_demo { get; set; }
        public string? url_github { get; set; }
        public string? duration { get; set; }
        public DateTime? from { get; set; }
        public string? to { get; set; }

    }
    public class TechDto
    {
        public string tech_name { get; set; } = default!;
    }
}