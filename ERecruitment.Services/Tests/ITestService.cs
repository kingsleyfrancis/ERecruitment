using System.Collections.Generic;
using ERecruitment.Models.Questions;
using ERecruitment.Models.Tests;

namespace ERecruitment.Services.Tests
{
    public interface ITestService
    {
        void AddTest(Test test);

        Test GetTest(int testId);

        List<Test> GetAllTests();

        void DeleteTest(int testId);

        bool IsReady(int testId);

        bool MarkAsReady(int testId, bool state);

        List<Test> GetReadyTests();
    }
}