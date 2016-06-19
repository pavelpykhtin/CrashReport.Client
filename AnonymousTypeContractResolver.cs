using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CrashReport.Client
{
	internal class AnonymousTypeContractResolver: DefaultContractResolver
	{
		private readonly Regex _propertyNameRegex;

		public AnonymousTypeContractResolver()
		{
			_propertyNameRegex = new Regex(@"^<(?<name>[^>]*)>.*");
			
		}

		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			if (!type.IsAnonymous())
				return base.CreateProperties(type, memberSerialization);

			var props = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Select(x => CreateProperty(x, memberSerialization))
				.ToList();

			props.ForEach(x =>
			{
				x.PropertyName = _propertyNameRegex.Match(x.PropertyName).Groups["name"].Value;
				x.Readable = true;
				x.Writable = true;
			});

			return props;
		}
	}
}