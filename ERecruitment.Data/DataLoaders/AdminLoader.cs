using System.Data.Entity;
using System.Diagnostics;
using ERecruitment.Core.Common;
using ERecruitment.Core.Identity;
using ERecruitment.Models.Accounts;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Identities;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ERecruitment.Data.DataLoaders
{
    public class AdminLoader : IMigrationDataLoader
    {
        public int Order
        {
            get { return 2; }
        }

        public void Load(DataContext context)
        {
            LoadAdmin(context);
        }

        public ApplicationRoleManager ReturnRoleManager(DataContext context)
        {
            var customRoleStore = new CustomRoleStore(context);
            var roleManager = new ApplicationRoleManager(customRoleStore);

            return roleManager;
        }

        private ApplicationUserManager ReturnUserManager(DataContext context)
        {
            var customUserStore = new CustomUserStore(context);
            var factoryOptions = new IdentityFactoryOptions<UserManager<Account, int>>();

            //set data protection provider.
            CommonHelper.DataProtectionProvider = factoryOptions.DataProtectionProvider;

            var userManager = new ApplicationUserManager(customUserStore);
            return userManager;
        }

        private void LoadAdmin(DataContext context)
        {
            using (var userManager = ReturnUserManager(context))
            {
                var account = new Account
                {
                    Email = "test@mail.com",
                    Firstname = "John",
                    Lastname = "Doe",
                    PhoneNumber = "+2340831234561",
                    UserName = "test@mail.com",
                    EmailConfirmed = true,
                    AccountType = AccountType.Admin,
                    State = "Enugu State",
                    ObjectState = ObjectState.Added
                };

                const string password = "testPassword0";

                //Insert user
                if (userManager.FindByEmail(account.Email) != null)
                    return;

                var identityResult = userManager.Create(account, password);

                if (!identityResult.Succeeded) 
                    return;

                context.SaveChanges();

                if (account.Id > 0)
                {
                    const string role = "admin";
                    if (userManager.IsInRole(account.Id, role))
                        return;
                    IdentityResult result = userManager
                        .AddToRole(account.Id, role);
                    context.SaveChanges();
                }
            }
        }
    }
}