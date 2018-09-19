
using System.ComponentModel.DataAnnotations;
using ERecruitment.Models.Enums;

namespace ERecruitment.Models.Questions
{
    public class QuestionModel
    {
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public float Score { get; set; }

        [Display(Name = "Question type")]
        public QuestionType QuestionType { get; set; }

        [Required]
        [Display(Name = "Test title")]
        public int TestId { get; set; }
    }
}
