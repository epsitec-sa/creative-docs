//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbForeignKey décrit une clef, constituée par une ou deux
	/// colonnes DbColumn.
	/// </summary>
	public class DbForeignKey
	{
		public DbForeignKey(DbColumn a)
		{
			System.Diagnostics.Debug.Assert (a.ParentTableName != null);
			System.Diagnostics.Debug.Assert (a.ParentTableName.Length > 0);
			
			if ((a.ColumnClass == DbColumnClass.RefLiveId) ||
				(a.ColumnClass == DbColumnClass.RefSimpleId))
			{
				this.columns = new DbColumn[] { a };
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid column, DbColumnClass set to {0}.", a.ColumnClass));
			}
		}
		
		public DbForeignKey(DbColumn a, DbColumn b)
		{
			System.Diagnostics.Debug.Assert (a.ParentTableName != null);
			System.Diagnostics.Debug.Assert (a.ParentTableName.Length > 0);
			System.Diagnostics.Debug.Assert (a.ParentTableName == b.ParentTableName);
			
			if ((a.ColumnClass == DbColumnClass.RefTupleId) &&
				(b.ColumnClass == DbColumnClass.RefTupleRevision))
			{
				this.columns = new DbColumn[] { a, b };
			}
			else if ((a.ColumnClass == DbColumnClass.RefTupleRevision) &&
				/**/ (b.ColumnClass == DbColumnClass.RefTupleId))
			{
				this.columns = new DbColumn[] { b, a };
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid columns, DbColumnClass set to {0} and {1}.", a.ColumnClass, b.ColumnClass));
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
		
		public string							ParentTableName
		{
			get
			{
				return this.columns[0].ParentTableName;
			}
		}
		
		
		protected DbColumn[]					columns;
	}
}
