using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERecruitment.Models.Identities
{
    public class AccountUserLogin : IdentityUserLogin<int>, IEntity
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }

        [NotMapped]
        public int Id { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }
    }
}