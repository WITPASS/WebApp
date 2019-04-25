using Data;

namespace Api.Controllers
{
    public class RolesController : BaseController<Role>
    {
        public RolesController(AppDbContext context) : base(context, context.Roles) { }
    }

    public class UsersController : BaseController<AppUser>
    {
        public UsersController(AppDbContext context) : base(context, context.Users) { }
    }

    public class UserRolesController : BaseController<UserRole>
    {
        public UserRolesController(AppDbContext context) : base(context, context.UserRoles) { }
    }

    public class ImagesController : BaseController<Image>
    {
        public ImagesController(AppDbContext context) : base(context, context.Images) { }
    }
}
