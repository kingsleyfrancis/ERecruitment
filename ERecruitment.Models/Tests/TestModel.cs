

using System.ComponentModel.DataAnnotations;

namespace ERecruitment.Models.Tests
{
    public class TestModel
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        public string TestTitle { get; set; }

        [Display(Name = "Count")]
        [Required(ErrorMessage = "Questions count is required")]
        public int QuestionsCount { get; set; }

        [Display(Name = "Total score")]
        [Required(ErrorMessage = "Total score is required")]
        public float TotalScore { get; set; }

    }
}
