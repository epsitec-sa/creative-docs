//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTransactionMode</c> enumeration defines the transactions
	/// modes supported by <c>DbInfrastructure</c>.
	/// </summary>
	public enum DbTransactionMode
	{
		/// <summary>
		/// Unknown transaction mode.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Read/write transaction.
		/// </summary>
		ReadWrite = 1,

		/// <summary>
		/// Read only transaction.
		/// </summary>
		ReadOnly = 2,
	}
}
