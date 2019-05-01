using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Authorize(Roles = "Super")]
    public class BranchesController : EntityController<Branch>
    {
        public BranchesController(AppDbContext context) : base(context, context.Branches) { }
    }

    [Authorize(Roles = "Admins")]
    public class RolesController : BranchEntityController<Role>
    {
        public RolesController(AppDbContext context) : base(context, context.Roles) { }
    }

    [Authorize(Roles = "Admins")]
    public class UsersController : BranchEntityController<AppUser>
    {
        public UsersController(AppDbContext context) : base(context, context.Users) { }

        // below sql is to skip Password column
        readonly private string _sql = "select \"Id\", \"Name\", \"Email\", '' as \"Password\", \"BranchId\" from \"Users\"";

        public override async Task<ActionResult<IEnumerable<AppUser>>> Get()
        {
            return await _dbSet.FromSql(_sql).AsNoTracking().Where(c => c.BranchId == BranchId).ToListAsync();
        }

        public override async Task<ActionResult<AppUser>> Get(Guid id)
        {
            var ent = await _dbSet.FromSql(_sql).AsNoTracking().Where(c => c.BranchId == BranchId).FirstOrDefaultAsync(c => c.Id == id);

            if (ent == null)
            {
                return NotFound();
            }

            return ent;
        }

        public override async Task<IActionResult> Put(Guid id, AppUser ent)
        {
            if (string.IsNullOrWhiteSpace(ent.Password))
            {
                var user = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                ent.Password = user.Password;
            }

            return await base.Put(id, ent);
        }

        [HttpPost, Route("login"), AllowAnonymous]
        public ActionResult<UserInfo> Login([FromBody] LoginInfo login)
        {
            var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var user = _dbSet.AsNoTracking().Include(c=>c.Branch).FirstOrDefault(c => c.Email == login.Email && c.Password == login.Password);

            if (user == null) return BadRequest("email or password is incorrect");

            if (config["SUPER_USER"] == login.Email)
            {
                var roles = new List<string> { "Super", "Admins" };
                return GetToken(config["API_KEY"], user.Id.ToString(), user.Name, null, roles);
            }
            else
            {
                var roles = _context.UserRoles.AsNoTracking().Where(c => c.UserId == user.Id).Select(c => c.Role.Name).ToList();
                return GetToken(config["API_KEY"], user.Id.ToString(), user.Name, user.Branch, roles);
            }
        }

        private UserInfo GetToken(string key, string subject, string user, Branch branch, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var bytes = Encoding.ASCII.GetBytes(key);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, subject), new Claim("branch", branch == null ? string.Empty : branch.Id.ToString()) };

            claims.AddRange(roles.Select(c =>
            {
                return new Claim(ClaimTypes.Role, c);
            }));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(bytes), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userInfo = new UserInfo
            {
                Branch = branch == null ? string.Empty : branch.Name,
                User = user,
                Token = tokenHandler.WriteToken(token)
            };

            return userInfo;
        }
    }

    [Authorize(Roles = "Admins")]
    public class UserRolesController : BranchEntityController<UserRole>
    {
        public UserRolesController(AppDbContext context) : base(context, context.UserRoles) { }
    }

    public class ImagesController : BranchEntityController<Image>
    {
        public ImagesController(AppDbContext context) : base(context, context.Images) { }

        // below sql is used to skip Data column
        readonly private string _sql = "select \"Id\", \"BranchId\", \"Name\", \"Title\", \"Description\", \"Width\", \"Height\"," +
                " \"Size\", \"Meta\", \"Version\", \"Active\", ''::bytea as \"Data\" from \"Images\"";

        public override async Task<ActionResult<IEnumerable<Image>>> Get()
        {
            return await _dbSet.FromSql(_sql).AsNoTracking().Where(c => c.BranchId == BranchId).ToListAsync();
        }

        public override async Task<ActionResult<Image>> Get(Guid id)
        {
            var ent = await _dbSet.FromSql(_sql).AsNoTracking().Where(c => c.BranchId == BranchId).FirstOrDefaultAsync(c => c.Id == id);

            if (ent == null)
            {
                return NotFound();
            }

            return ent;
        }

        public override async Task<IActionResult> Put(Guid id, Image ent)
        {
            if (ent.Data.Length == 0)
            {
                // TODO: instead of fetching and saving Image.Data a patch save should be used excluding Image.Data field
                var image = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                ent.Data = image.Data;
            }

            return await base.Put(id, ent);
        }

        [HttpGet, Route("{id}/{version}"), AllowAnonymous]
        public async Task<IActionResult> Image(Guid id, int version)
        {
            // version is to disable browser cache

            var image = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return File(image.Data, image.Meta);
        }
    }
}
