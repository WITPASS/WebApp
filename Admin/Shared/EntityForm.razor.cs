using Admin.Services;
using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Admin.Shared
{
    public class EntityFormBase<T> : ComponentBase where T : Entity
    {
        private readonly string _endpoint;

        public EntityFormBase(string endpoint)
        {
            _endpoint = endpoint;
        }

        // Item will be passed to EntityForm.
        protected T Item { get; set; }

        // Id will be merged with EntityForm. See RoleForm and RoleFormBase
        [Parameter]
        protected string Id { get; set; }

        [Inject]
        protected ApiService Api { get; set; }
        [Inject]
        protected DialogService Dialog { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Id) && Guid.TryParse(Id, out Guid _id))
            {
                Item = await Api.GetAsync<T>(_endpoint, _id, GetODataSelectExpand());
            }
            else
            {
                Item = (T)Activator.CreateInstance(typeof(T));
            }
        }

        virtual protected async Task CancelAsync()
        {
            if (Item.Id != Guid.Empty)
            {
                Item = await Api.GetAsync<T>(_endpoint, Item.Id, GetODataSelectExpand());
            }

            Dialog.Close(false);
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

            Dialog.Close(true);
        }

        virtual protected string GetODataSelectExpand()
        {
            return string.Empty;
        }
    }
}