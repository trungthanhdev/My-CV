using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Core.Domain;

namespace ZEN.Domain.Entities.Identities
{
    public class Certificate : CTBaseEntity
    {
        public string? certificate_name { get; set; }
        [ForeignKey(nameof(AspUser))]
        public string? user_id { get; set; }
        public virtual AspUser AspUser { get; set; } = default!;
        private Certificate() { }
        private Certificate(string user_id, string certificate_name)
        {
            this.user_id = user_id;
            this.certificate_name = certificate_name;
        }
        public static Certificate Create(string user_id, string certificate_name)
        {
            return new Certificate(user_id, certificate_name);
        }

        public void Update(string certificate_name)
        {
            this.certificate_name = certificate_name ?? certificate_name;
        }
    }
}