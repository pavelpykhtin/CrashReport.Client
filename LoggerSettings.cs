using System;

namespace CrashReport.Client
{
	public class LoggerSettings
	{
		public Version Version { get; set; }
		public string ApplicationKey { get; set; }
		public string Url { get; set; }
		public bool IsAsync { get; set; }
	}
}