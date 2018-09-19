using System;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Models.Accounts;
using ERecruitment.Patterns.Repositories;
using ERecruitment.Patterns.UnitOfWork;

namespace ERecruitment.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IRepository<Account> accountRepo,
            IUnitOfWork unitOfWork)
        {
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
        }

        public int GetClientId(string email)
        {
            if (!CommonHelper.IsValidEmail(email))
                throw new Exception("Invalid email");

            int userId = _accountRepo.Query(a => a.Email.Equals(email, 
                StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefault();

            return userId;
        }

        public string GetFullname(int clientId)
        {
            if (!CommonHelper.IsWithinIntegerRange(clientId))
                return string.Empty;

            var acct = _accountRepo
                .Query(a => a.Id == clientId)
                .Select()
                .FirstOrDefault();

            if (acct == null)
                return string.Empty;

            var fullname = string.Format("{0} {1}", acct.Lastname, acct.Firstname);
            return fullname;
        }
    }
}