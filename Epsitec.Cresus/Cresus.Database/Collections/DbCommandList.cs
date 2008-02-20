//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
