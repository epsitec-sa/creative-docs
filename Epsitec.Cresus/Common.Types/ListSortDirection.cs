//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ListSortDirection</c> enumeration specified the direction of a
	/// sort operation.
	/// </summary>
	public enum ListSortDirection : byte
	{
		/// <summary>
		/// Sort in ascending order.
		/// </summary>
		Ascending,

		/// <summary>
		/// Sort in descending order.
		/// </summary>
		Descending,

		/// <summary>
		/// Don't sort.
		/// </summary>
		None,
	}
}
