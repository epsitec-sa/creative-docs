namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTable décrit une table dans la base de données. Cette classe
	/// ressemble fortement à System.Data.DataTable.
	/// </summary>
	public class DbTable
	{
		public DbTable()
		{
		}
		
		
		public string							Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public DbColumnCollection				Columns
		{
			get { return this.columns; }
		}
		
		public DbColumn[]						PrimaryKey
		{
			get
			{
				if (this.primary_key == null)
				{
					return new DbColumn[0];
				}
				
				DbColumn[] columns = new DbColumn[this.primary_key.Count];
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
					
					this.primary_key = new DbColumnCollection ();
				}
				
				this.primary_key.Clear ();
				this.primary_key.AddRange (value);
			}
		}
		
		
		protected string						name;
		protected DbColumnCollection			columns = new DbColumnCollection ();
		protected DbColumnCollection			primary_key = null;
	}
}
