using System.Collections.Generic;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Models.Questions;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;
using ERecruitment.Patterns.UnitOfWork;
using ERecruitment.Services.Time;

namespace ERecruitment.Services.Questions
{
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Question> _queRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public QuestionService(IRepository<Question> queRepo,
            IUnitOfWork unitOfWork, IClock clock)
        {
            _queRepo = queRepo;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public Question GetQuestion(int questionId)
        {
            if (!CommonHelper.IsWithinIntegerRange(questionId))
                return null;

            var question = _queRepo
                .Query(a => a.Id == questionId)
                .Include(a => a.Answers)
                .Include(a => a.Test)
                .Select()
                .FirstOrDefault();

            return question;
        }

        public List<Question> GetAllQuestions()
        {
            var questions = _queRepo
               .Query()
               .Include(a => a.Answers)
               .Include(a => a.Test)
               .Select()
               .ToList();

            return questions;
        }

        public void Delete(int questionId)
        {
            if (!CommonHelper.IsWithinIntegerRange(questionId))
                return;

            var question = GetQuestion(questionId);

            if (question != null)
            {
                question.ObjectState = ObjectState.Deleted;
                if (question.Answers.Any())
                {
                    var answers = new List<ValidAnswer>();
                    foreach (var answer in question.Answers)
                    {
                        answer.ObjectState = ObjectState.Deleted;
                        answers.Add(answer);
                    }
                    question.Answers = answers;
                }
                _queRepo.Delete(question);
                _unitOfWork.SaveChanges();
            }
        }

        public void UpdateQuestion(Question question)
        {
            if(question == null)
                return;

            question.ObjectState = ObjectState.Modified;
            
            _queRepo.Update(question);
            _unitOfWork.SaveChanges();
        }

        public void Add(Question question)
        {
           if(question == null)
               return;

            question.ObjectState = ObjectState.Added;
            question.AddedOn = _clock.GetCurrentDateTimeUtc();


            if (question.Answers != null && question.Answers.Any())
            {
                var answers = new List<ValidAnswer>();
                foreach (var ans in question.Answers)
                {
                    ans.ObjectState = ObjectState.Modified;
                    answers.Add(ans);
                }
                question.Answers = answers;
            }

            _queRepo.Insert(question);
            _unitOfWork.SaveChanges();
        }
    }
}