using System.Web.Mvc;
using ERecruitment.Services.Accounts;
using ERecruitment.Services.Tests;
using Microsoft.AspNet.Identity;

namespace ERecruitment.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ITestService _testService;
        private readonly IAccountService _accountService;
        private readonly Core.Identity.ApplicationUserManager _userManager;

        public HomeController(ITestService testService,
            IAccountService accountService, 
            Core.Identity.ApplicationUserManager userManager)
        {
            _testService = testService;
            _accountService = accountService;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated) 
                return View();

            var clientId = User.Identity.GetUserId<int>();

            var isAdmin = _userManager.IsInRole(clientId, "admin");

            if (isAdmin)
            {
                return RedirectToAction("index", "dashboard", new {Area = "Admin"});
            }

            var clientName = _accountService.GetFullname(clientId);

            ViewBag.ClientName = clientName;

            var readyTests = _testService.GetReadyTests();
            return View("Dashboard", readyTests);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}