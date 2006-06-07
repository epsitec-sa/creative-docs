//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>ILayoutEngine</c> defines the interface with a layout engine used
	/// to measure the minium/maximum constraints based on a visual's children and
	/// then laying out the children based on their measures.
	/// </summary>
	public interface ILayoutEngine
	{
		void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children);
		void UpdateMinMax(Visual container, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize);
	}
}
