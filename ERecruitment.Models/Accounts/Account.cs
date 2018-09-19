using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Identities;
using ERecruitment.Models.Questions;
using ERecruitment.Models.Tests;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERecruitment.Models.Accounts
{
    public class Account : IdentityUser<int, AccountUserLogin, AccountUserRole, AccountUserClaim>, IEntity
    {
        private ICollection<TestDetail> _testDetails;
 
        public AccountType AccountType { get; set; }
       
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string State { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public byte[] TimeStamp { get; set; }

        public virtual ICollection<TestDetail> Tests
        {
            get { return _testDetails ?? (_testDetails = new List<TestDetail>()); }
            set { _testDetails = value; }
        } 

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Account, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity =
                await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            var claim = new Claim("AcctType", AccountType.ToString());

            userIdentity.AddClaim(claim);

            // Add custom user claims here
            return userIdentity;
        }
    }
}