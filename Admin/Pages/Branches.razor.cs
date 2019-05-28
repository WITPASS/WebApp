using Data;
using System;
using System.Threading.Tasks;
using Admin.Shared;

namespace Admin.Pages
{
    public class BranchesComponent : BaseComponent<Branch>
    {
        public BranchesComponent() : base("api/branches") { }

        internal Guid BranchId { get; private set; }
        protected override async Task OnInitAsync()
        {
            BranchId = Api.BranchId;

            await base.OnInitAsync();
        }

        protected void Select(Branch branch)
        {
            Api.BranchId = BranchId = branch.Id;
            Api.Branch = branch.Name;
            Api.LoginInfoHasChanaged();
        }
    }
}
