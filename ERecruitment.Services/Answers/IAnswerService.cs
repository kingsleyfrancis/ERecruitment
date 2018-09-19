using System.Collections.Generic;
using ERecruitment.Models.Questions;

namespace ERecruitment.Services.Answers
{
    public interface IAnswerService
    {
        void Add(ValidAnswer answer);

        ValidAnswer GetAnswer(int answerId);

        List<ValidAnswer> GetAnswersForQuestion(int questionId);

        void DeleteAnswer(int answerid);

        ValidAnswer GetValidAnswerForQuestion(int questionId);
    }
}