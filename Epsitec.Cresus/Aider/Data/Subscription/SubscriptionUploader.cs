//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Epsitec.Aider.Data.Subscription
{
	public static class SubscriptionUploader
	{
		public static bool FtpUploadFile(System.IO.FileInfo fileInfo, System.IO.FileInfo responseFileInfo = null)
		{
			var fileName   = SubscriptionUploader.GetTargetFileName ();
			var fileSize   = fileInfo.Length;
			var ftpRequest = FtpWebRequest.Create (SubscriptionUploader.GetFtpUri (fileName)) as FtpWebRequest;

			ftpRequest.Method        = WebRequestMethods.Ftp.UploadFile;
			ftpRequest.Proxy         = new WebProxy ();
			ftpRequest.Credentials   = SubscriptionUploader.GetCredentials ();
			ftpRequest.UseBinary     = true;
			ftpRequest.UsePassive    = true;
			ftpRequest.KeepAlive     = false;
			ftpRequest.ContentLength = fileSize;
			ftpRequest.Timeout       = System.Threading.Timeout.Infinite;

			using (var uploadStream = ftpRequest.GetRequestStream ())
			using (var sourceStream = System.IO.File.OpenRead (fileInfo.FullName))
			{
				System.Console.WriteLine ("FTP: Uploading {0} to {1}, {2} MB", fileInfo.FullName, fileName, fileSize/(1024*1024));
				
				sourceStream.CopyTo (uploadStream);
			}

			using (var response = ftpRequest.GetResponse () as FtpWebResponse)
			{
				System.Console.WriteLine ("FTP: {0}", response.StatusDescription);
				System.Diagnostics.Trace.WriteLine (string.Format ("FTP: {0}", response.StatusDescription));

				if (responseFileInfo != null)
				{
					System.IO.File.WriteAllText (responseFileInfo.FullName, response.StatusDescription);
				}
			}

			return true;
		}
		
		private static string GetTargetFileName()
		{
			var now = System.DateTime.Now;

			return string.Format ("BN_{0:00}{1:00}{2:00}.slf", now.Year % 100, now.Month, now.Day);
		}

		private static System.Uri GetFtpUri()
		{
			return new System.Uri ("ftp://srftp.tamedia.ch/");
		}

		private static System.Uri GetFtpUri(string name)
		{
			return new System.Uri ("ftp://srftp.tamedia.ch/" + name);
		}

		private static NetworkCredential GetCredentials()
		{
			//	Contact person: Fabien Francillon
			//	mailto:informatique.syscom.reseaux@sr.tamedia.ch

			return new NetworkCredential ("epsitec", "PMS61=eg");
		}
	}
}

