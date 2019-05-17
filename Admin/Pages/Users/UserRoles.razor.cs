using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Pages
{
    public class UserRolesComponent : BaseComponent<UserRole>
    {
        public UserRolesComponent() : base("api/userroles") { }

        protected IList<Role> Roles { get; set; }
        protected AppUser User { get; set; }
        protected string Title { get; set; } = "User Roles";

        [Parameter]
        protected string UserId { get; set; }

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            Roles = await Api.GetAsync<Role>("api/roles");
            User = await Api.GetAsync<AppUser>("api/users", new Guid(UserId));
            Title = $"{User.Name} Roles";
        }

        protected override string GetListODataOptions()
        {
            return $"$expand=Role($select=Id,Name)&$filter=UserId+eq+{UserId}";
        }

        protected override string GetSingleODataOptions()
        {
            return $"$expand=Role($select=Id,Name)";
        }

        protected override void Add()
        {
            base.Add();
            Item.UserId = new Guid(UserId);
        }
    }
}
