//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTable décrit la structure d'une table dans la base de données.
	/// Cette classe ressemble dans l'esprit à System.Data.DataTable.
	/// </summary>
	public class DbTable
	{
		public DbTable()
		{
		}
		
		
		public string					Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		
		protected string				name				= null;
	}
}
