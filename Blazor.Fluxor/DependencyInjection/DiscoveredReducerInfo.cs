using System;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class DiscoveredReducerInfo
	{
		public readonly Type ImplementingType;
		public readonly Type StateType;

		public DiscoveredReducerInfo(Type implementingType, Type stateType)
		{
			ImplementingType = implementingType;
			StateType = stateType;
		}
	}
}
