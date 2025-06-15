using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;
using Microsoft.AspNetCore.Http;
using ZEN.Domain.Interfaces;

namespace ZEN.Domain.Entities.Identities
{
    public class Skill : CTBaseEntity, IAggregationRoot
    {
        public string skill_name { get; set; } = default!;
        public string? position { get; set; }

        public virtual List<UserSkill> UserSkills { get; set; } = [];
        private Skill() { }
        private Skill(string skill_name, string position)
        {
            this.skill_name = skill_name;
            this.position = position;

            Validate();
        }

        public static Skill Create(string skill_name, string position)
        {
            return new Skill(skill_name, position);
        }
        public void Update(string skill_name, string position)
        {
            this.skill_name = skill_name ?? this.skill_name;
            this.position = position ?? this.position;
        }
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(skill_name))
                throw new BadHttpRequestException("Skill name is required!");
        }
    }
}