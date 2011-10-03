//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.DebugService
{
	/// <summary>
	/// The <c>WebUploader</c> class is used to upload data using an HTTP POST.
	/// </summary>
	public static class WebUploader
	{
		public static string Upload(string uri, byte[] data)
		{
			//	See Scott Hanselman's sample code :
			//	http://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx

			var webRequest = System.Net.WebRequest.Create (uri);

			webRequest.ContentType   = "application/binary";
			webRequest.Method        = "POST";
			webRequest.ContentLength = data.Length;

			using (var requestStream = webRequest.GetRequestStream ())
			{
				requestStream.Write (data, 0, data.Length);
				requestStream.Close ();
			}

			using (var response = webRequest.GetResponse ())
			{
				if (response == null)
				{
					return null;
				}
				
				using (var reader = new System.IO.StreamReader (response.GetResponseStream ()))
				{
					return reader.ReadToEnd ();
				}
			}
		}


		public static readonly string DebugServiceUrl = "http://remotecompta.cresus.ch:8081/debugservice/";
	}
}
