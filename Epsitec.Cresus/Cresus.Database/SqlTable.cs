//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

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
		
		public SqlTable(string name)
		{
			this.Name = name;
		}
		
		
		public bool Validate(ISqlValidator validator)
		{
			return validator.ValidateName (this.name);
		}
		
		
		public string					Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public SqlColumnCollection		Columns
		{
			get { return this.columns; }
		}
		
		public bool						HasPrimaryKeys
		{
			get { return (this.primary_key != null) && (this.primary_key.Count > 0); }
		}
		
		public SqlColumn[]				PrimaryKey
		{
			get
			{
				//	NB: les clefs primaires spécifiées par PrimaryKey sont utilisées
				//	pour former un 'tuple' (par exemple une paire de clef). Déclarer
				//	une série de colonnes comme PrimaryKey implique que les tuples
				//	doivent être uniques !
				
				if (this.primary_key == null)
				{
					return new SqlColumn[0];
				}
				
				SqlColumn[] columns = new SqlColumn[this.primary_key.Count];
				this.primary_key.CopyTo (columns, 0);
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
				
				if (this.primary_key == null)
				{
					if (value == null)
					{
						return;
					}
					
					this.primary_key = new SqlColumnCollection ();
				}
				
				this.primary_key.Clear ();
				this.primary_key.AddRange (value);
			}
		}
		
		
		protected string				name;
		protected SqlColumnCollection	columns = new SqlColumnCollection ();
		protected SqlColumnCollection	primary_key = null;
	}
}
