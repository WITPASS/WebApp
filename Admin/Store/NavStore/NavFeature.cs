using Blazor.Fluxor;

namespace Admin.Store
{
    public class NavStateFeature : Feature<NavState>
    {
        public override string GetName() => "NavState";
        protected override NavState GetInitialState() => new NavState();
    }
}
