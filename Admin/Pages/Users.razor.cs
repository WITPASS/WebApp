using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Admin.Pages
{
    public class UsersComponent : ComponentBase
    {
        protected AppUser item;
        readonly string endpoint = "users";
        protected IList<AppUser> items = new List<AppUser>();

        [Inject]
        public HttpClient Http { get; set; }

        protected override async Task OnInitAsync()
        {
            items = await Http.GetJsonAsync<IList<AppUser>>(endpoint);
        }

        protected void Add()
        {
            item = new AppUser();
        }

        protected void Edit(AppUser _item)
        {
            item = _item;
        }

        protected async Task CancelAsync()
        {
            if (item.Id == Guid.Empty)
            {
                item = null;
            }
            else
            {
                var __item = await Http.GetJsonAsync<AppUser>($"{endpoint}/{item.Id}");
                items.Insert(items.IndexOf(item), __item);
                items.Remove(item);
            }
        }

        protected async Task DeleteAsync(AppUser item)
        {
            await Http.DeleteAsync($"{endpoint}/{item.Id}");
            items.Remove(item);
        }

        protected async Task SaveAsync()
        {
            if (item.Id == Guid.Empty)
            {
                await Http.PostJsonAsync(endpoint, item);
            }
            else
            {
                await Http.PutJsonAsync($"{endpoint}/{item.Id}", item);
            }

            item = null;
            items = await Http.GetJsonAsync<IList<AppUser>>(endpoint);
        }
    }
}
