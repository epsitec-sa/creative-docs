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
			return (Remoting.IRequestExecutionService) remoteServiceManager.GetRemoteService (databaseId, Epsitec.Cresus.Remoting.RemotingServices.RequestExecutionServiceId);
		}

		public static Remoting.IConnectionService GetConnectionService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			return (Remoting.IConnectionService) remoteServiceManager.GetRemoteService (databaseId, Epsitec.Cresus.Remoting.RemotingServices.ConnectionServiceId);
		}

		public static Remoting.IOperatorService GetOperatorService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			return (Remoting.IOperatorService) remoteServiceManager.GetRemoteService (databaseId, Epsitec.Cresus.Remoting.RemotingServices.OperatorServiceId);
		}

		public static Remoting.IReplicationService GetReplicationService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			return (Remoting.IReplicationService) remoteServiceManager.GetRemoteService (databaseId, Epsitec.Cresus.Remoting.RemotingServices.ReplicationServiceId);
		}
	}
}
