//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		
		public SqlTable(string name)
		{
			this.Name = name;
		}
		
		
		public string							Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public Collections.SqlColumns			Columns
		{
			get { return this.columns; }
		}
		
		public bool								HasPrimaryKey
		{
			get { return (this.primary_key != null) && (this.primary_key.Count > 0); }
		}
		
		public SqlColumn[]						PrimaryKey
		{
			get
			{
				//	NB: les clefs primaires sp�cifi�es par PrimaryKey sont utilis�es
				//	pour former un 'tuple' (par exemple une paire de clef). D�clarer
				//	une s�rie de colonnes comme PrimaryKey implique que les tuples
				//	doivent �tre uniques !
				
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
				//	Il n'est pas n�cessaire de marquer les colonnes ajout�es ici comme
				//	�tant index�es.
				
				if (this.primary_key == null)
				{
					if (value == null)
					{
						return;
					}
					
					this.primary_key = new Collections.SqlColumns ();
				}
				
				this.primary_key.Clear ();
				this.primary_key.AddRange (value);
			}
		}
		
		
		public bool Validate(ISqlValidator validator)
		{
			return validator.ValidateName (this.name);
		}
		
		
		protected string						name;
		protected Collections.SqlColumns		columns		= new Collections.SqlColumns ();
		protected Collections.SqlColumns		primary_key;
	}
}
