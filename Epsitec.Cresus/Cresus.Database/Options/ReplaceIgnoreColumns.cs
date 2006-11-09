using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Cresus.Database.Options
{
	public class ReplaceIgnoreColumns : IReplaceOptions
	{
		public ReplaceIgnoreColumns()
		{
			this.columns = new System.Collections.Hashtable ();
		}


		public void AddIgnoreColumn(string name, object defaultValue)
		{
			this.columns[name] = defaultValue;
		}


		#region IReplaceOptions Members
		public bool IgnoreColumn(int index, DbColumn column)
		{
			return this.columns.ContainsKey (column.Name);
		}

		public object GetDefaultValue(int index, DbColumn column)
		{
			return this.columns[column.Name];
		}
		#endregion

		System.Collections.Hashtable columns;
	}
}
