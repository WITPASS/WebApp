using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class LocalStorageService
    {
        private readonly IJSRuntime jSRuntime;

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public Task<string> GetItem(string key)
        {
            return jSRuntime.InvokeAsync<string>("localStorageGetItem", key);
        }

        public Task SetItem(string key, string value)
        {
            return jSRuntime.InvokeAsync<object>("localStorageSetItem", key, value);
        }

        public Task RemoveItem(string key)
        {
            return jSRuntime.InvokeAsync<object>("localStorageRemoveItem", key);
        }
    }
}
