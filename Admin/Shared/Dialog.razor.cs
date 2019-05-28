using Admin.Services;
using Microsoft.AspNetCore.Components;
using System;

namespace Admin.Shared
{
    public class DialogBase : ComponentBase, IDisposable
    {
        [Inject] DialogService DialogSrv { get; set; }

        protected bool IsVisible { get; set; }
        protected string Title { get; set; }
        protected RenderFragment Content { get; set; }

        protected override void OnInit()
        {
            DialogSrv.OnShow += ShowModal;
            DialogSrv.OnClose += CloseModal;
        }

        public void ShowModal(string title, RenderFragment content)
        {
            Title = title;
            Content = content;
            IsVisible = true;

            StateHasChanged();
        }

        public void CloseModal(bool success)
        {
            IsVisible = false;
            Title = "";
            Content = null;

            StateHasChanged();
        }

        public void Dispose()
        {
            DialogSrv.OnShow -= ShowModal;
            DialogSrv.OnClose -= CloseModal;
        }
    }
}