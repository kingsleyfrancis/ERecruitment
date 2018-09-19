using System;
using System.Collections.Generic;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Models.Questions;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;
using ERecruitment.Patterns.UnitOfWork;

namespace ERecruitment.Services.Answers
{
    public class AnswerService : IAnswerService
    {
        private readonly IRepository<ValidAnswer> _ansRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AnswerService(IRepository<ValidAnswer> ansRepo,
            IUnitOfWork unitOfWork)
        {
            _ansRepo = ansRepo;
            _unitOfWork = unitOfWork;
        }

        public void Add(ValidAnswer answer)
        {
           if(answer == null)
               return;

            if (answer.Question == null &&
                !CommonHelper.IsWithinIntegerRange(answer.QuestionId))
            {
                throw new Exception("Answer has no question attached to it.");
            }

            answer.ObjectState = ObjectState.Added;


            _ansRepo.Insert(answer);
            _unitOfWork.SaveChanges();
        }

        public ValidAnswer GetAnswer(int answerId)
        {
            if (!CommonHelper.IsWithinIntegerRange(answerId))
                return null;

            var answer = _ansRepo.Query(a => a.Id == answerId)
                .Include(q => q.Question)
                .Select()
                .FirstOrDefault();

            return answer;
        }

        public List<ValidAnswer> GetAnswersForQuestion(int questionId)
        {
            var list = new List<ValidAnswer>();
            if (!CommonHelper.IsWithinIntegerRange(questionId))
                return list;

            list = _ansRepo.Query(a => a.QuestionId == questionId)
                .Include(a => a.Question)
                .Select()
                .ToList();

            return list;
        }

        public void DeleteAnswer(int answerid)
        {
            if(!CommonHelper.IsWithinIntegerRange(answerid))
                return;

            var answer = GetAnswer(answerid);
            if(answer == null)
                return;

            answer.ObjectState = ObjectState.Deleted;

            _ansRepo.Delete(answer);
            _unitOfWork.SaveChanges();
        }

        public ValidAnswer GetValidAnswerForQuestion(int questionId)
        {
            if(!CommonHelper.IsWithinIntegerRange(questionId))
                return null;

            var answer = _ansRepo
                .Query(a => a.QuestionId == questionId &&
                            a.IsValidAnswer)
                .Select()
                .FirstOrDefault();

            return answer;
        }
    }
}