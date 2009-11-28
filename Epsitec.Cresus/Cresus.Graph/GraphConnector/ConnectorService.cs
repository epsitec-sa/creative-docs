//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Epsitec.Cresus.Graph
{
	public delegate bool SendDataCallback(ConnectorData connectorData);

	/// <summary>
	/// The <c>ConnectorService</c> class implements the <see cref="IConnector"/>
	/// service contract.
	/// </summary>
	[ServiceBehavior]
	internal class ConnectorService : IConnector
	{
		/// <summary>
		/// Defines the callback used to send data to the Graph application.
		/// </summary>
		/// <param name="callback">The callback.</param>
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
				return ConnectorService.sendData (new ConnectorData (new System.IntPtr (windowHandle), path, meta, data));
            }
		}

		#endregion

		private static SendDataCallback sendData;
	}
}
