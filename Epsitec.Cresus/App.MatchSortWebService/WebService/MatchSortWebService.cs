using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;
using Epsitec.Data.Platform;
using System.IO;

namespace App.MatchSortWebService
{
	public class MatchSortWebService
	{
		public MatchSortWebService(string uri)
		{
			this.hosting = new NancyHost (new HostConfiguration()
			{
				RewriteLocalhost = false
			},new Uri(uri));
		}

		public void StartWebService()
		{
			if (this.hosting!=null)
			{
				this.hosting.Start ();
			}
		}

		public void StopWebService()
		{
			if(this.hosting!=null)
			{
				this.hosting.Stop ();
			}
		}

		private NancyHost hosting;
		//public static MatchSortEtl data = new MatchSortEtl (Path.GetDirectoryName (Application.ExecutablePath), @"s:/MAT[CH]news.csv", false);
 
	}

}
