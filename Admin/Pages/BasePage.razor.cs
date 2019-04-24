using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Data;

namespace Admin.Pages
{
    public class BaseComponent<T> : ComponentBase where T : Entity
    {
        public BaseComponent(string endpoint)
        {
            _endpoint = endpoint;
        }

        protected T item;
        protected string _endpoint = "users";
        protected IList<T> items = new List<T>();

        [Inject]
        protected HttpClient Http { get; set; }

        protected override async Task OnInitAsync()
        {
            items = await Http.GetJsonAsync<IList<T>>(_endpoint);
        }

        virtual protected void Add()
        {
            item = (T)Activator.CreateInstance(typeof(T));
        }

        virtual protected void Edit(T _item)
        {
            item = _item;
        }

        virtual protected async Task CancelAsync()
        {
            if (item.Id == Guid.Empty)
            {
                item = null;
            }
            else
            {
                var __item = await Http.GetJsonAsync<T>($"{_endpoint}/{item.Id}");
                items.Insert(items.IndexOf(item), __item);
                items.Remove(item);
            }
        }

        virtual protected async Task DeleteAsync(T item)
        {
            await Http.DeleteAsync($"{_endpoint}/{item.Id}");
            items.Remove(item);
        }

        virtual protected async Task SaveAsync()
        {
            if (item.Id == Guid.Empty)
            {
                await Http.PostJsonAsync(_endpoint, item);
            }
            else
            {
                await Http.PutJsonAsync($"{_endpoint}/{item.Id}", item);
            }

            item = null;
            items = await Http.GetJsonAsync<IList<T>>(_endpoint);
        }
    }
}
