using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;
using Microsoft.AspNetCore.Http;

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

        private UserSkill() { }
        private UserSkill(string user_id, string skill_id)
        {
            this.user_id = user_id;
            this.skill_id = skill_id;

            Validate();
        }

        public static UserSkill Create(string user_id, string skill_id)
        {
            return new UserSkill(user_id, skill_id);
        }
        public void Update(string user_id, string skill_id)
        {
            this.user_id = user_id ?? this.user_id;
            this.skill_id = skill_id ?? this.skill_id;
        }
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(user_id) || string.IsNullOrWhiteSpace(skill_id))
                throw new BadHttpRequestException("user_id and skill_id are required!");
        }
    }
}