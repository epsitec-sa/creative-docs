//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.DbColumns</c> class manages a list of <c>DbColumn</c> items.
	/// </summary>
	public sealed class DbColumns : NameList<DbColumn>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumns"/> class.
		/// </summary>
		public DbColumns()
		{
		}

		/// <summary>
		/// Gets the <see cref="DbColumn"/> with the specified column name and
		/// column class.
		/// </summary>
		/// <value>The <c>DbColumn</c>.</value>
		public DbColumn this[string columnName, DbColumnClass columnClass]
		{
			get
			{
				int index = this.IndexOf (columnName);

				while (index >= 0)
				{
					if (this[index].ColumnClass == columnClass)
					{
						return this[index];
					}

					index = this.IndexOf (columnName, index+1);
				}

				return null;
			}
		}
	}
}
