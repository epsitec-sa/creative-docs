//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataQuerySortOrder</c> enumeration defines the sort order for
	/// a column in a data query (see <see cref="DataQueryColumn"/>).
	/// </summary>
	public enum DataQuerySortOrder
	{
		/// <summary>
		/// The column does not participate in the sort.
		/// </summary>
		None,

		/// <summary>
		/// The column data will be sorted in ascending order.
		/// </summary>
		Ascending,

		/// <summary>
		/// The column data will be sorted in descending order.
		/// </summary>
		Descending
	}
}
