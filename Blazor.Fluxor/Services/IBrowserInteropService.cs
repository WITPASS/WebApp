using System;

namespace Blazor.Fluxor.Services
{
	/// <summary>
	/// An interface that provides the required initialisation steps to the browser
	/// </summary>
	public interface IBrowserInteropService
	{
		/// <summary>
		/// <see cref="BrowserInterop.PageLoaded"/>
		/// </summary>
		event EventHandler PageLoaded;
		/// <summary>
		/// <see cref="BrowserInterop.GetClientScripts"/>
		/// </summary>
		/// <returns></returns>
		string GetClientScripts();
	}
}
