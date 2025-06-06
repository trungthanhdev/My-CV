using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;

namespace ZEN.Domain.Entities.Identities
{
    public class UserProject : CTBaseEntity
    {
        [ForeignKey(nameof(AspUser))]
        public string? user_id { get; set; }
        public virtual AspUser AspUser { get; set; } = default!;

        [ForeignKey(nameof(Project))]
        public string? project_id { get; set; }
        public virtual Project Project { get; set; } = default!;
        private UserProject() { }
        private UserProject(string user, string project)
        {
            user_id = user;
            project_id = project;
            Validate();
        }
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(user_id))
                throw new ArgumentNullException(nameof(user_id));
            if (string.IsNullOrWhiteSpace(project_id))
                throw new ArgumentNullException(nameof(project_id));
        }

        public static UserProject Create(
                string aspUser_id,
                string project_id
            )
        {
            return new UserProject(aspUser_id, project_id);
        }
    }
}