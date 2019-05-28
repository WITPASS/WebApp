using Admin.Shared;
using Microsoft.AspNetCore.Components;
using System;

namespace Admin.Services
{
    public class DialogService
    {
        public event Action<string, RenderFragment> OnShow;
        public event Action<bool> OnClose;

        public void ShowForm(string title, Type form, Guid? id = null)
        {
            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, form);

                if (id != null)
                {
                    x.AddAttribute(2, "Id", id.ToString());
                }

                x.CloseComponent();
            });

            OnShow?.Invoke(title, content);
        }

        public void Show(string title, RenderFragment content)
        {
            OnShow?.Invoke(title, content);
        }

        public void Close(bool success = true)
        {
            OnClose?.Invoke(success);
        }
    }
}