//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTable d�crit la structure d'une table dans la base de donn�es.
	/// Cette classe ressemble dans l'esprit � System.Data.DataTable.
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
