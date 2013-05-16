using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin.Hosting;
using Epsitec.Cresus.WebCore.Server.Owin.Hubs;

namespace Epsitec.Cresus.WebCore.Server.Owin
{
	class OwinServer : IDisposable
	{

		public void Dispose()
		{
			if (this.owin!=null)
			{
				this.owin.Dispose ();
			}
		}

		public OwinServer(Uri uri)
		{

			this.owin = WebApplication.Start<Startup> (uri.AbsoluteUri);
			
			Console.WriteLine ("Owin Server running at " + uri.AbsoluteUri);
			
            this.hubClient = NotificationClient.Instance;
		}

		private IDisposable owin;
        private NotificationClient hubClient;
	}
}
