//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library
{
	public static partial class UI
	{
		public static readonly double RightMargin				= 10;
		public static readonly double MarginUnderLabel			= 1;
		public static readonly double MarginUnderTextField		= 2;
		public static readonly double TinyButtonSize			= 19;  // doit être impair à cause de GlyphButton !
		public static readonly double ComboButtonWidth			= 14;

		public const double ButtonLargeWidth	= 2 * ((UI.IconLargeWidth + 1) / 2 + 5);
		public const double ButtonSmallWidth	= 2 * ((UI.IconSmallWidth + 1) / 2 + 5);

		public const int IconSmallWidth		= 14;
		public const int IconLargeWidth		= 31;
	}
}
