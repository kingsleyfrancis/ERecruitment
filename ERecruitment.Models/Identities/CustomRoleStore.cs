using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERecruitment.Models.Identities
{
    public class CustomRoleStore : RoleStore<UserRole, int, AccountUserRole>
    {
        public CustomRoleStore(DbContext context) : base(context)
        {
            base.DisposeContext = true;
        }
    }
}