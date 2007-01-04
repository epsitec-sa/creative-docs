//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>Verbosity</c> enumeration defines different verbosity modes
	/// used by the <see cref="Placeholder"/> to represent its labels.
	/// </summary>
	public enum Verbosity
	{
		/// <summary>
		/// No text.
		/// </summary>
		None,

		/// <summary>
		/// Shortest text or label.
		/// </summary>
		Compact,
		
		/// <summary>
		/// Default text or label length.
		/// </summary>
		Default,
		
		/// <summary>
		/// Longest text or label.
		/// </summary>
		Verbose,
	}
}
