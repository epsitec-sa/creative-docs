//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	public class EngineHost : System.MarshalByRefObject, System.IDisposable
	{
		public EngineHost(int portNumber)
		{
			this.portNumber = portNumber;
			this.serviceManager = new RemotingServiceManager ();

			this.RegisterChannel ();
			this.RegisterService ();
		}

		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion

		public void AddEngine(Engine engine)
		{
			this.serviceManager.AddEngine (engine);
		}

		
		private void RegisterChannel()
		{
			System.Collections.Hashtable setup = new System.Collections.Hashtable ();
			
			setup["port"] = this.portNumber;
			
			SoapServerFormatterSinkProvider sinkProvider = new SoapServerFormatterSinkProvider ();
			sinkProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			
			HttpChannel channel = new HttpChannel (setup, null, sinkProvider);
			ChannelServices.RegisterChannel (channel);
		}

		private void RegisterService()
		{
			RemotingServices.Marshal (this.serviceManager, EngineHost.RemoteServiceManagerServiceName);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				RemotingServices.Disconnect (this.serviceManager);
				this.serviceManager.Dispose ();
			}
		}

		public static readonly string				RemoteServiceManagerServiceName = "RemoteServiceManager.soap";

		readonly int								portNumber;
		readonly RemotingServiceManager				serviceManager;
	}
}
