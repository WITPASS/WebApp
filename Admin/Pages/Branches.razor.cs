using Data;
using System;
using System.Threading.Tasks;
using Admin.Shared;

namespace Admin.Pages
{
    public class BranchesComponent : BaseComponent<Branch>
    {
        public BranchesComponent() : base("api/branches") { }

        protected override async Task OnInitAsync()
        {

            await base.OnInitAsync();
        }
    }
}
