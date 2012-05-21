//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbForeignKey</c> structure represents a foreign key, which maps
	/// to one or several <c>DbColumn</c>s.
	/// </summary>
	public struct DbForeignKey
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbForeignKey"/> structure.
		/// </summary>
		/// <param name="column">The single column.</param>
		public DbForeignKey(DbColumn column)
		{
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (column.TargetTableName));
			
			if (column.ColumnClass == DbColumnClass.RefId)
			{
				this.columns = new DbColumn[] { column };
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid column, DbColumnClass set to {0}.", column.ColumnClass));
			}
		}

		/// <summary>
		/// Gets the columns which encode the foreign key.
		/// </summary>
		/// <value>The columns which encode the foreign key.</value>
		public DbColumn[]						Columns
		{
			get
			{
				if (this.columns == null)
				{
					return new DbColumn[0];
				}
				else
				{
					DbColumn[] copy = new DbColumn[this.columns.Length];
					this.columns.CopyTo (copy, 0);
					return copy;
				}
			}
		}

		/// <summary>
		/// Gets the name of the target table.
		/// </summary>
		/// <value>The name of the target table.</value>
		public string							TargetTableName
		{
			get
			{
				if (this.columns == null)
				{
					return null;
				}
				else
				{
					return this.columns[0].TargetTableName;
				}
			}
		}
		
		
		private DbColumn[]						columns;
	}
}
