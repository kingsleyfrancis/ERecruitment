using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ERecruitment.Core.Engine;
using ERecruitment.Services.Tests;

namespace ERecruitment.Common
{
    public static class SiteHelper
    {
        private static readonly string[] _states =
        {
            "Abia", "Adamawa", "Anambra", "Akwa Ibom",
            "Bauchi", "Bayelsa", "Benue", "Borno", "Cross River", "Delta",
            "Ebonyi", "Enugu", "Edo", "Ekiti", "Gombe", "Imo", "Jigawa",
            "Kaduna", "Kano", "Katsina", "Kebbi", "Kogi", "Kwara", "Lagos",
            "Nasarawa", "Niger", "Ogun", "Ondo", "Osun", "Oyo", "Plateau",
            "Rivers", "Sokoto", "Taraba", "Yobe", "Zamfara"
        };

        public static List<SelectListItem> GetStateListItems()
        {
            List<SelectListItem> stateList = _states
                .Select(state => string.Format("{0} state", state))
                .Select(temp => new SelectListItem
                {
                    Text = temp,
                    Value = temp
                }).ToList();

            return stateList;
        }

        public static List<SelectListItem> GetTestListItems()
        {
            var ec = EngineContext.Current;
            var testService = ec.Resolve<ITestService>();
            var tests = testService.GetAllTests();

            var testList = new List<SelectListItem>();

            if (tests.Any())
            {
                testList.AddRange(tests.Select(test => new SelectListItem
                {
                    Text = test.TestTitle, 
                    Value = test.Id.ToString()
                }));
            }
            return testList;
        }
    }
}