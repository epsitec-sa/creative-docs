//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class DataCubeButton : Button
	{
		public DataCubeButton()
		{
			this.AutoFocus = false;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			var rect  = this.Client.Bounds;
			var state = Widget.ConstrainPaintState (this.GetPaintState ());

			if ((state & WidgetPaintState.Enabled) == 0)
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}

			if ((state & WidgetPaintState.Selected) != 0)
			{
				//	Force a selected button to paint like a passively enabled one, since the
				//	adornment is painted in a fully different way afterwards on the foreground
				//	paint pass :

				double ox = 2;
				double oy = 4;
				double dx = rect.Width - ox;
				double dy = rect.Height - 2;
				double r  = 4;

				using (Path path = new Path ())
				{
					path.MoveTo (dx-0.5, oy);
					path.LineTo (dx-0.5, dy-0.5-r);
					path.ArcTo (dx-0.5, dy-0.5, dx-0.5-r, dy-0.5);
					path.LineTo (ox+0.5+r, dy-0.5);
					path.ArcTo (ox+0.5, dy-0.5, ox+0.5, dy-0.5-r);
					path.LineTo (ox+0.5, oy);

					graphics.Rasterizer.AddOutline (path, 5, CapStyle.Butt, JoinStyle.Round);
					graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0.0, 0.0));

					graphics.Rasterizer.AddOutline (path, 3, CapStyle.Butt, JoinStyle.Round);
					graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0.0, 0.0));

					path.Close ();

					graphics.LineJoin = JoinStyle.Round;
					graphics.Rasterizer.AddSurface (path);
					graphics.RenderSolid (Color.FromBrightness (1));
				}
			}
			else
			{
				adorner.PaintButtonBackground (graphics, rect, state, Direction.Down, ButtonStyle.ToolItem);
			}

			var image = ImageProvider.Default.GetImage ("manifest:Epsitec.Cresus.Graph.Images.DataCube.icon", Resources.DefaultManager);
			var bounds = new Rectangle (System.Math.Floor ((rect.Width - image.Size.Width) / 2),
										System.Math.Floor ((rect.Height - image.Size.Height) / 2),
										image.Size.Width, image.Size.Height);

			if ((state & WidgetPaintState.Engaged) != 0)
			{
				bounds = Rectangle.Offset (bounds, 1, -1);
			}

			graphics.PaintImage (image, bounds);
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ((this.GetPaintState () & WidgetPaintState.Selected) == 0)
			{
				return;
			}

			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			var rect    = this.Client.Bounds;

			double ox = 2;
			double oy = 4;
			double dx = rect.Width - ox;
			double dy = rect.Height - 2;
			double r  = 4;

			using (Path path = new Path ())
			{
				path.MoveTo (dx-0.5, oy);
				path.LineTo (dx-0.5, dy-0.5-r);
				path.ArcTo (dx-0.5, dy-0.5, dx-0.5-r, dy-0.5);
				path.LineTo (ox+0.5+r, dy-0.5);
				path.ArcTo (ox+0.5, dy-0.5, ox+0.5, dy-0.5-r);
				path.LineTo (ox+0.5, oy);

				graphics.Rasterizer.AddOutline (path, 1, CapStyle.Butt, JoinStyle.Round);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}
	}
}
