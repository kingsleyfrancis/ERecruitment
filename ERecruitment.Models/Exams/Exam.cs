using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERecruitment.Models.Exams
{
    public class Exam
    {
        [Required]
        public int TestId { get; set; }

        public List<ExamQuestion> Questions { get; set; }
    }
}