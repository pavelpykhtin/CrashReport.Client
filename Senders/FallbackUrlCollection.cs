using System;
using System.Collections.Generic;
using System.Linq;

namespace CrashReport.Client.Senders
{
	public class FallbackUrlCollection
	{
		private readonly string[] _urls;
		private int _currentUrlIndex;
		public string Current { get { return _urls[_currentUrlIndex]; } }

		public FallbackUrlCollection(IEnumerable<string> urls)
		{
			if(!urls.Any())
				throw new ArgumentException("Url collection could not be empty");

			_currentUrlIndex = 0;
			_urls = urls.ToArray();
		}
		
		public bool SwitchUrl(int fallbackStep)
		{
			if (fallbackStep + 1 >= _urls.Length)
				return false;

			_currentUrlIndex = (_currentUrlIndex + 1) % _urls.Length;

			return true;
		}
	}
}