using System;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace CrashReport.Client
{
	[Target("CrashReport")]
	public class NLogTarget: TargetWithLayout
	{
		private Logger _logger;
		private readonly LogLevelTranslator _levelTranslator;
		private readonly JsonSerializerSettings _serializationSettings;

		public string Version { get; set; }
		public string VersionFromType { get; set; }
		[RequiredParameter]
		public string ApplicationKey { get; set; }
		[RequiredParameter]
		public string Url { get; set; }
		[DefaultParameter]
		public bool IsAsync { get; set; }

		public NLogTarget()
		{
			_levelTranslator = new LogLevelTranslator();
			_serializationSettings = new JsonSerializerSettings {ContractResolver = new AnonymousTypeContractResolver()};
			

			IsAsync = true;
		}

		protected override void InitializeTarget()
		{
			base.InitializeTarget();

			var urlList = string.IsNullOrWhiteSpace(Url) 
				? Enumerable.Empty<string>() 
				: Url.Split(';');

			urlList = urlList
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToArray();

			_logger = new Logger(new LoggerSettings
			{
				ApplicationKey = ApplicationKey,
				Version = !string.IsNullOrEmpty(Version) 
					? new Version(Version) 
					: Type.GetType(VersionFromType)?.Assembly.GetName().Version,
				Urls = urlList,
				IsAsync = IsAsync
			});
		}

		protected override void Write(LogEventInfo logEvent)
		{
			var parameters = logEvent.Parameters != null && logEvent.Parameters.Length == 1 
				? logEvent.Parameters[0] 
				: new {logEvent.Parameters};
			var additionalInformation = JsonConvert.SerializeObject(
				parameters, 
				_serializationSettings);

			var message = new Message
			{
				MessageText = Layout.Render(logEvent),
				Version = Version,
				LogLevel = _levelTranslator.Translate(logEvent.Level),
				AdditionalInformation = additionalInformation,
				ClientTimeStamp = logEvent.TimeStamp,
				StackTrace = logEvent.Exception?.StackTrace,
				InnerException = logEvent.Exception != null ?_logger.FlatternException(logEvent.Exception) : null
			};

			_logger.Log(message);
		}
	}
}