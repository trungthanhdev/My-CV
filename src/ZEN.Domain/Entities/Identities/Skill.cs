using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;

namespace ZEN.Domain.Entities.Identities
{
    public class Skill : CTBaseEntity
    {
        public string skill_name { get; set; } = default!;
        public string? position { get; set; }

        public virtual List<UserSkill> UserSkills { get; set; } = [];
    }
}