//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Reporting.Settings
{
	/// <summary>
	/// The <c>InclusionMode</c> enumeration defines how values will be included
	/// in an output row (vector) when navigating through the view.
	/// </summary>
	public enum InclusionMode
	{
		/// <summary>
		/// Do not include any value.
		/// </summary>
		None,

		/// <summary>
		/// Include the specified value.
		/// </summary>
		Include,

		/// <summary>
		/// Exclude the specified value.
		/// </summary>
		Exclude,
	}
}
