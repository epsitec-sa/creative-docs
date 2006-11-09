//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database.Options
{
	/// <summary>
	/// The <c>ReplaceIgnoreColumns</c> class can be used to filter out
	/// columns from updates.
	/// </summary>
	public sealed class ReplaceIgnoreColumns : IReplaceOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReplaceIgnoreColumns"/> class.
		/// </summary>
		public ReplaceIgnoreColumns()
		{
			this.columns = new Dictionary<string, object> ();
		}


		/// <summary>
		/// Adds a column to the ignore list and specifies which default
		/// value to use for it when inserting a row with such a column.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="defaultValue">The default value.</param>
		public void AddIgnoreColumn(string columnName, object defaultValue)
		{
			this.columns[columnName] = defaultValue;
		}


		#region IReplaceOptions Members

		/// <summary>
		/// Verifies if the column should be ignored.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>
		/// 	<c>true</c> if the column should be ignored; otherwise, <c>false</c>.
		/// </returns>
		public bool ShouldIgnoreColumn(DbColumn column)
		{
			return this.columns.ContainsKey (column.Name);
		}

		/// <summary>
		/// Gets the default value for an ignored column.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>The default value.</returns>
		public object GetDefaultValue(DbColumn column)
		{
			return this.columns[column.Name];
		}
		
		#endregion

		private Dictionary<string, object>		columns;
	}
}
