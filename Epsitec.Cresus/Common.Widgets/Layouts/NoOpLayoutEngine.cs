//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public sealed class NoOpLayoutEngine : ILayoutEngine
	{
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
		{
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
		}
	}
}
