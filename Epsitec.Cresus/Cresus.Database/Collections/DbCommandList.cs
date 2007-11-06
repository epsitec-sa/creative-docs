//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.DbCommandList</c> class manages a list of <c>IDbCommand</c> items.
	/// </summary>
	public sealed class DbCommandList : GenericList<System.Data.IDbCommand>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbCommandList"/> class.
		/// </summary>
		public DbCommandList()
		{
		}
	}
}
