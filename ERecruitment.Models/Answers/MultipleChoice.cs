using System.Collections.Generic;

namespace ERecruitment.Models.Answers
{
    public class MultipleChoice
    {
        public string ValidAnswer { get; set; }

        public List<AnswerModel> Answers { get; set; }
    }
}