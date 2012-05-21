//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCommandType</c> enumeration describes the different command types;
	/// this allows the database infrastructure to decide which <c>Execute</c> method
	/// to call (<c>ExecuteNonQuery</c>, <c>ExecuteScalar</c>, <c>ExecuteReader</c>,
	/// <c>IDataAdapter.Fill</c>, etc.).
	/// </summary>
	public enum DbCommandType : byte
	{
		/// <summary>
		/// No command defined.
		/// </summary>
		None			= 0,
		
		/// <summary>
		/// Command for which <c>ExecuteNonQuery</c> returns no meaningful value.
		/// </summary>
		Silent			= 1,
		
		/// <summary>
		/// Command for which <c>ExecuteNonQuery</c> returns the number of affected rows.
		/// </summary>
		NonQuery		= 2,
		
		/// <summary>
		/// Command returning data (such as a <c>SELECT</c> statement).
		/// </summary>
		ReturningData	= 3
	}
}
