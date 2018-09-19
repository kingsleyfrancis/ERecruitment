using System;
using System.Linq;
using System.Web.Mvc;
using ERecruitment.Models.Answers;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Questions;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Services.Answers;
using ERecruitment.Services.Questions;
using CH = ERecruitment.Core.Common.CommonHelper;

namespace ERecruitment.Areas.Admin.Controllers
{
    public class AnswersController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;

        public AnswersController(IAnswerService answerService,
            IQuestionService questionService)
        {
            _answerService = answerService;
            _questionService = questionService;
        }

        public ActionResult Index(int id, QuestionType type)
        {
            if (!CH.IsWithinIntegerRange(id))
            {
                ViewBag.ErrorMessage = "The question id provided was " +
                                       "invalid so an answer could not be added.";
                return View("Error");
            }

            Question question = _questionService.GetQuestion(id);
            if (question == null)
            {
                return View("Error");
            }

            string body = question.QuestionBody;
            ViewBag.Question = body;
            ViewBag.QuestionId = id;

            switch (type)
            {
                case QuestionType.Objective:
                {
                    return View("MultipleChoice");
                }
                case QuestionType.Theory:
                {
                    return View("Theory");
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MultipleChoice(int quesid, MultipleChoice model)
        {
            if (!CH.IsWithinIntegerRange(quesid))
                return View("Error");

            if (model != null && model.Answers.Any())
            {
                string validAnswer = model.ValidAnswer;

                foreach (AnswerModel ans in model.Answers)
                {
                    if (!string.IsNullOrWhiteSpace(ans.Answer))
                    {
                        var answer = new ValidAnswer
                        {
                            Answer = ans.Answer,
                            QuestionId = quesid,
                            ObjectState = ObjectState.Added
                        };

                        if (validAnswer.Equals(ans.AnswerCharacter,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            answer.IsValidAnswer = true;
                        }

                        _answerService.Add(answer);
                    }
                }

                return RedirectToAction("add", "question");
            }
            ModelState.AddModelError("", "You must add one or more answers to this question.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Theory(int quesid, AnswerModel model)
        {
            if (CH.IsWithinIntegerRange(quesid) &&
                ModelState.IsValid)
            {
                var answer = new ValidAnswer
                {
                    Answer = model.Answer,
                    IsValidAnswer = true,
                    ObjectState = ObjectState.Added,
                    QuestionId = quesid
                };

                _answerService.Add(answer);

                return RedirectToAction("add", "question");
            }
            ModelState.AddModelError("", "You must add an answer to this question.");
            return View(model);
        }
    }
}