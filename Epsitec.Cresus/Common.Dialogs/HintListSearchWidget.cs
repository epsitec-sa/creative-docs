//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Dialogs
{
	public class HintListSearchWidget : Widget
	{
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle frame = this.Client.Bounds;
			
			frame.Deflate (0.5);
			
			using (Path path = new Path ())
			{
				path.AppendRoundedRectangle (frame, 6);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Color.FromBrightness (1));

				graphics.Rasterizer.AddOutline (path, 1, CapStyle.Round, JoinStyle.Round);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}
	}
}
