//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public sealed class Globals : AbstractBase
	{
		internal Globals(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.Setup (infrastructure, transaction, "Settings/Globals");
		}
		
		
	}
}
