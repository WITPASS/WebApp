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
        protected ApiService<Role> Api { get; set; }

        protected override async Task OnInitAsync()
        {
            roles = await Api.GetAsync();
        }

        public void Add()
        {
            var role = new Role { __action = "edit" };
            roles.Add(role);
        }

        public void Edit(Role role)
        {
            role.__action = "edit";
        }

        public async Task CancelAsync(Role role)
        {
            if (role.Id == Guid.Empty)
                roles.Remove(role);
            else
            {
                var _role = await Api.GetAsync(role.Id);
                roles.Insert(roles.IndexOf(role), _role);
                roles.Remove(role);
            }
        }

        public async Task DeleteAsync(Role role)
        {
            await Api.DeleteAsync(role.Id);
            roles.Remove(role);
        }

        public async Task SaveAsync(Role role)
        {
            await Api.SaveAsync(role);
            role.__action = null;
        }
    }
}
