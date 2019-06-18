using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	/// <summary>
	/// A generic class that can be used as a base for effects.
	/// </summary>
	/// <typeparam name="TTriggerAction"></typeparam>
	public abstract class Effect<TTriggerAction> : IEffect
		where TTriggerAction : IAction
	{
		/// <summary>
		/// <see cref="IEffect.HandleAsync(IAction, IDispatcher)"/>
		/// </summary>
		protected abstract Task HandleAsync(TTriggerAction action, IDispatcher dispatcher);

		/// <summary>
		/// <see cref="IEffect.ShouldReactToAction(IAction)"/>
		/// </summary>
		public bool ShouldReactToAction(IAction action)
		{
			return typeof(TTriggerAction).IsAssignableFrom(action.GetType());
		}

		Task IEffect.HandleAsync(IAction action, IDispatcher dispatcher)
		{
			return HandleAsync((TTriggerAction)action, dispatcher);
		}
	}
}
