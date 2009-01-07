//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services.Extensions
{
	/// <summary>
	/// The <c>RemoteServiceManagerExtensions</c> class provides extension methods for
	/// the <see cref="IRemoteServiceManager"/> interface.
	/// </summary>
	public static class RemoteServiceManagerExtensions
	{
		/// <summary>
		/// Gets the request execution service.
		/// </summary>
		/// <param name="remoteServiceManager">The remote service manager.</param>
		/// <param name="databaseId">The database id.</param>
		/// <returns>The proxy to the requested service.</returns>
		public static Remoting.IRequestExecutionService GetRequestExecutionService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.RequestExecutionServiceId);
			return Engine.GetService<IRequestExecutionService> (remoteServiceManager, endpointAddress);
		}

		/// <summary>
		/// Gets the connection service.
		/// </summary>
		/// <param name="remoteServiceManager">The remote service manager.</param>
		/// <param name="databaseId">The database id.</param>
		/// <returns>The proxy to the requested service.</returns>
		public static Remoting.IConnectionService GetConnectionService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.ConnectionServiceId);
			return Engine.GetService<IConnectionService> (remoteServiceManager, endpointAddress);
		}

		/// <summary>
		/// Gets the operator service.
		/// </summary>
		/// <param name="remoteServiceManager">The remote service manager.</param>
		/// <param name="databaseId">The database id.</param>
		/// <returns>The proxy to the requested service.</returns>
		public static Remoting.IOperatorService GetOperatorService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.OperatorServiceId);
			return Engine.GetService<IOperatorService> (remoteServiceManager, endpointAddress);
		}

		/// <summary>
		/// Gets the replication service.
		/// </summary>
		/// <param name="remoteServiceManager">The remote service manager.</param>
		/// <param name="databaseId">The database id.</param>
		/// <returns>The proxy to the requested service.</returns>
		public static Remoting.IReplicationService GetReplicationService(this IRemoteServiceManager remoteServiceManager, System.Guid databaseId)
		{
			string endpointAddress = remoteServiceManager.GetRemoteServiceEndpointAddress (databaseId, Epsitec.Cresus.Remoting.RemotingServices.ReplicationServiceId);
			return Engine.GetService<IReplicationService> (remoteServiceManager, endpointAddress);
		}
	}
}
