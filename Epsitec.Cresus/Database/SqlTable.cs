namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlTable décrit une table dans la base de données. Cette classe
	/// ressemble fortement à System.Data.DataTable.
	/// </summary>
	public class SqlTable
	{
		public SqlTable()
		{
		}
		
		
		public string							Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public SqlColumnCollection				Columns
		{
			get { return this.columns; }
		}
		
		public SqlColumn[]						PrimaryKeys
		{
			get
			{
				//	NB: les clefs primaires spécifiées par PrimaryKey sont utilisées
				//	pour former un 'tuple' (par exemple une paire de clef). Déclarer
				//	une série de colonnes comme PrimaryKeys implique que les tuples
				//	doivent être uniques !
				
				if (this.primary_keys == null)
				{
					return new SqlColumn[0];
				}
				
				SqlColumn[] columns = new SqlColumn[this.primary_keys.Count];
				this.primary_keys.CopyTo (columns, 0);
				return columns;
			}
			set
			{
				//	Il n'est pas nécessaire de marquer les colonnes ajoutées ici comme
				//	étant indexées (SqlColumn.IsIndexed). Si l'appelant le spécifie
				//	néanmoins, des index supplémentaires seront créés pour les colonnes
				//	spécifiées. Cela permet par exemple d'avoir l'indexage automatique
				//	selon le tuple des clefs primaires, et l'indexage de chaque clef
				//	individuellement.
				
				if (this.primary_keys == null)
				{
					if (value == null)
					{
						return;
					}
					
					this.primary_keys = new SqlColumnCollection ();
				}
				
				this.primary_keys.Clear ();
				this.primary_keys.AddRange (value);
			}
		}
		
		
		
		protected string						name;
		protected SqlColumnCollection			columns = new SqlColumnCollection ();
		protected SqlColumnCollection			primary_keys = null;
	}
}
