using Blazor.Fluxor;
using Data;
using Microsoft.JSInterop;

namespace Admin.Store
{
    public class NavState
    {
        internal Branch Branch { get; set; } = new Branch();
        IJSRuntime jsRuntime;
        public NavState(Branch branch) => Branch = branch;
        public NavState()
        {

        }
    }

    public class ActionSetBranch : IAction
    {
        public ActionSetBranch(Branch branch) => Branch = branch;
        internal Branch Branch { get; set; } = null;
    }

    public class ReducerSetBranch : Reducer<NavState, ActionSetBranch>
    {
        readonly IJSRuntime _jsRuntime;
        readonly ApiService _apiService;
        public ReducerSetBranch(IJSRuntime jsRuntime, ApiService apiService)
        {
            _apiService = apiService;
            _jsRuntime = jsRuntime;
        }
        public override NavState Reduce(NavState state, ActionSetBranch action)
        {
            _apiService.SaveLocal("branch", action.Branch);
            return new NavState(action.Branch);
        }
    }
}
