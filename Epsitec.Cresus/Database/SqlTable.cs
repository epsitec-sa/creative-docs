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
		
		public SqlColumn[]						PrimaryKey
		{
			get
			{
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
		
		
		protected string						name;
		protected SqlColumnCollection			columns = new SqlColumnCollection ();
		protected SqlColumnCollection			primary_key = null;
	}
}
