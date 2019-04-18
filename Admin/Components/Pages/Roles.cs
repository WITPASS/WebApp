using Admin.Services;
using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Components.Pages
{
    public class RolesBase : ComponentBase
    {
        public IList<Role> roles = new List<Role>();

        [Inject]
        protected ApiService Api { get; set; }

        protected override async Task OnInitAsync()
        {
            roles = await Api.GetAsync<Role>("roles");
        }

        public void Add()
        {
            var role = new Role { __tag = "edit" };
            roles.Add(role);
        }

        public void Edit(Role role)
        {
            role.__tag = "edit";
        }

        public async Task CancelAsync(Role role)
        {
            if (role.Id == Guid.Empty)
                roles.Remove(role);
            else
            {
                var _role = await Api.GetAsync<Role>("roles", role.Id);
                roles.Insert(roles.IndexOf(role), _role);
                roles.Remove(role);
            }
        }

        public async Task DeleteAsync(Role role)
        {
            await Api.DeleteAsync("roles", role.Id);
            roles.Remove(role);
        }

        public async Task SaveAsync(Role role)
        {
            await Api.SaveAsync("roles", role);
            role.__tag = null;
        }
    }
}
