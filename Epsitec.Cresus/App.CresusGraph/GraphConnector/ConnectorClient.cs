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
				NetNamedPipeBinding binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None)
				{
					MaxReceivedMessageSize = Connector.MaxMessageSize,
					MaxBufferSize = Connector.MaxMessageSize,
//					TransferMode = System.ServiceModel.TransferMode.Streamed
				};

				var quotas = binding.ReaderQuotas;
				quotas.MaxStringContentLength = Connector.MaxMessageSize;

				EndpointAddress     address = new EndpointAddress (ConnectorServer.GetAddress (this.serverProcess.Id));

				bool? result  = null;
				int   timeout = 30*1000;
				int   sleep   = 50;

				//	Try connecting to the endpoint; if it does not respond, wait a bit and try
				//	again - the server could be started, but not ready yet to accept data.
				
				for (int i = 0; i < timeout && !result.HasValue; i += sleep)
				{
					using (ChannelFactory<IConnector> factory = new ChannelFactory<IConnector> (binding, address))
					{
						IConnector     connector = factory.CreateChannel ();
						IClientChannel channel   = connector as IClientChannel;

						using (channel)
						{
							try
							{
								channel.Open ();
							}
							catch
							{
								channel.Abort ();
								System.Threading.Thread.Sleep (sleep);
								continue;
							}

							result = connector.SendData (windowHandle.ToInt64 (), path, meta, data);
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
