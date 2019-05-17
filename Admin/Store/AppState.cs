using Blazor.Fluxor;

namespace Admin.Store
{
    public class AppState
    {
        public int ClickCount { get; private set; }

        public AppState(int clickCount)
        {
            ClickCount = clickCount;
        }



    }

    public class IncrementCounterAction : IAction
    {
    }

    public class IncrementCounterReducer : Reducer<AppState, IncrementCounterAction>
    {
        public override AppState Reduce(AppState state, IncrementCounterAction action)
        {
            return new AppState(state.ClickCount + 1);
        }
    }
}
