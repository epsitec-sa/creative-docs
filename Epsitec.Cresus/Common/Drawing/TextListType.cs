//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>TextListType</c> enumeration defines the type of a text list,
	/// either a glyph based list or a numbered list.
	/// </summary>
	public enum TextListType
	{
		/// <summary>
		/// The text does not belong to a text list.
		/// </summary>
		None,

		/// <summary>
		/// The text belongs to a fixed bullet list, defined using
		/// <see cref="TextListGlyph"/>.
		/// </summary>
		Fixed,
		
		/// <summary>
		/// The text belongs to a numbered list.
		/// </summary>
		Numbered,
	}
}
