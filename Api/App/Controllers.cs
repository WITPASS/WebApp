using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
}
