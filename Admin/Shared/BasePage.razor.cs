using Admin.Services;
using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Shared
{
    public class BaseComponent<T> : ComponentBase where T : Entity
    {
        public BaseComponent(string endpoint)
        {
            _endpoint = endpoint;
        }

        private readonly string _endpoint;
        protected T Item { get; set; }
        protected IList<T> Items { get; set; } = new List<T>();

        [Inject]
        protected ApiService Api { get; set; }

        protected override async Task OnInitAsync()
        {
            Items = await Api.GetAsync<T>(_endpoint, GetListODataOptions());
        }

        virtual protected void Add()
        {
            Item = (T)Activator.CreateInstance(typeof(T));
        }

        virtual protected void Edit(T _item)
        {
            Item = _item;
        }

        virtual protected async Task CancelAsync()
        {
            if (Item.Id != Guid.Empty)
            {
                var item = await Api.GetAsync<T>(_endpoint, Item.Id, GetSingleODataOptions());
                Items.Insert(Items.IndexOf(Item), item);
                Items.Remove(Item);
            }

            Item = null;
        }

        virtual protected async Task DeleteAsync(T item)
        {
            await Api.DeleteAsync<T>(_endpoint, item.Id);
            Items.Remove(item);
        }

        virtual protected async Task SaveAsync()
        {
            if (Item.Id == Guid.Empty)
            {
                await Api.PostAsync(_endpoint, Item);
            }
            else
            {
                await Api.PutAsync(_endpoint, Item.Id, Item);
            }

            Item = null;
            Items = await Api.GetAsync<T>(_endpoint, GetListODataOptions());
        }

        virtual protected string GetListODataOptions()
        {
            return string.Empty;
        }

        virtual protected string GetSingleODataOptions()
        {
            return string.Empty;
        }
    }
}