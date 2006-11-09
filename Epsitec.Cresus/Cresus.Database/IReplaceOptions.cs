//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IReplaceOptions</c> interface is used to filter out columns when
	/// updating a table. See <c>DbRichCommand.ReplaceTables</c>.
	/// </summary>
	public interface IReplaceOptions
	{
		/// <summary>
		/// Verifies if the column should be ignored.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns><c>true</c> if the column should be ignored; otherwise, <c>false</c>.</returns>
		bool ShouldIgnoreColumn(DbColumn column);

		/// <summary>
		/// Gets the default value for an ignored column.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>The default value.</returns>
		object GetDefaultValue(DbColumn column);
	}
}
