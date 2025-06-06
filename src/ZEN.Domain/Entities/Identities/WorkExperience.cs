using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;

namespace ZEN.Domain.Entities.Identities
{
    public class WorkExperience : CTBaseEntity
    {
        [ForeignKey(nameof(AspUser))]
        public string? user_id { get; set; }
        public virtual AspUser AspUser { get; set; } = default!;

        public string company_name { get; set; } = default!;
        public string? position { get; set; }
        public string? duration { get; set; }
        public string? description { get; set; }

        // [ForeignKey(nameof(Project))]
        public string? project_id { get; set; }
        // public virtual Project? Project { get; set; }
    }
}