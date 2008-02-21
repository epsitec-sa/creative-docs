//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>MaskComponent</c> enumeration defines which component to
	/// use in a mask pixmap for masking rendering operations.
	/// </summary>
	public enum MaskComponent
	{
		/// <summary>
		/// Don't use any component.
		/// </summary>
		None	= -1,

		/// <summary>
		/// Use the alpha channel for masking.
		/// </summary>
		A		= 0,
		
		/// <summary>
		/// Use the alpha channel for masking.
		/// </summary>
		Alpha	= 0,

		/// <summary>
		/// Use the red channel for masking.
		/// </summary>
		R		= 1,
		
		/// <summary>
		/// Use the red channel for masking.
		/// </summary>
		Red		= 1,
		
		/// <summary>
		/// Use the green channel for masking.
		/// </summary>
		G		= 2,
		
		/// <summary>
		/// Use the green channel for masking.
		/// </summary>
		Green	= 2,
		
		/// <summary>
		/// Use the blue channel for masking.
		/// </summary>
		B		= 3,
		
		/// <summary>
		/// Use the blue channel for masking.
		/// </summary>
		Blue	= 3
	}
}
