using ERecruitment.Models.Tests;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;
using ERecruitment.Patterns.UnitOfWork;

namespace ERecruitment.Services.TestDetails
{
    public class TestDetailService : ITestDetailService
    {
        private readonly IRepository<TestDetail> _testDetailRepo;
        private readonly IUnitOfWork _unitOfWork;

        public TestDetailService(IRepository<TestDetail> testDetailRepo,
            IUnitOfWork unitOfWork)
        {
            _testDetailRepo = testDetailRepo;
            _unitOfWork = unitOfWork;
        }

        public void AddDetail(TestDetail testDetail)
        {
            if(testDetail == null)
                return;

            testDetail.ObjectState = ObjectState.Added;

            _testDetailRepo.Insert(testDetail);
            _unitOfWork.SaveChanges();
        }
    }
}