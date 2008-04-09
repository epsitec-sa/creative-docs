using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
