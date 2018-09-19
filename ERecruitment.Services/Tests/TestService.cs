using System.Collections.Generic;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Models.Questions;
using ERecruitment.Models.Tests;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;
using ERecruitment.Patterns.UnitOfWork;
using ERecruitment.Services.Time;

namespace ERecruitment.Services.Tests
{
    public class TestService : ITestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Test> _testRepo;
        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<TestDetail> _testDetailRepo;
        private readonly IClock _clock;

        public TestService(IUnitOfWork unitOfWork, 
            IRepository<Test> testRepo, 
            IRepository<Question> questionRepo,
            IRepository<TestDetail> testDetailRepo, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _testRepo = testRepo;
            _questionRepo = questionRepo;
            _testDetailRepo = testDetailRepo;
            _clock = clock;
        }

        public void AddTest(Test test)
        {
            if(test == null)
                return;

            if (test.Id > 0)
            {
                test.ObjectState = ObjectState.Modified;
                _testRepo.Update(test);
            }
            else
            {
                test.ObjectState = ObjectState.Added;
                test.AddedOn = _clock.GetCurrentDateTimeUtc();

                _testRepo.Insert(test);
            }
            _unitOfWork.SaveChanges();
        }

        public Test GetTest(int testId)
        {
            if (!CommonHelper.IsWithinIntegerRange(testId))
                return null;

            var test = _testRepo
                .Query(a => a.Id == testId)
                .Include(a => a.Questions)
                .Include(a => a.TestDetails)
                .Select()
                .FirstOrDefault();

            return test;
        }

        public List<Test> GetAllTests()
        {
            var test = _testRepo
                .Query()
                .Include(a => a.Questions)
                .Include(a => a.TestDetails)
                .OrderBy(a =>a.OrderByDescending(b => b.AddedOn))
                .Select()
                .ToList();

            return test;
        }

        public void DeleteTest(int testId)
        {
            var test = GetTest(testId);
            if (test == null)
                return;

            var questions = test.Questions;
            var testDetails = test.TestDetails;

            if (questions != null && questions.Any())
            {
                foreach (var question in questions)
                {
                    question.ObjectState = ObjectState.Deleted;
                    _questionRepo.Delete(question);
                }
            }

            if (testDetails != null &&
                testDetails.Any())
            {
                foreach (var detail in testDetails)
                {
                    detail.ObjectState = ObjectState.Deleted;
                    _testDetailRepo.Delete(detail);
                }
            }
            _testRepo.Delete(test);
            _unitOfWork.SaveChanges();
        }

        public bool IsReady(int testId)
        {
            if (!CommonHelper.IsWithinIntegerRange(testId))
                return false;

            var test = _testRepo.Query(a => a.Id == testId)
                .Select().FirstOrDefault();

            return test != null && test.IsReady;
        }

        public bool MarkAsReady(int testId, bool state)
        {
            if(!CommonHelper.IsWithinIntegerRange(testId))
                return false;

            var test = GetTest(testId);
            if (test == null)
                return false;

            test.IsReady = state;
            test.ObjectState = ObjectState.Modified;

            _testRepo.Update(test);
            _unitOfWork.SaveChanges();

            return true;
        }

        public List<Test> GetReadyTests()
        {
            var tests = _testRepo.Query(a => a.IsReady)
                .Include(a => a.Questions)
                .Include(a => a.TestDetails)
                .Select().ToList();

            return tests;
        }
    }
}