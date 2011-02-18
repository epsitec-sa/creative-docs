//	Copyright © 2003-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>IRenderer</c> interface is common to all renderers.
	/// </summary>
	public interface IRenderer
	{
		/// <summary>
		/// Attaches the pixmap to the renderer. All the rendering will go to
		/// the specified pixmap. Setting it to <c>null</c> detaches the pixmap.
		/// </summary>
		/// <value>The pixmap.</value>
		Pixmap Pixmap
		{
			set;
		}

		/// <summary>
		/// Gets the internal AGG handle.
		/// </summary>
		/// <value>The AGG handle.</value>
		System.IntPtr Handle
		{
			get;
		}

		/// <summary>
		/// Sets the alpha mask using the specified 8-bit component of the
		/// mask pixmap. The mask can be reset by specifying a <c>null</c>
		/// pixmap.
		/// </summary>
		/// <param name="maskPixmap">The mask pixmap.</param>
		/// <param name="component">The component.</param>
		void SetAlphaMask(Pixmap maskPixmap, MaskComponent component);
	}
}
