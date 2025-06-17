using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Contract.WEDTO.Response
{
    public class ResWEDto
    {
        public string? we_id { get; set; }
        public string? user_id { get; set; }
        public string? company_name { get; set; }
        public string? position { get; set; }
        public string? duration { get; set; }
        public string? description { get; set; }
        public string? project_id { get; set; }
        public List<ResMyTask>? tasks { get; set; }
    }
    public record ResMyTask(string mt_id, string? task_description);
}