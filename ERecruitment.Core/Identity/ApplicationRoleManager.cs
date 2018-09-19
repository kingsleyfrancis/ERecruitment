using ERecruitment.Models.Identities;
using Microsoft.AspNet.Identity;

namespace ERecruitment.Core.Identity
{
    public class ApplicationRoleManager : RoleManager<UserRole, int>
    {
        public ApplicationRoleManager(CustomRoleStore roleStore)
            : base(roleStore)
        {
        }
    }
}