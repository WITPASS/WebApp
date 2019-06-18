using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal class EffectsRegistration
	{
		internal static IEnumerable<DiscoveredEffectInfo> DiscoverEffects(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredEffectInfo> discoveredEffectInfos = allCandidateTypes
				.Where(t => typeof(IEffect).IsAssignableFrom(t))
				.Select(t => new DiscoveredEffectInfo(implementingType: t))
				.ToList();

			foreach (DiscoveredEffectInfo discoveredEffectInfo in discoveredEffectInfos)
			{
				RegisterEffect(serviceCollection, discoveredEffectInfo);
			}

			return discoveredEffectInfos;
		}

		private static void RegisterEffect(IServiceCollection serviceCollection, DiscoveredEffectInfo discoveredEffectInfo)
		{
			// Register the effect class
			serviceCollection.AddScoped(discoveredEffectInfo.ImplementingType);
		}
	}
}
