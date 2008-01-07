//	Copyright © 2007-2008, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Protocols
{
	internal static class FtpProtocol
	{
		public static byte[] ReadBytes(string name)
		{
			using (System.Net.WebClient client = new System.Net.WebClient ())
			{
				return client.DownloadData (string.Concat ("ftp:", name));
			}
		}
	}
}
