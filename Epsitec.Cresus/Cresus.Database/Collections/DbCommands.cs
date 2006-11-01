//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.DbCommands</c> class manages a list of <c>IDbCommand</c> items.
	/// </summary>
	public sealed class DbCommands : GenericList<System.Data.IDbCommand>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbCommands"/> class.
		/// </summary>
		public DbCommands()
		{
		}
	}
}
