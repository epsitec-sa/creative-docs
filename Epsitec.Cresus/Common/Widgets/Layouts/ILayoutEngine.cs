//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>ILayoutEngine</c> interface defines basic layout engine methods used
	/// to measure the minium/maximum constraints based on a visual's children and
	/// then laying out the children based on their measures.
	/// </summary>
	public interface ILayoutEngine
	{
		/// <summary>
		/// Updates the layout of the container, using the specified rectangle
		/// and collection of children.
		/// </summary>
		/// <param name="container">The parent container.</param>
		/// <param name="rect">The parent container rectangle.</param>
		/// <param name="children">The children.</param>
		void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children);

		/// <summary>
		/// Updates the minima and maxima of the container, using the specified
		/// layout context and collection of children.
		/// </summary>
		/// <param name="container">The parent container.</param>
		/// <param name="context">The layout context.</param>
		/// <param name="children">The children.</param>
		/// <param name="minSize">Minimum size.</param>
		/// <param name="maxSize">Maximum size.</param>
		void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize);

		/// <summary>
		/// Gets the layout mode of the layout engine.
		/// </summary>
		/// <value>The layout mode of the layout engine.</value>
		LayoutMode LayoutMode
		{
			get;
		}
	}
}
