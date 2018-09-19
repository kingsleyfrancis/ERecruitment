using System.Collections.Generic;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Core.Identity;
using ERecruitment.Models.Accounts;
using ERecruitment.Models.Identities;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ERecruitment.Data.DataLoaders
{
    public class EssentialData : IMigrationDataLoader
    {
        public int Order
        {
            get { return 1; }
        }

        public void Load(DataContext context)
        {
            CreateRoles(context);
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

        private void CreateRoles(DataContext context)
        {
            ApplicationRoleManager roleManager = ReturnRoleManager(context);

            var roles = new List<string>
            {
                "user",
                "admin"
            };

            foreach (IdentityResult result in
                from role in roles
                where !roleManager.RoleExists(role)
                select new UserRole(role) {ObjectState = ObjectState.Added}
                into userRole
                select roleManager.Create(userRole))
            {
                context.SaveChanges();
            }
        }
    }
}