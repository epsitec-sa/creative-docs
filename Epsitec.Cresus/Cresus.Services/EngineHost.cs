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
		public EngineHost(int port_number)
		{
			this.port_number    = port_number;
			this.services       = new List<System.MarshalByRefObject> ();
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion
		
		public void RegisterChannel()
		{
			System.Collections.Hashtable setup = new System.Collections.Hashtable ();
			
			setup["port"] = this.port_number;
			
			SoapServerFormatterSinkProvider sink_provider = new SoapServerFormatterSinkProvider ();
			sink_provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			
			HttpChannel channel = new HttpChannel (setup, null, sink_provider);
			ChannelServices.RegisterChannel (channel);
			this.isChannelRegistered = true;
		}

		public void RegisterService(string name, System.MarshalByRefObject service)
		{
			if (this.isChannelRegistered == false)
			{
				this.RegisterChannel ();
			}

			RemotingServices.Marshal (service, name);

			this.services.Add (service);
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.services.Count > 0)
				{
					System.MarshalByRefObject[] array = this.services.ToArray ();

					foreach (var item in array)
					{
						RemotingServices.Disconnect (item);

						System.IDisposable disposable = item as System.IDisposable;
						
						if (disposable != null)
						{
							disposable.Dispose ();
						}
					}
					
					this.services.Clear ();
				}
			}
		}
		
		private int								port_number;
		private List<System.MarshalByRefObject>	services;
		private bool							isChannelRegistered;
	}
}
