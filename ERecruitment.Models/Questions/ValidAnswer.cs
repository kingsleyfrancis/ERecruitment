using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Patterns.Infrastructure;

namespace ERecruitment.Models.Questions
{
    public class ValidAnswer : IEntity
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public int Id { get; set; }

        public byte[] TimeStamp { get; set; }

        public string Answer { get; set; }

        public bool IsValidAnswer { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
       
    }
}