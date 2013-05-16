using Epsitec.Common.IO;
using Epsitec.Cresus.WebCore.Server.Owin.Hubs;

using Microsoft.Owin.Hosting;

using System;
using System.Diagnostics;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Owin
{
	internal sealed class OwinServer : IDisposable
	{
		public OwinServer(Uri uri)
		{
			this.owin = WebApplication.Start<Startup> (uri.AbsoluteUri);
			this.hubClient = NotificationClient.Instance;

			OwinServer.CleanTraceListeners ();

			Logger.LogToConsole ("Owin Server started");
		}

		public void Dispose()
		{
			if (this.owin!=null)
			{
				this.owin.Dispose ();
			}
		}

		private static void CleanTraceListeners()
		{
			// In the dll Microsoft.Owin.Hosting, there is the class KatanaEngine which registers a
			// TraceListener with the name "KatanaTraceListener" to the global Trace.Listeners list
			// which is the same list as the Diagnostic.Debug.Listeners list. This means that the
			// debug output is polluted with stuff that we don't want to see.
			// With this method, we remove this listener from the global Trace.Listeners list, so
			// the debug output is clean again.

			var listeners = Trace.Listeners
				.OfType<TraceListener> ()
				.Where (t => t.Name == "KatanaTraceListener")
				.ToList ();

			foreach (var listener in listeners)
			{
				Trace.Listeners.Remove (listener);
			}
		}

		private readonly IDisposable owin;
		private readonly NotificationClient hubClient;
	}
}
