//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public LayoutMode LayoutMode
		{
			get
			{
				return LayoutMode.None;
			}
		}
	}
}
