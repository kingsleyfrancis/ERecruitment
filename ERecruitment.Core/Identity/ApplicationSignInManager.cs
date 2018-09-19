using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ERecruitment.Core.Engine;
using ERecruitment.Models.Accounts;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace ERecruitment.Core.Identity
{
    public class ApplicationSignInManager : SignInManager<Account, int>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Account account)
        {
            return account.GenerateUserIdentityAsync(UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

    }
}