//	Copyright © 2004-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ValidationState</c> enumeration lists all possible states for an
	/// <see cref="IValidationResult"/>.
	/// </summary>
	public enum ValidationState
	{
		/// <summary>
		/// Unknown state (not defined).
		/// </summary>
		Unknown,

		/// <summary>
		/// Valid state (correct data found).
		/// </summary>
		Ok,

		/// <summary>
		/// Error state (incorrect or invalid data found).
		/// </summary>
		Error,

		/// <summary>
		/// Warning state (possibly incorrect or invalid data found).
		/// </summary>
		Warning,

		/// <summary>
		/// Dirty state (needs a validation to find out what the state is).
		/// </summary>
		Dirty,
	}
}
