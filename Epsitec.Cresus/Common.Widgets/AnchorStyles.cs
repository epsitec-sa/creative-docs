//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	[System.Flags]
	public enum AnchorStyles
	{
		None	= 0x00,
		Top		= 0x10,
		Bottom	= 0x20,
		Left	= 0x40,
		Right	= 0x80,

		TopLeft			= Top | Left,
		BottomLeft		= Bottom | Left,
		TopRight		= Top | Right,
		BottomRight		= Bottom | Right,
		LeftAndRight	= Left | Right,
		TopAndBottom	= Top | Bottom,
		All				= TopAndBottom | LeftAndRight
	}
}
