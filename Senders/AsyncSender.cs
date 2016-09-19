using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CrashReport.Client.Senders
{
	public class AsyncSender: ISender
	{
		private readonly TaskFactory _taskFactory;
		private readonly FallbackUrlCollection _urls;

		public string ApplicationKey { get; set; }

		public AsyncSender(string applicationKey, IEnumerable<string> urls)
		{
			ApplicationKey = applicationKey;
			_urls = new FallbackUrlCollection(urls);
			_taskFactory = new TaskFactory();
		}

		public void Send(Message message)
		{
			Send(message, 0);
		}

		private void Send(Message message, int fallbackStep)
		{
			var url = $"{_urls.Current}/api/{ApplicationKey}/log";

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ServicePoint.Expect100Continue = false;

			_taskFactory
				.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null, TaskCreationOptions.None)
				.ContinueWith(task =>
				{
					try
					{
						using (var stream = task.Result)
						{
							using (var writer = new StreamWriter(stream))
							{
								var serializer = new JsonSerializer();

								serializer.Serialize(writer, message);
							}

							stream.Close();
						}

						request.GetResponse();
					}
					catch
					{
						if (_urls.SwitchUrl(fallbackStep))
							Send(message, fallbackStep + 1);
						else
							throw;
					}
				});
		}
	}
}