using System;
using System.Collections.Generic;
using NLog;

namespace CrashReport.Client
{
	internal class LogLevelTranslator
	{
		private readonly Dictionary<string, ELogLevel> _levelMatchTable;

		public LogLevelTranslator()
		{
			_levelMatchTable = new Dictionary<string, ELogLevel>
			{
				{LogLevel.Error.Name, ELogLevel.Error},
				{LogLevel.Debug.Name, ELogLevel.Debug},
				{LogLevel.Fatal.Name, ELogLevel.Fatal},
				{LogLevel.Info.Name, ELogLevel.Info},
				{LogLevel.Trace.Name, ELogLevel.Trace},
				{LogLevel.Warn.Name, ELogLevel.Warn}
			};
		}

		public ELogLevel Translate(LogLevel src)
		{
			if(_levelMatchTable.ContainsKey(src.Name))
				return _levelMatchTable[src.Name];

			throw new ArgumentException($"Unknown log level '{src.Name}'");
		}
	}
}