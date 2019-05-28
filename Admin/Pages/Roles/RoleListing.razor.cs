using Data;
using Admin.Shared;

namespace Admin.Pages.Roles
{
    public class RoleListingBase : EntityListingBase<Role, RoleForm>
    {
        public RoleListingBase() : base("api/roles") { }
    }
}
