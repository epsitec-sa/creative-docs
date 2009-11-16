//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;

namespace Epsitec.Cresus.Graph
{
	[ServiceContract]
	public interface IConnector
	{
		[OperationContract]
		bool SendData(long handle, string path, string meta, string data);
	}
}
