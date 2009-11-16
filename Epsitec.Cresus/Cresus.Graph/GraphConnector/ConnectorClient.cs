//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.InteropServices;

namespace Epsitec.Cresus.Graph
{
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
					IConnector proxy = factory.CreateChannel ();

					using (IClientChannel channel = proxy as IClientChannel)
					{
						return proxy.SendData (windowHandle.ToInt64 (), path, meta, data);
					}
				}
			}
			catch
			{
				return false;
			}
		}

		private readonly System.Diagnostics.Process serverProcess;
	}
}
