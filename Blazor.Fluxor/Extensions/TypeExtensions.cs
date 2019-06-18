using System;
using System.Linq;

namespace Blazor.Fluxor.Extensions
{
	internal static class TypeExtensions
	{
		public static string GetNamespace(this Type type)
		{
			string result = string.Join(".", type.FullName.Split('.').Reverse().Skip(1).Reverse());
			return result;
		}
	}
}
