namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlTable d�crit une table dans la base de donn�es. Cette classe
	/// ressemble fortement � System.Data.DataTable.
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
				//	NB: les clefs primaires sp�cifi�es par PrimaryKey sont utilis�es
				//	pour former un 'tuple' (par exemple une paire de clef). D�clarer
				//	une s�rie de colonnes comme PrimaryKeys implique que les tuples
				//	doivent �tre uniques !
				
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
				//	Il n'est pas n�cessaire de marquer les colonnes ajout�es ici comme
				//	�tant index�es (SqlColumn.IsIndexed). Si l'appelant le sp�cifie
				//	n�anmoins, des index suppl�mentaires seront cr��s pour les colonnes
				//	sp�cifi�es. Cela permet par exemple d'avoir l'indexage automatique
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
