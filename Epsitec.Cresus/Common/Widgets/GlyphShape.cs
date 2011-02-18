//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// GlyphShape détermine l'aspect d'un "glyph" représenté par la classe
	/// GlyphButton.
	/// </summary>
	public enum GlyphShape
	{
		None,
		ArrowUp,
		ArrowDown,
		ArrowLeft,
		ArrowRight,
		TriangleUp,
		TriangleDown,
		TriangleLeft,
		TriangleRight,
		Menu,
		Close,
		Dots,
		Accept,
		Reject,
		TabLeft,
		TabRight,
		TabCenter,
		TabDecimal,
		TabIndent,
		ResizeKnob,
		Plus,
		Minus,
		HorizontalMove,
		VerticalMove,
		Lock,
	}
}
