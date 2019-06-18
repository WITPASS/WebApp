using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal static class ReducersRegistration
	{
		internal static IEnumerable<DiscoveredReducerInfo> DiscoverReducers(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredReducerInfo> discoveredReducerInfos = allCandidateTypes
				.Select(t => new
				{
					ImplementingType = t,
					GenericParameterTypes = TypeHelper.GetGenericParametersForImplementedInterface(t, typeof(IReducer<>))
				})
				.Where(x => x.GenericParameterTypes != null)
				.Select(x => new DiscoveredReducerInfo(
					implementingType: x.ImplementingType,
					stateType: x.GenericParameterTypes[0]))
				.ToList();

			foreach (DiscoveredReducerInfo discoveredReducerInfo in discoveredReducerInfos)
			{
				RegisterReducer(serviceCollection, discoveredReducerInfo);
			}

			return discoveredReducerInfos;
		}

		private static void RegisterReducer(IServiceCollection serviceCollection, DiscoveredReducerInfo discoveredReducerInfo)
		{
			// Register the feature class
			serviceCollection.AddScoped(serviceType: discoveredReducerInfo.ImplementingType);
		}
	}
}
