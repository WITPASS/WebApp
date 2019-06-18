using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class AssemblyScanSettings
	{
		public readonly Assembly Assembly;
		public readonly string Namespace;

		public AssemblyScanSettings(Assembly assembly) : this(assembly, null) { }
		public bool Matches(Type type) =>
			type.Assembly == Assembly
			&& (Namespace == null || type.FullName.StartsWith(Namespace + "."));


		public AssemblyScanSettings(Assembly assembly, string @namespace)
		{
			Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			Namespace = @namespace;
		}

		public static IEnumerable<Type> Filter(IEnumerable<Type> types, IEnumerable<AssemblyScanSettings> scanBlacklist, 
			IEnumerable<AssemblyScanSettings> scanWhitelist)
		{
			return types
				.Where(t => scanWhitelist.Any(wl => wl.Matches(t)) || !scanBlacklist.Any(bl => bl.Matches(t)));
		}

		public override bool Equals(object obj)
		{
			AssemblyScanSettings other = obj as AssemblyScanSettings;
			if (other == null)
				return false;

			return other.Assembly.FullName == Assembly.FullName && other.Namespace == Namespace;
		}

		public override int GetHashCode()
		{
			return (Assembly.FullName + "/" + Namespace).GetHashCode();
		}

	}
}
