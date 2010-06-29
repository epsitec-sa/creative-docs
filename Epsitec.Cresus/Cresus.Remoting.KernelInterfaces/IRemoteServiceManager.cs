//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>IRemoteServiceManager</c> interface is the central entry point used
	/// to start the communication with a remote server.
	/// </summary>
	[ServiceContract]
	public interface IRemoteServiceManager
	{
		/// <summary>
		/// Gets the collection of databases hosted by the server.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		KernelDatabaseInfo[] GetDatabaseInfos();

		/// <summary>
		/// Gets the remote service endpoint address which should be used to talk
		/// to the specified service on the given database.
		/// </summary>
		/// <param name="databaseId">The database id.</param>
		/// <param name="serviceId">The service id.</param>
		/// <returns>The remote service endpoint address.</returns>
		[OperationContract]
		string GetRemoteServiceEndpointAddress(System.Guid databaseId, System.Guid serviceId);

		/// <summary>
		/// Cancels the operation.
		/// </summary>
		/// <param name="operationId">The operation id.</param>
		[OperationContract]
		void CancelOperation(long operationId);

		/// <summary>
		/// Cancels the operation asynchronously.
		/// </summary>
		/// <param name="operationId">The operation id.</param>
		/// <returns>The cancel progress information.</returns>
		[OperationContract]
		ProgressInformation CancelOperationAsync(long operationId);

		/// <summary>
		/// Waits for progress on the specified long running operation.
		/// </summary>
		/// <param name="operationId">The operation id.</param>
		/// <param name="minimumProgress">The minimum progress before a notification should
		/// be issued.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns><c>true</c> if there was progress within the specified time interval; otherwise, <c>false</c>.</returns>
		[OperationContract]
		bool WaitForProgress(long operationId, int minimumProgress, System.TimeSpan timeout);

	}
}
