using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Domain.DTO.WorkExperienceDto.Request
{
    public class ReqWorkExperienceDto
    {
        public string? project_id { get; set; }
        public string? description { get; set; }
        public string company_name { get; set; } = string.Empty;
        public string? duration { get; set; }
        public string? position { get; set; }
    }
}