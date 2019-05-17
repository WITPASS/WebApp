using Blazor.Fluxor;

namespace Admin.Store
{
    public class AppFeature : Feature<AppState>
    {
        public override string GetName() => "AppState";
        protected override AppState GetInitialState() => new AppState(0);
    }
}
