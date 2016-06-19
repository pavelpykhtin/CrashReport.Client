using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace CrashReport.Client.Senders
{
	public class SyncSender: ISender
	{
		public string ApplicationKey { get; set; }
		public string Url { get; set; }

		public SyncSender(string applicationKey, string url)
		{
			ApplicationKey = applicationKey;
			Url = url;
		}

		public void Send(Message message)
		{
			var url = $"{Url}/api/{ApplicationKey}/log";

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ServicePoint.Expect100Continue = false;

			using (var stream = request.GetRequestStream())
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
	}
}