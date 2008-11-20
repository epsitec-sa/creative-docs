//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	[ServiceContract]
	public interface IRemoteServiceManager
	{
		[OperationContract]
		KernelDatabaseInfo[] GetDatabaseInfos();

		[OperationContract]
		string GetRemoteServiceEndpointAddress(System.Guid databaseId, System.Guid serviceId);

		[OperationContract]
		void CancelOperation(long operationId);
		
		[OperationContract]
		void CancelOperationAsync(long operationId, out ProgressInformation cancelProgressInformation);

		[OperationContract]
		bool WaitForProgress(long operationId, int minimumProgress, System.TimeSpan timeout);

	}
}
