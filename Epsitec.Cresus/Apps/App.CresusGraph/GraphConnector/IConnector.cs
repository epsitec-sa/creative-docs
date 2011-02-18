//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>IConnector</c> interface defines the service contract for the
	/// WCF pipe-based communication between Graph and its clients.
	/// </summary>
	[ServiceContract]
	public interface IConnector
	{
		/// <summary>
		/// Sends data to the Graph engine.
		/// </summary>
		/// <param name="handle">The window handle of the calling application.</param>
		/// <param name="path">The path to the source document.</param>
		/// <param name="meta">The metadata (or <c>null</c>).</param>
		/// <param name="data">The data (or <c>null</c>).</param>
		/// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
		[OperationContract]
		bool SendData(long handle, string path, string meta, string data);
	}
}
