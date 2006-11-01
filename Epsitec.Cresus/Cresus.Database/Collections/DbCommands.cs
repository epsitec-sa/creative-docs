//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	using IDbCommand = System.Data.IDbCommand;
	
	/// <summary>
	/// La classe Collections.DbCommands encapsule une collection d'instances de type IDbCommand.
	/// </summary>
	public class DbCommands : GenericList<IDbCommand>
	{
		public DbCommands()
		{
		}
	}
}
