using System;
using System.Collections.Generic;

namespace CrashReport.Client
{
	public class LoggerSettings
	{
		public Version Version { get; set; }
		public string ApplicationKey { get; set; }
		public IEnumerable<string> Urls { get; set; }
		public bool IsAsync { get; set; }
	}
}