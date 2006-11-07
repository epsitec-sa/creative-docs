//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.DbColumns</c> class manages a list of <c>DbColumn</c> items.
	/// </summary>
	public class DbColumns : NameList<DbColumn>
	{
		public DbColumns()
		{
		}

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
