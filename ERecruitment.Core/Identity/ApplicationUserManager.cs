using System;
using ERecruitment.Core.Common;
using ERecruitment.Models.Accounts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace ERecruitment.Core.Identity
{
    public sealed class ApplicationUserManager : UserManager<Account, int>
    {
        public ApplicationUserManager(IUserStore<Account, int> store)
            //,IdentityFactoryOptions<UserManager<Account, int>> options)
            : base(store)
        {
            // Configure validation logic for usernames
            UserValidator = new UserValidator<Account, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and 
            //Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Account, int>
            {
                MessageFormat = "Your security code is {0}"
            });
            RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Account, int>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            //Note: http://stackoverflow.com/questions/30060932/how-to-access-dataprotectionprovider-in-simple-class
            /* Throws cryptographic exception.
            * var provider = new MachineKeyProtectionProvider();*/

            IDataProtectionProvider provider = CommonHelper.DataProtectionProvider;
            if (provider != null)
            {
                UserTokenProvider =
                    new DataProtectorTokenProvider<Account, int>(provider.Create("Jobatease Identity"));
            }
        }
    }
}