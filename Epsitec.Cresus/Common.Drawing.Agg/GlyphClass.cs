//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>GlyphClass</c> enumeration specifies the class to which a glyph
	/// belongs (space or plain text).
	/// </summary>
	public enum GlyphClass
	{
		/// <summary>
		/// The glyph belongs to the <c>Space</c> class.
		/// </summary>
		Space,
		
		/// <summary>
		/// The glyph belongs to the <c>PlainText</c> class.
		/// </summary>
		PlainText
	}
}
