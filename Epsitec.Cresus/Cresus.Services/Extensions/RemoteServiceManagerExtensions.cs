//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services.Extensions
{
	public static class RemoteServiceManagerExtensions
	{
		public static Remoting.IRequestExecutionService GetRequestExecutionService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.RequestExecutionServiceId);
			return Engine.GetService<IRequestExecutionService> (remoteServiceManager, endpointAddress);
		}

		public static Remoting.IConnectionService GetConnectionService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.ConnectionServiceId);
			return Engine.GetService<IConnectionService> (remoteServiceManager, endpointAddress);
		}

		public static Remoting.IOperatorService GetOperatorService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.OperatorServiceId);
			return Engine.GetService<IOperatorService> (remoteServiceManager, endpointAddress);
		}

		public static Remoting.IReplicationService GetReplicationService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.ReplicationServiceId);
			return Engine.GetService<IReplicationService> (remoteServiceManager, endpointAddress);
		}
	}
}
