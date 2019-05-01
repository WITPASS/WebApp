using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public partial class SeedData
    {
        /*
        public static async Task RequiredData(AppDbContext context, Guid adminId, Guid adminsRoleId)
        {
            var jobAppMenu = await AddAppMenu(context, "Manage Jobs", "Job", "/Index", adminsRoleId, 0);
            await AddAppMenu(context, "Manage Projects", "Project", "/Projects/Index", adminsRoleId, 1);
            await AddAppMenu(context, "Manage Offers", "Offer", "/Offers/Index", adminsRoleId, 2);
            await AddAppMenu(context, "Manage Users", "User", "/Users/Index", adminsRoleId, 3);
            // job management
            await AddPageMenu(context, jobAppMenu, "All Jobs", "Job", "/Jobs/Index", adminsRoleId, 0);
            await AddPageMenu(context, jobAppMenu, "Job Types", "Job", "/JobTypes", adminsRoleId, 0);
            // project management
            // offer management
            // user management
            await context.SaveChangesAsync();
        }

        private static async Task<AppMenu> AddAppMenu(AppDbContext context, string name, string area, string page, Guid roleId, int seqNo)
        {
            var menu = await context.AppMenus.Where(m => m.Name == name).FirstOrDefaultAsync();

            if (menu == null)
            {
                menu = new AppMenu { Name = name, Title = name };
                context.AppMenus.Add(menu);
            }

            menu.Area = area;
            menu.Page = page;
            menu.RoleId = roleId;
            menu.SeqNo = seqNo;

            return menu;
        }

        private static async Task<PageMenu> AddPageMenu(AppDbContext context, AppMenu appMenu, string name, string area, string page, Guid roleId, int seqNo)
        {
            var menu = await context.PageMenus.Where(m => m.Name == name).FirstOrDefaultAsync();

            if (menu == null)
            {
                menu = new PageMenu { Name = name, Title = name };
                context.PageMenus.Add(menu);
            }

            menu.AppMenu = appMenu;
            menu.Area = area;
            menu.Page = page;
            menu.RoleId = roleId;
            menu.SeqNo = seqNo;

            return menu;
        }
        */

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var env = serviceProvider.GetRequiredService<IHostingEnvironment>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var admin = await EnsureUser(context, "Super", config["SUPER_USER"], config["SUPER_PASS"]);

                //await RequiredData(context, admin.Id, adminsRole.Id);

                if (env.IsDevelopment())
                {
                    //await LocalData(context, admin.Id, adminsRole.Id);
                }
            }
        }

        private static async Task<AppUser> EnsureUser(AppDbContext context, string name, string email, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(c => c.Email == email);

            if (user == null)
            {
                user = new AppUser { Email = email, Name = name, Password = password };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            return user;
        }
    }
}