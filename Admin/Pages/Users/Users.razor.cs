using Data;

namespace Admin.Pages
{
    public class UsersComponent : BaseComponent<AppUser>
    {
        public UsersComponent() : base("api/users") { }

        protected override string GetListODataOptions()
        {
            return "$expand=UserRoles($select=Id)";
        }

        protected override string GetSingleODataOptions()
        {
            return "$expand=UserRoles($select=Id)";
        }
    }
}