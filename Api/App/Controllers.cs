using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public override async Task<IActionResult> Put(Guid id, AppUser ent)
        {
            if (ent.Password == null)
            {
                var user = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                ent.Password = user.Password;
            }

            return await base.Put(id, ent);
        }
    }

    public class UserRolesController : BaseController<UserRole>
    {
        public UserRolesController(AppDbContext context) : base(context, context.UserRoles) { }
    }

    public class ImagesController : BaseController<Image>
    {
        public ImagesController(AppDbContext context) : base(context, context.Images) { }

        public override async Task<IActionResult> Put(Guid id, Image ent)
        {
            if (ent.Data == null)
            {
                var image = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                ent.Data = image.Data;
            }

            return await base.Put(id, ent);
        }

        [HttpGet, Route("{id}/{version}")]
        public async Task<IActionResult> Image(Guid id, int version)
        {
            // version is to disable browser cache
            var image = await _context.Images.FindAsync(id);
            return File(image.Data, image.Meta);
        }
    }
}
