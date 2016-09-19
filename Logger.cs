using System;
using System.Text;
using CrashReport.Client.Senders;

namespace CrashReport.Client
{
    public class Logger
    {
	    private readonly LoggerSettings _settings;
	    public ISender _sender;

		public Logger(LoggerSettings settings)
		{
			_settings = settings;

			_sender = _settings.IsAsync
				? (ISender)new AsyncSender(_settings.ApplicationKey, _settings.Urls)
				: new SyncSender(_settings.ApplicationKey, _settings.Urls);
		}

		public void Log(Message message)
		{
			message.Version = _settings.Version.ToString();

			_sender.Send(message);
		}

	    public void Log(Exception exception)
	    {
		    var message = BuildMessageFromException(exception);

		    Log(message);
	    }

	    public void Trace(string message)
	    {
		    Log(new Message
		    {
			    LogLevel = ELogLevel.Trace,
				MessageText = message
		    });
		}

		public string FlatternException(Exception exception)
		{
			var innerException = new StringBuilder();

			FlatternException(innerException, exception);

			return innerException.ToString();
		}

		private Message BuildMessageFromException(Exception exception)
	    {
		    return new Message
		    {
			    LogLevel = ELogLevel.Fatal,
				MessageText = exception.Message,
				InnerException = FlatternException(exception),
				StackTrace = exception.StackTrace
		    };
	    }

	    private void FlatternException(StringBuilder result, Exception exception)
	    {
		    if (exception == null)
			    return;

		    if (exception.InnerException == null)
			    return;

		    result.AppendLine("-------Begining of the inner exception-------");

		    result.AppendLine($"{exception.InnerException.Message}\r\nStackTrace:{exception.InnerException.StackTrace}");

			FlatternException(result, exception.InnerException);

			result.AppendLine("---------End of the inner exception----------");
	    }
	}
}
