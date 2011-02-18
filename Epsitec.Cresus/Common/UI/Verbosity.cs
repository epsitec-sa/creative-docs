//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>Verbosity</c> enumeration defines different verbosity modes
	/// used by the <see cref="Placeholder"/> to represent its labels.
	/// </summary>
	[DesignerVisible]
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
