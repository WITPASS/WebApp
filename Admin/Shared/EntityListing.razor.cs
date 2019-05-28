using Admin.Services;
using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Shared
{
    public class EntityListingBase<T, T2> : ComponentBase, IDisposable where T : Entity where T2 : ComponentBase
    {
        private readonly string _endpoint;

        public EntityListingBase(string endpoint)
        {
            _endpoint = endpoint;
        }

        protected IList<T> Items { get; set; } = new List<T>();

        [Inject]
        protected ApiService Api { get; set; }

        [Inject]
        protected DialogService Dialog { get; set; }

        protected override async Task OnInitAsync()
        {
            Dialog.OnClose += DialogClosed;
            Items = await Api.GetAsync<T>(_endpoint, GetODataOptions());
        }

        private async void DialogClosed(bool success)
        {
            // if not cancel
            if (success) {
                Items = await Api.GetAsync<T>(_endpoint, GetODataOptions());
                StateHasChanged();
            }
        }

        virtual protected void Add()
        {
            string title = $"Add {typeof(T).Name}";
            Dialog.ShowForm(title, typeof(T2));
        }

        virtual protected void Edit(T _item)
        {
            string title = $"Edit {typeof(T).Name}";
            Dialog.ShowForm(title, typeof(T2), _item.Id);
        }

        virtual protected async Task DeleteAsync(T item)
        {
            await Api.DeleteAsync<T>(_endpoint, item.Id);
            Items.Remove(item);
        }

        virtual protected string GetODataOptions()
        {
            return string.Empty;
        }

        public void Dispose()
        {
            Dialog.OnClose -= DialogClosed;
        }
    }
}