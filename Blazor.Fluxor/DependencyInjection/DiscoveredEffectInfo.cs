using System;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class DiscoveredEffectInfo
	{
		public readonly Type ImplementingType;

		public DiscoveredEffectInfo(Type implementingType)
		{
			ImplementingType = implementingType;
		}
	}
}
