//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	/// <summary>
	/// The <c>OperatorServiceAdapter</c> class implements an adapter for the
	/// <see cref="IOperatorService"/>. See also <see cref="AbstractServiceAdapter"/>.
	/// </summary>
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class OperatorServiceAdapter : AbstractServiceAdapter<IOperatorService>, IOperatorService
	{
		public OperatorServiceAdapter(IOperatorService target)
			: base (target)
		{
		}

		#region IOperatorService Members

		public ProgressInformation CreateRoamingClient(string clientName)
		{
			return this.target.CreateRoamingClient (clientName);
		}

		public bool GetRoamingClientData(long operationId, out ClientIdentity client, out byte[] compressedData)
		{
			return this.target.GetRoamingClientData (operationId, out client, out compressedData);
		}

		#endregion
	}
}
