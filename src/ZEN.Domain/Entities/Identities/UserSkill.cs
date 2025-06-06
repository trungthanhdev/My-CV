using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;

namespace ZEN.Domain.Entities.Identities
{
    public class UserSkill : CTBaseEntity
    {
        [ForeignKey(nameof(AspUser))]
        public string? user_id { get; set; }
        public virtual AspUser AspUser { get; set; } = default!;

        [ForeignKey(nameof(Skill))]
        public string? skill_id { get; set; }
        public virtual Skill Skill { get; set; } = default!;
    }
}