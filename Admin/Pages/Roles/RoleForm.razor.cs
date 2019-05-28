using Admin.Shared;
using Data;

namespace Admin.Pages.Roles
{
    public class RoleFormBase : EntityFormBase<Role>
    {
        public RoleFormBase() : base("api/roles") { }
    }
}