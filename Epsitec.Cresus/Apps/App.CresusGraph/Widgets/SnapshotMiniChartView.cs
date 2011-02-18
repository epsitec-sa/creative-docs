//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	/// <summary>
	/// The <c>SnapshotMiniChartView</c> class is a specialized <see cref="MiniChartView"/> which is
	/// used to paint miniature snapshots.
	/// </summary>
	public sealed class SnapshotMiniChartView : MiniChartView
	{
		public SnapshotMiniChartView()
		{
			this.DisplayValue = false;
		}

		protected override void PaintTopmostSheetBackground(Graphics graphics, Rectangle rectangle, Color hiliteColor)
		{
			base.PaintTopmostSheetBackground (graphics, rectangle, hiliteColor);
			
			this.AddFrameSurface (graphics, rectangle);
			graphics.RenderSolid (Color.FromBrightness (1));
		}

		protected override void AppendFramePath(Path path, Rectangle rectangle)
		{
			path.AppendRoundedRectangle (rectangle, 3);
		}
	}
}
