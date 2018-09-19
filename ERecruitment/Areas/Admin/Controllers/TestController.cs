using System.Collections.Generic;
using System.Web.Mvc;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Tests;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.UnitOfWork;
using ERecruitment.Services.Tests;
using CH = ERecruitment.Core.Common.CommonHelper;

namespace ERecruitment.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class TestController : Controller
    {
        private readonly ITestService _testService;
        private readonly IUnitOfWork _unitOfWork;

        public TestController(ITestService testService, IUnitOfWork unitOfWork)
        {
            _testService = testService;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public ActionResult Index()
        {
            List<Test> tests = _testService.GetAllTests();
            return View(tests);
        }


        [HttpGet]
        public ActionResult Preview(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");

            int testId = id.Value;
            Test test = _testService.GetTest(testId);

            if (test == null)
                return RedirectToAction("Index");

            return View(test);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");

            int testId = id.Value;
            _testService.DeleteTest(testId);

            return RedirectToAction("index", "dashboard");
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.FormMode = FormMode.Create;
            return View("Add");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");

            int testId = id.Value;
            Test test = _testService.GetTest(testId);

            if (test == null)
                return RedirectToAction("Index");

            var testModel = new TestModel
            {
                Id = test.Id,
                TestTitle = test.TestTitle,
                QuestionsCount = test.QuestionsCount,
                TotalScore = test.TotalScore
            };

            ViewBag.FormMode = FormMode.Edit;
            return View("Add", testModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(TestModel test, FormMode mode)
        {
            if (ModelState.IsValid)
            {
                switch (mode)
                {
                    case FormMode.Edit:
                    {
                        UpdateTest(test);
                        break;
                    }
                    case FormMode.Create:
                    {
                        CreateTest(test);
                        break;
                    }
                    default:
                    {
                        return RedirectToAction("index");
                    }
                }
                return RedirectToAction("add", "question", new {testid = test.Id});
            }
            ViewBag.FormMode = mode;
            return View(test);
        }

        private void CreateTest(TestModel model)
        {
            if (model == null)
                return;

            var test = new Test
            {
                TestTitle = model.TestTitle,
                QuestionsCount = model.QuestionsCount,
                TotalScore = model.TotalScore
            };

            _testService.AddTest(test);
            model.Id = test.Id;
        }

        private void UpdateTest(TestModel model)
        {
            if (model == null)
                return;
            Test test = _testService.GetTest(model.Id);
            if (test == null)
                return;

            test.TestTitle = model.TestTitle;
            test.QuestionsCount = model.QuestionsCount;
            test.TotalScore = model.TotalScore;
            test.ObjectState = ObjectState.Modified;

            _testService.AddTest(test);
        }

        public ActionResult Activate(int id)
        {
            if (CH.IsWithinIntegerRange(id))
            {
                _testService.MarkAsReady(id, true);
            }
            return RedirectToAction("index");
        }


        public ActionResult Deactivate(int id)
        {
            if (CH.IsWithinIntegerRange(id))
            {
                _testService.MarkAsReady(id, false);
            }
            return RedirectToAction("index");
        }

    }
}