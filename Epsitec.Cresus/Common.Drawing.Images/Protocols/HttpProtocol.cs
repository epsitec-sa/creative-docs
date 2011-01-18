//	Copyright © 2007-2011, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Protocols
{
	internal static class HttpProtocol
	{
		public static byte[] ReadBytes(string name)
		{
			using (System.Net.WebClient client = new System.Net.WebClient ())
			{
				client.Proxy = new System.Net.WebProxy ();
				return client.DownloadData (string.Concat ("http:", name));
			}
		}
	}
}
