using System;
using Epsitec.Data.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace Data.Platform.Tests
{
	[TestClass]
	public class TestMatchSortWS
	{
		[TestMethod]
		public void MatchSortWSAvailable()
		{
			var host = MatchSortWebService.Current;
			host.StartWebService ();

			var result = MakeHttpRequest (new Uri ("http://localhost:8889/WS/MATCHSORT/"), "hello");

			host.StopWebService ();

			StringAssert.Contains ("Mat[CH]Sort Web Service", result);
            
		}


		private string MakeHttpRequest(Uri uri, string text)
		{
			var request = WebRequest.Create (uri);

			var response = request.GetResponse ();

			return this.ReadHttpResponse (response);
		}

		private string ReadHttpResponse(WebResponse response)
		{
			using (var inputStream = response.GetResponseStream ())
			using (var streamReader = new StreamReader (inputStream))
			{
				return streamReader.ReadToEnd ();
			}
		}
	}
}
