//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TextComparison</c> flags are similar to the <see cref="System.StringComparison"/>
	/// enumeration; they are used to specify how texts should be compared by the <see cref="Comparer"/>.
	/// </summary>
	[System.Flags]
	public enum TextComparison
	{
		/// <summary>
		/// Default ordinal comparison.
		/// </summary>
		Default = 0x0000,

		/// <summary>
		/// Ignore the case (internally, characters are first mapped to lower case).
		/// </summary>
		IgnoreCase    = 0x0001,

		/// <summary>
		/// Ignore the accents (internally, characters are stripped from their accents).
		/// </summary>
		IgnoreAccents = 0x0002,
	}
}
