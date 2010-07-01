//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>ExecutionState</c> enumeration defines the possible states for
	/// a request stored in the <see cref="ExecutionQueue"/>.
	/// </summary>
	public enum ExecutionState
	{
		/// <summary>
		/// Pending, local execution.
		/// </summary>
		Pending				= 0,

		/// <summary>
		/// Pending, a conflict needs to be resolved.
		/// </summary>
		Conflicting			= 1,

		/// <summary>
		/// Pending, a conflict has been resolved.
		/// </summary>
		ConflictResolved	= 2,

		/// <summary>
		/// Successfully executed by the client.
		/// </summary>
		ExecutedByClient	= 3,

		/// <summary>
		/// Successfully sent to the server.
		/// </summary>
		SentToServer		= 4,

		/// <summary>
		/// Successfully executed by the server.
		/// </summary>
		ExecutedByServer	= 5,

		/// <summary>
		/// Transient, a conflict has been identified on the server.
		/// </summary>
		ConflictingOnServer	= 6,
	}
	
	//	The following transitions are currently supported :
	//
	//		Pending ----------> | -> ExecutedByClient -> SentToServer -> | -> ExecutedByServer
	//		                    |                                        |
	//		ConflictResolved -> | -> Conflicting -> ConflictResolved     | -> ConflictingOnServer -> Conflicting -> ConflictResolved
}
