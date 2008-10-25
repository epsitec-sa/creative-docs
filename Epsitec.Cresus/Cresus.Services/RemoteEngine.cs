//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	public class RemoteEngine
	{
		public RemoteEngine()
		{
		}




		public IRemoteServiceManager GetRemoteKernel(string machine, int port)
		{
			if (this.remoteServiceManager == null)
			{
				string      url  = string.Concat ("http://", this.remoteHost, ":", this.remotePort.ToString (System.Globalization.CultureInfo.InvariantCulture), "/", Engine.RemoteServiceManagerServiceName);
				System.Type type = typeof (Remoting.IRemoteServiceManager);

				this.remoteServiceManager = (Remoting.IRemoteServiceManager) System.Activator.GetObject (type, url);
			}

			return this.remoteServiceManager;
		}


		string remoteHost;
		int remotePort;
		IRemoteServiceManager remoteServiceManager;
	}
}
