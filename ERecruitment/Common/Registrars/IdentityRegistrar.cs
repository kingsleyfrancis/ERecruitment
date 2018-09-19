using System.Data.Entity;
using System.Web;
using Autofac;
using ERecruitment.Core.Common;
using ERecruitment.Core.Engine;
using ERecruitment.Core.Identity;
using ERecruitment.Data;
using ERecruitment.Models.Accounts;
using ERecruitment.Models.Identities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;

namespace ERecruitment.Common.Registrars
{
    public class IdentityRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 2; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<DataContext>()
               .As<IdentityDbContext<Account, UserRole, int,
                   AccountUserLogin, AccountUserRole, AccountUserClaim>>()
               .As<DbContext>()
               .InstancePerLifetimeScope();


            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication)
                .As<IAuthenticationManager>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomUserStore>()
                .As<UserStore<Account, UserRole, int, AccountUserLogin,
                    AccountUserRole, AccountUserClaim>>()
                .As<IUserStore<Account, int>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomRoleStore>()
                .As<RoleStore<UserRole, int, AccountUserRole>>()
                .As<IRoleStore<UserRole, int>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<Core.Identity.ApplicationUserManager>()
                .AsSelf()
                .InstancePerLifetimeScope();

            /* builder.Register<Identity.ApplicationUserManager>(c => HttpContext.Current.GetOwinContext().GetUserManager)
                .As<ApplicationUserManager>();*/
            builder.RegisterType<Core.Identity.ApplicationSignInManager>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<ApplicationRoleManager>()
                .AsSelf()
                .InstancePerLifetimeScope();

            var options = new IdentityFactoryOptions<ApplicationUserManager>
            {
                DataProtectionProvider = new DpapiDataProtectionProvider("ERecruitment Application​")
            };
            builder.Register(c => options).AsSelf();
        }
    }
}