using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;

namespace Epsitec.Data.Platform
{
	public class MatchSortWebService
	{
		private MatchSortWebService()
		{
			this.hosting = new NancyHost (new HostConfiguration()
			{
				RewriteLocalhost = false
			}, new Uri("http://localhost:8889/"));
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

		public static MatchSortWebService Current = new MatchSortWebService ();
		private NancyHost hosting;
		public static MatchSortEtl data = MatchSortEtl.Current;
 
	}

}
