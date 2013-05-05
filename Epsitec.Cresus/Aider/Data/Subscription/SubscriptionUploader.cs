//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Epsitec.Aider.Data.Subscription
{
	public class SubscriptionUploader
	{
		public SubscriptionUploader()
		{
			//	TODO: upload the file to the Tamedia server
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

