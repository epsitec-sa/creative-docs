//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>CapStyle</c> enumeration defines the style used to paint the
	/// line caps (i.e. the extremities of an open path).
	/// </summary>
	public enum CapStyle
	{
		/// <summary>
		/// Butt cap, the line starts and stops at the exact coordinates.
		/// </summary>
		Butt   = 0,

		/// <summary>
		/// Square cap, the line extends by half its width the start and stop
		/// coordinates.
		/// </summary>
		Square = 1,

		/// <summary>
		/// Round cap, the line starts and stops with a rounded extremity, with
		/// a radius equal to the half of its width.
		/// </summary>
		Round  = 2
	}
}
