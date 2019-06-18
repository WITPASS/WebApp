using System;

namespace Blazor.Fluxor.Services
{
	/// <see cref="IBrowserInteropService"/>
	public class BrowserInteropPassthrough : IBrowserInteropService
	{
		/// <see cref="IBrowserInteropService.PageLoaded"/>
		public event EventHandler PageLoaded
		{
			add => BrowserInterop.PageLoaded += value;
			remove => BrowserInterop.PageLoaded -= value;
		}

		/// <see cref="IBrowserInteropService.GetClientScripts"/>
		public string GetClientScripts()
		{
			return BrowserInterop.GetClientScripts();
		}
	}
}
