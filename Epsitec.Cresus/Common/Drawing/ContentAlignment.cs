//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>ContentAlignment</c> enumeration defines how content should be
	/// aligned in a 2 dimension space.
	/// </summary>
	public enum ContentAlignment : byte
	{
		/// <summary>
		/// No alignment.
		/// </summary>
		None,

		/// <summary>
		/// Bottom left alignment.
		/// </summary>
		BottomLeft,
		
		/// <summary>
		/// Bottom center alignment.
		/// </summary>
		BottomCenter,
		
		/// <summary>
		/// Bottom right alignment.
		/// </summary>
		BottomRight,

		/// <summary>
		/// Middle left alignment.
		/// </summary>
		MiddleLeft,

		/// <summary>
		/// Middle center alignment.
		/// </summary>
		MiddleCenter,

		/// <summary>
		/// Middle right alignment.
		/// </summary>
		MiddleRight,

		/// <summary>
		/// Top left alignment.
		/// </summary>
		TopLeft,

		/// <summary>
		/// Top center alignment.
		/// </summary>
		TopCenter,

		/// <summary>
		/// Top right alignment.
		/// </summary>
		TopRight,

		/// <summary>
		/// Undefined alignement.
		/// </summary>
		Undefined=0xff,
	}
}
