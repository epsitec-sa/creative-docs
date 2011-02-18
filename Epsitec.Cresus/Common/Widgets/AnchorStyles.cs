//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>AnchorStyles</c> enumeration defines how widgets get
	/// anchored relative to their parent. Several anchoring modes
	/// can be combined together.
	/// </summary>
	[System.Flags]
	public enum AnchorStyles
	{
		/// <summary>
		/// No anchor.
		/// </summary>
		None	= 0x00,
		
		/// <summary>
		/// Top of the widget is anchored to the top of its parent.
		/// </summary>
		Top		= 0x10,
		
		/// <summary>
		/// Bottom of the widget is anchored to the bottom of its parent.
		/// </summary>
		Bottom	= 0x20,
		
		/// <summary>
		/// Left of the widget is anchored to the left of its parent.
		/// </summary>
		Left	= 0x40,
		
		/// <summary>
		/// Right of the widget is anchored to the right of its parent.
		/// </summary>
		Right	= 0x80,

		/// <summary>
		/// Combination of Top and Left.
		/// </summary>
		TopLeft			= Top | Left,
		
		/// <summary>
		/// Combination of Bottom and Left.
		/// </summary>
		BottomLeft		= Bottom | Left,
		
		/// <summary>
		/// Combination of Top and Right.
		/// </summary>
		TopRight		= Top | Right,
		
		/// <summary>
		/// Combination of Bottom and Right.
		/// </summary>
		BottomRight		= Bottom | Right,
		
		/// <summary>
		/// Combination of Left and Right.
		/// </summary>
		LeftAndRight	= Left | Right,
		
		/// <summary>
		/// Combination of Top and Bottom.
		/// </summary>
		TopAndBottom	= Top | Bottom,
		
		/// <summary>
		/// Combination of all modes.
		/// </summary>
		All				= TopAndBottom | LeftAndRight
	}
}
