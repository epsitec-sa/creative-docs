//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services
{
	public static class ClientEngine
	{
		public static IRemoteServiceManager GetRemoteServiceManager(string machine, int port)
		{
			System.Uri uri = EngineHost.GetAddress (machine, port, EngineHost.RemoteServiceManagerName);
			return Engine.GetService<IRemoteServiceManager> (uri);
		}
	}
}
