using System;
using System.Data.Entity;
using System.Threading.Tasks;
using ERecruitment.Models.Accounts;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERecruitment.Models.Identities
{
    public class CustomUserStore : UserStore<Account, UserRole, int,
        AccountUserLogin, AccountUserRole, AccountUserClaim>
    {
        private readonly DbContext _context;
        private readonly CustomRoleStore _roleStore;


        public CustomUserStore(DbContext context) : base(context)
        {
            _roleStore = new CustomRoleStore(context);
            _context = context;
        }

        /// <summary>
        ///     Add a user to a role
        /// </summary>
        /// <param name="account"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override async Task AddToRoleAsync(Account account, string roleName)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("The role name cannot be null or empty", "roleName");
            }
            UserRole roleEntity = await _roleStore.FindByNameAsync(roleName);
            if (roleEntity == null)
            {
                throw new InvalidOperationException("The specified role does not exist!");
            }

            var ur = new AccountUserRole
            {
                UserId = account.Id,
                RoleId = roleEntity.Id,
                ObjectState = ObjectState.Added
            };
            account.Roles.Add(ur);
            _context.SaveChanges();
        }

        /// <summary>
        ///     Set IsConfirmed on the user
        /// </summary>
        /// <param name="account"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public override Task SetEmailConfirmedAsync(Account account, bool confirmed)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            account.EmailConfirmed = confirmed;
            account.ObjectState = ObjectState.Modified;

            _context.SaveChangesAsync();
            return Task.FromResult(0);
        }


        /// <summary>
        ///     Set the password hash for a user
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public override Task SetPasswordHashAsync(Account account, string passwordHash)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            account.PasswordHash = passwordHash;


            if (account.Id > 0)
            {
                account.ObjectState = ObjectState.Modified;
                _context.SaveChangesAsync();
            }
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Set the security stamp for the user
        /// </summary>
        /// <param name="account"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public override Task SetSecurityStampAsync(Account account, string stamp)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            account.SecurityStamp = stamp;

            if (account.Id > 0)
            {
                account.ObjectState = ObjectState.Modified;
                _context.SaveChangesAsync();
            }
            return Task.FromResult(0);
        }


        public override Task SetEmailAsync(Account account, string email)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            account.Email = email;

            if (account.Id > 0)
            {
                account.ObjectState = ObjectState.Modified;
                _context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public override Task SetPhoneNumberAsync(Account account, string phoneNumber)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            account.PhoneNumber = phoneNumber;

            if (account.Id > 0)
            {
                account.ObjectState = ObjectState.Modified;
                _context.SaveChanges();
            }
            return Task.FromResult(0);
        }

        public override Task SetPhoneNumberConfirmedAsync(Account account, bool confirmed)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            account.PhoneNumberConfirmed = confirmed;

            if (account.Id > 0)
            {
                account.ObjectState = ObjectState.Modified;
                _context.SaveChanges();
            }
            return Task.FromResult(0);
        }
    }
}