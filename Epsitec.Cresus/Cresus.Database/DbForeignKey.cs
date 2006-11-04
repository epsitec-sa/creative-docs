//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbForeignKey décrit une clef, constituée par une ou plusieurs
	/// colonnes DbColumn.
	/// </summary>
	public class DbForeignKey
	{
		public DbForeignKey(DbColumn a)
		{
			System.Diagnostics.Debug.Assert (a.TargetTableName != null);
			System.Diagnostics.Debug.Assert (a.TargetTableName.Length > 0);
			
			if (a.ColumnClass == DbColumnClass.RefId)
			{
				this.columns = new DbColumn[] { a };
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid column, DbColumnClass set to {0}.", a.ColumnClass));
			}
		}
		
		
		public DbColumn[]						Columns
		{
			get
			{
				DbColumn[] copy = new DbColumn[this.columns.Length];
				this.columns.CopyTo (copy, 0);
				return copy;
			}
		}
		
		public string							TargetTableName
		{
			get
			{
				return this.columns[0].TargetTableName;
			}
		}
		
		
		protected DbColumn[]					columns;
	}
}
