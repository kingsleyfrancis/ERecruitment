
using System.Web.Mvc;
using ERecruitment.Core.Caching;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Questions;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.UnitOfWork;
using ERecruitment.Services.Questions;
using ERecruitment.Services.Tests;

namespace ERecruitment.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class QuestionController : Controller
    {

        private readonly IQuestionService _questionService;
        private readonly ITestService _testService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheManager _cache;

        public QuestionController(IQuestionService questionService,
            IUnitOfWork unitOfWork, ICacheManager cache,
            ITestService testService)
        {
            _questionService = questionService;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _testService = testService;
        }

        [HttpGet]
        public ActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var queId = id.Value;
                var que = _questionService.GetQuestion(queId);

                if (que != null)
                {
                    return View("Index", que);
                }
            }
            var questions = _questionService.GetAllQuestions();
            return View("List", questions);
        }

        [HttpGet]
        public ActionResult Add(int? testId)
        {
            ViewBag.FormMode = FormMode.Create;
            ViewBag.TestId = testId.HasValue ? testId.Value : 0;

            if (testId.HasValue)
            {
                var id = testId.Value;
                var test = _testService.GetTest(id);

                if (test != null)
                {
                    var count = test.QuestionsCount;
                    if (test.Questions != null && test.Questions.Count == count)
                    {
                        return RedirectToAction("preview", "test", new{id});
                    }
                }
            }

            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                var questionId = id.Value;

                var question = _questionService.GetQuestion(questionId);
                if (question != null)
                {
                    var questionModel = new QuestionModel
                    {
                        Body = question.QuestionBody,
                        Score = question.Score,
                        QuestionType = question.QuestionType,
                        TestId = question.TestId,
                        Id = question.Id
                    };

                    ViewBag.FormMode = FormMode.Edit;
                    ViewBag.TestId = question.TestId;
                   
                    return View("Add", questionModel);
                }
            }
            ViewBag.FormMode = FormMode.Create;

            return View("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(QuestionModel model, FormMode mode)
        {
            if (ModelState.IsValid)
            {
                switch (mode)
                {
                    case FormMode.Create:
                    {
                        CreateQuestion(model);
                        break;
                    }
                    case FormMode.Edit:
                    {
                        UpdateQuestion(model);
                        break;
                    }
                    default:
                    {
                        return RedirectToAction("Index");
                    }
                }

                var questionType = model.QuestionType;
                return RedirectToAction("index", "answers", 
                    new {id = model.Id, type = questionType});
            }

            ViewBag.FormMode = mode;
            return View("Add", model);
        }

        private void UpdateQuestion(QuestionModel model)
        {
            if(model == null)
                return;

            var question = _questionService.GetQuestion(model.Id);

            if (question == null)
                return;

            question.QuestionBody = model.Body;
            question.Score = model.Score;
            question.QuestionType = model.QuestionType;
            question.ObjectState = ObjectState.Modified;

            _questionService.UpdateQuestion(question);
            model.Id = question.Id;

        }

        private void CreateQuestion(QuestionModel model)
        {
            if(model == null)
                return;

            var question = new Question
            {
                QuestionType = model.QuestionType,
                Score = model.Score,
                QuestionBody = model.Body,
                TestId = model.TestId,
                ObjectState = ObjectState.Added
            };

            _questionService.Add(question);
            model.Id = question.Id;

        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");

            var quesId = id.Value;
            _questionService.Delete(quesId);

            return RedirectToAction("Index");
        }
    }
}