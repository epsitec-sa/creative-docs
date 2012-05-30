//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class WindowButton : IconButton
	{
		public WindowButton()
		{
		}


		public WindowButtonType WindowButtonType
		{
			get;
			set;
		}


		protected override void PaintBackgroundImplementation(Common.Drawing.Graphics graphics, Common.Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			var rect  = this.Client.Bounds;
			var state = Widget.ConstrainPaintState (this.GetPaintState ());
			var pos   = this.GetTextLayoutOffset ();

			var path = this.GetPath ();

			var surfaceColor = Color.Empty;

			if ((state & WidgetPaintState.Entered) == 0)
			{
				surfaceColor = this.BackColor;
			}
			else
			{
				if (this.WindowButtonType == Widgets.WindowButtonType.Close)
				{
					surfaceColor = Color.FromHexa ("f49786");  // rouge clair
				}
				else
				{
					surfaceColor = Color.FromHexa ("7eafd3");  // bleu clair
				}
			}

			if (!surfaceColor.IsEmpty)
			{
				graphics.Color = surfaceColor;
				graphics.PaintSurface (path);
			}

			graphics.Color = Color.FromHexa ("5b6473");  // gris bleuté foncé
			graphics.PaintOutline (path);

			adorner.PaintButtonTextLayout (graphics, pos, this.TextLayout, state, this.ButtonStyle);
		}

		private Path GetPath()
		{
			var path = new Path ();

			var rect  = this.Client.Bounds;
			rect.Deflate (0.5);

			double r = rect.Height*0.25;

			if (WindowButtonType == Widgets.WindowButtonType.Minimize)  // bouton de gauche ?
			{
				var p1 = rect.TopLeft;
				var p2 = rect.TopRight;
				var p3 = rect.BottomRight;
				var p4 = rect.BottomLeft;

				var p41 = Point.Move (p4, p1, r);
				var p43 = Point.Move (p4, p3, r);

				path.MoveTo (p1);
				path.LineTo (p2);
				path.LineTo (p3);
				path.LineTo (p43);
				path.ArcTo (p4, p41);
				path.Close ();
			}
			else if (WindowButtonType == Widgets.WindowButtonType.Close)  // bouton de droite ?
			{
				var p1 = rect.TopLeft;
				var p2 = rect.TopRight;
				var p3 = rect.BottomRight;
				var p4 = rect.BottomLeft;

				var p32 = Point.Move (p3, p2, r);
				var p34 = Point.Move (p3, p4, r);

				path.MoveTo (p1);
				path.LineTo (p2);
				path.LineTo (p32);
				path.ArcTo (p3, p34);
				path.LineTo (p4);
				path.Close ();
			}
			else  // bouton central ?
			{
				path.AppendRectangle (rect);
			}

			return path;
		}
	}
}
