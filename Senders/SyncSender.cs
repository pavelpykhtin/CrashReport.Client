using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace CrashReport.Client.Senders
{
	public class SyncSender: ISender
	{
		private readonly FallbackUrlCollection _urls;
		public string ApplicationKey { get; set; }

		public SyncSender(string applicationKey, IEnumerable<string> urls)
		{
			ApplicationKey = applicationKey;
			_urls = new FallbackUrlCollection(urls);
		}

		public void Send(Message message)
		{
			Send(message, 0);
		}

		public void Send(Message message, int fallbackStep)
		{
			var url = $"{_urls.Current}/api/{ApplicationKey}/log";

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ServicePoint.Expect100Continue = false;

			try
			{
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
			catch
			{
				if (_urls.SwitchUrl(fallbackStep))
					Send(message, fallbackStep + 1);
				else
					throw;
			}
		}
	}
}