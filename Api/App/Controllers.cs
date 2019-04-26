using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        [HttpGet, Route("{id}/{version}")]
        public async Task<IActionResult> Image(Guid id, int version)
        {
            // version is to disable browser cache
            var image = await _context.Images.FindAsync(id);
            return File(image.Data, image.Meta);
        }
    }
}
