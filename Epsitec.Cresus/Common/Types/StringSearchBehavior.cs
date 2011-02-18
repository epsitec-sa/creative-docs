//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StringSearchBehavior</c> enumeration defines how a string value
	/// should be matched against a reference string.
	/// </summary>
	[DesignerVisible]
	public enum StringSearchBehavior
	{
		/// <summary>
		/// Searches for exact matches.
		/// </summary>
		ExactMatch,

		/// <summary>
		/// Searches for matches using a simple wildcard pattern with "*" and
		/// "?" behaving in the standard way.
		/// </summary>
		WildcardMatch,

		/// <summary>
		/// Searches for matches where both value and reference strings start
		/// with the same text.
		/// </summary>
		MatchStart,

		/// <summary>
		/// Searches for matches where both value and reference strings end
		/// with the same text.
		/// </summary>
		MatchEnd,

		/// <summary>
		/// Searches for matches where the string value is contained anywhere
		/// in the reference string.
		/// </summary>
		MatchAnywhere,
	}
}
