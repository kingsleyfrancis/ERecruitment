using System.Net;
using System.Web;
using System.Web.Mvc;
using ERecruitment.Services.Tests;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ERecruitment.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ITestService _testService; 

        public DashboardController(ITestService testService)
        {
            _testService = testService;
        }


        public ActionResult Index()
        {
            var tests = _testService.GetAllTests();

            ViewBag.Tests = tests;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("index", "home", new{ area = ""});
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
    }
}