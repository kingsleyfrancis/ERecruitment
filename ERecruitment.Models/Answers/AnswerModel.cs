
using System.ComponentModel.DataAnnotations;

namespace ERecruitment.Models.Answers
{
    public class AnswerModel
    {
        [Required]
        public string Answer { get; set; }

        public string AnswerCharacter { get; set; }
    }
}
