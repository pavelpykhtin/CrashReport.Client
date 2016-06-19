using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CrashReport.Client.Senders
{
	public class AsyncSender: ISender
	{
		private readonly TaskFactory _taskFactory;

		public string ApplicationKey { get; set; }
		public string Url { get; set; }

		public AsyncSender(string applicationKey, string url)
		{
			ApplicationKey = applicationKey;
			Url = url;
			_taskFactory = new TaskFactory();
		}

		public void Send(Message message)
		{
			var url = $"{Url}/api/{ApplicationKey}/log";

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ServicePoint.Expect100Continue = false;

			_taskFactory
				.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null, TaskCreationOptions.None)
				.ContinueWith(task =>
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
				});
		}
	}
}