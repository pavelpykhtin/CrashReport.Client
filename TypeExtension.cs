using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CrashReport.Client
{
	internal static class TypeExtension
	{
		public static bool IsAnonymous(this Type type)
		{
			var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
			var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
			var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

			return isAnonymousType;
		}
	}
}