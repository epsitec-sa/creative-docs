//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

namespace Epsitec.Cresus.DataLayer.Expressions
{
	/// <summary>
	/// The <c>SortOrder</c> enumeration defines how items are to be sorted when querying
	/// the database. This enumeration contains only two possibilities: ascending and
	/// descending.
	/// </summary>
	public enum SortOrder
	{
		//	Do not add other sort orders in this enumeration, as only the two values
		//	are expected in the database layers :

		Ascending,
		Descending,
	}
}