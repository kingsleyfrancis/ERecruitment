using System;
using System.Linq;
using System.Web.Mvc;
using ERecruitment.Models.Exams;
using ERecruitment.Models.Tests;
using ERecruitment.Services.Answers;
using ERecruitment.Services.Logs;
using ERecruitment.Services.Questions;
using ERecruitment.Services.TestDetails;
using ERecruitment.Services.Tests;
using Microsoft.AspNet.Identity;

namespace ERecruitment.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly ITestService _testService;
        private readonly IQuestionService _questionService;
        private readonly ILogService _logService;
        private readonly IAnswerService _answerService;
        private readonly ITestDetailService _testDetailService;

        public ExamController(ITestService testService, 
            IQuestionService questionService,
            ILogService logService, 
            IAnswerService answerService, 
            ITestDetailService testDetailService)
        {
            _testService = testService;
            _questionService = questionService;
            _logService = logService;
            _answerService = answerService;
            _testDetailService = testDetailService;
        }

        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("index", "home");

            var testId = id.Value;
            var isReady = _testService.IsReady(testId);

            if (isReady)
            {
                var test = _testService.GetTest(testId);

                return View(test);
            }

            return RedirectToAction("index", "home");
        }

        public ActionResult Start(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("index", "home");

            var testId = id.Value;
            var test = _testService.GetTest(testId);

            ViewBag.Test = test;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(Exam model)
        {
            if (!ModelState.IsValid)
            {
                _logService.Warning("The test id was not avaliable when an exam was taken");
                return View("ExamError");
            }


            var testId = model.TestId;
            var test = _testService.GetTest(testId);

            float testScore = 0;
            bool suceeded = ProcessQuestions(model, test, out testScore);
            if (suceeded)
            {
                var totalScore = test.TotalScore;
                ViewBag.TestScore = testScore;
                ViewBag.TotalScore = totalScore;

                return View("ExamScore");
            }
            return View("ExamError");
        }

        private bool ProcessQuestions(Exam model, Test test, out float testScore)
        {
            testScore = 0;
            var testId = model.TestId;
            var clientId = User.Identity.GetUserId<int>();

            var testDetail = new TestDetail
            {
                AccountId = clientId,
                TestId = testId,
                HasTakenTest = true
            };

            if (test != null)
            {
                var questions = test.Questions;
                if (questions.Any())
                {
                    foreach (var question in questions)
                    {
                        var question1 = question;

                        var examQuestion = model.Questions
                            .Where(a => a.QuestionId == question1.Id)
                            .Select(a => a).FirstOrDefault();

                        if (examQuestion != null)
                        {
                            var validAnswer = _answerService
                                .GetValidAnswerForQuestion(question.Id);

                            if (validAnswer != null)
                            {
                                var ansString = validAnswer.Answer;


                                if (ansString.Equals(examQuestion.Answer,
                                    StringComparison.InvariantCultureIgnoreCase))
                                {
                                    testScore += question.Score;
                                }
                            }
                            else
                            {
                                var errMsg = string.Format("The question {0} has no valid answer",
                                    question.Id);

                                _logService.Error(errMsg);
                                return false;
                            }
                        }
                        else
                        {
                            var errMsg = string.Format("The question with id {0} answered " +
                                                       "by the user does not exist", question1.Id);
                            _logService.Error(errMsg);
                            return false;
                        }
                    }
                    testDetail.TestScore = testScore;
                    _testDetailService.AddDetail(testDetail);

                    return true;
                }
            }
            return false;
        }
    }
}