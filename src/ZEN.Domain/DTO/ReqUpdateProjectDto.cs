using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
        public string? from { get; set; }
        public string? to { get; set; }
        public IFormFile? img_url { get; set; }

    }
    public class TechDto
    {
        public string tech_name { get; set; } = default!;
    }
}