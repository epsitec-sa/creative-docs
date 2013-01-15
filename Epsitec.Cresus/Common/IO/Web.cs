//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Epsitec.Common.IO
{
	public static class Web
	{
		public static IEnumerable<string> DownloadLines(string uri, System.Text.Encoding encoding)
		{
			string value;

			using (WebClient client = new WebClient ())
			{
				byte[] data = client.DownloadData (uri);
				value = encoding.GetString (data);
			}

			return value.Split ('\n').Select (x => x.TrimEnd (' ', '\r'));
		}
	}
}
