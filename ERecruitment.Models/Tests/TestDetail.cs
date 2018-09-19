using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Models.Accounts;
using ERecruitment.Patterns.Infrastructure;

namespace ERecruitment.Models.Tests
{
    public class TestDetail : IEntity
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public int Id { get; set; }

        public byte[] TimeStamp { get; set; }

        public bool HasTakenTest { get; set; }

        public float TestScore { get; set; }

        public virtual Test Test { get; set; }

        public int TestId { get; set; }

        public virtual Account Account { get; set; }

        public int AccountId { get; set; }
    }
}