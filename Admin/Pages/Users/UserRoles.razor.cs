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

        public IList<Role> Roles { get; set; }
        public AppUser User { get; set; }

        [Parameter]
        protected string UserId { get; set; }

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            Roles = await Api.GetAsync<Role>("api/roles");
            User = await Api.GetAsync<AppUser>("api/users", new Guid(UserId));
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
