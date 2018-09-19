using System.Collections.Generic;
using ERecruitment.Models.Questions;

namespace ERecruitment.Services.Questions
{
    public interface IQuestionService
    {
        Question GetQuestion(int questionId);

        List<Question> GetAllQuestions();

        void Delete(int questionId);

        void UpdateQuestion(Question question);

        void Add(Question question);
    }
}