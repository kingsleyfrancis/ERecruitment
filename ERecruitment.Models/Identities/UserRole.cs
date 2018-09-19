using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERecruitment.Models.Identities
{
    public class UserRole : IdentityRole<int, AccountUserRole>, IEntity
    {
        public UserRole()
        {
        }

        public UserRole(string name)
        {
            Name = name;
        }

        public string Description { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        [NotMapped, Timestamp]
        public byte[] TimeStamp { get; set; }
    }
}