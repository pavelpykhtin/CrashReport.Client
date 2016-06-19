using System;

namespace CrashReport.Client
{
	public class Message
	{
		public ELogLevel LogLevel { get; set; }
		public string MessageText { get; set; }
		public string StackTrace { get; set; }
		public string AdditionalInformation { get; set; }
		public string InnerException { get; set; }
		public string Version { get; set; }
		public DateTime? ClientTimeStamp { get; set; }
	}
}