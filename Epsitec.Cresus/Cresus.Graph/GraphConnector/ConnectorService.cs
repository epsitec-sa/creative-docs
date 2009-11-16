//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Epsitec.Cresus.Graph
{
	public delegate bool SendDataCallback(System.IntPtr windowHandle, string path, string meta, string data);

	[ServiceBehavior]
	internal class ConnectorService : IConnector
	{
		internal static void DefineSendDataCallback(SendDataCallback callback)
		{
			ConnectorService.sendData = callback;
		}

		#region IConnector Members

		[OperationBehavior]
		bool IConnector.SendData(long windowHandle, string path, string meta, string data)
		{
			if (ConnectorService.sendData == null)
			{
				return false;
			}
			else
            {
				return ConnectorService.sendData (new System.IntPtr (windowHandle), path, meta, data);
            }
		}

		#endregion

		private static SendDataCallback sendData;
	}
}
