//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>FillMode</c> enumeration specifies how intersecting surfaces
	/// are filled.
	/// </summary>
	public enum FillMode
	{
		/// <summary>
		/// Use the even/odd fill rule (for a given point, if moving in a straight
		/// line to infinity, we cross the path an odd number of times, then the
		/// point will be considered as inside the surface).
		/// </summary>
		EvenOdd		= 1,

		/// <summary>
		/// Use the non zero winding rule (a hole must be drawn in the opposite
		/// order as the surrounding path).
		/// </summary>
		NonZero		= 2
	}
}
