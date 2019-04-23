using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Admin.Pages
{
    public class RolesComponent : ComponentBase
    {
        readonly string endpoint = "roles";
        protected IList<Role> items = new List<Role>();

        [Inject]
        public HttpClient Http { get; set; }

        protected override async Task OnInitAsync()
        {
            items = await Http.GetJsonAsync<IList<Role>>(endpoint);
        }

        protected void Add()
        {
            var item = new Role { __tag = "edit" };
            items.Add(item);
        }

        protected void Edit(Role item)
        {
            item.__tag = "edit";
        }

        protected async Task CancelAsync(Role item)
        {
            if (item.Id == Guid.Empty)
            {
                items.Remove(item);
            }
            else
            {
                var _item = await Http.GetJsonAsync<Role>($"{endpoint}/{item.Id}");
                items.Insert(items.IndexOf(item), _item);
                items.Remove(item);
            }
        }

        protected async Task DeleteAsync(Role item)
        {
            await Http.DeleteAsync($"{endpoint}/{item.Id}");
            items.Remove(item);
        }

        protected async Task SaveAsync(Role item)
        {
            if (item.Id == Guid.Empty)
            {
                await Http.PostJsonAsync(endpoint, item);
            }
            else
            {
                await Http.PutJsonAsync($"{endpoint}/{item.Id}", item);
            }

            item.__tag = null;
        }
    }
}
