using Blazor.Fluxor;

namespace Admin.Store
{
    public class LoginStateFeature : Feature<LoginState>
    {
        public override string GetName() => "LoginState";
        protected override LoginState GetInitialState() => new LoginState();
    }
}
