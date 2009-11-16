//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>ConnectorClient</c> class communicates with the <see cref="ConnectorServer"/>
	/// through a named pipe, in a purely synchronous way.
	/// </summary>
	class ConnectorClient
	{
		public ConnectorClient(System.Diagnostics.Process serverProcess)
		{
			this.serverProcess = serverProcess;
		}

		public bool SendData(System.IntPtr windowHandle, string path, string meta, string data)
		{
			try
			{
				NetNamedPipeBinding binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None);
				EndpointAddress     address = new EndpointAddress (ConnectorServer.GetAddress (this.serverProcess.Id));

				using (ChannelFactory<IConnector> factory = new ChannelFactory<IConnector> (binding, address))
				{
					bool? result  = null;
					int   timeout = 10*1000;
					int   sleep   = 50;

					//	Try connecting to the endpoint; if it does not respond, wait a bit and try
					//	again - the server could be started, but not ready yet to accept data.
					
					for (int i = 0; i < timeout && !result.HasValue; i += sleep)
					{
						IConnector proxy  = factory.CreateChannel ();
						IClientChannel channel = proxy as IClientChannel;

						bool ok = false;
						try
						{
							channel.Open ();
							ok = true;
						}
						catch
						{
							System.Threading.Thread.Sleep (sleep);
						}

						if (ok)
                        {
							using (channel)
							{
								result = proxy.SendData (windowHandle.ToInt64 (), path, meta, data);
							}
						}
					}

					if (result.HasValue)
					{
						return result.Value;
					}
				}
			}
			catch
			{
			}
			
			return false;
		}

		private readonly System.Diagnostics.Process serverProcess;
	}
}
