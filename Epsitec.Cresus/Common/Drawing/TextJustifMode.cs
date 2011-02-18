//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>TextJustifMode</c> enumeration defines how text should be
	/// justified.
	/// </summary>
	public enum TextJustifMode : byte
	{
		/// <summary>
		/// The justification mode is not defined.
		/// </summary>
		Undefined,

		/// <summary>
		/// Do not justify anything.
		/// </summary>
		None,
		
		/// <summary>
		/// Justify all lines, but the last one.
		/// </summary>
		AllButLast,
		
		/// <summary>
		/// Justify all lines, including the last one.
		/// </summary>
		All,
	}
}
