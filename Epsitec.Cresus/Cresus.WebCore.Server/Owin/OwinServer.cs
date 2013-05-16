using Epsitec.Common.IO;
using Epsitec.Cresus.WebCore.Server.Owin.Hubs;

using Microsoft.Owin.Hosting;

using System;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Owin
{
	internal sealed class OwinServer : IDisposable
	{
		public OwinServer(Uri uri)
		{
			this.owin = WebApplication.Start<Startup> (uri.AbsoluteUri);
			this.hubClient = NotificationClient.Instance;

			Logger.LogToConsole ("Owin Server started");
		}

		public void Dispose()
		{
			if (this.owin!=null)
			{
				this.owin.Dispose ();
			}
		}

		private readonly IDisposable owin;
		private readonly NotificationClient hubClient;
	}
}
