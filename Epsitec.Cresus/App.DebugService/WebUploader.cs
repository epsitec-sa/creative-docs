//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.DebugService
{
	public static class WebUploader
	{
		public static string Upload(string uri, byte[] data)
		{
			System.Net.WebRequest request = System.Net.WebRequest.Create (uri);

			request.ContentType = "application/binary";
			request.Method = "POST";
			request.ContentLength = data.Length;
			var stream = request.GetRequestStream ();
			stream.Write (data, 0, data.Length);
			stream.Close ();
			var response = request.GetResponse ();

			if (response == null)
			{
				return null;
			}

			var reader = new System.IO.StreamReader (response.GetResponseStream ());
			return reader.ReadToEnd ().Trim ();

		}
	}
}
