//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class VerticalInjectionArrow : FrameBox
	{
		public VerticalInjectionArrow()
		{
			this.arrowWidth = 40;
		}

		public double ArrowWidth
		{
			get
			{
				return this.arrowWidth;
			}
			set
			{
				if (this.arrowWidth != value)
				{
					this.arrowWidth = value;
				}
			}
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			var bounds  = Rectangle.Deflate (this.Client.Bounds, new Margins (1, 1, 1, 0));
			var center  = bounds.Center;
			var w = this.ArrowWidth;
			var d = w / 2;

			using (Path path = new Path ())
			{
				path.MoveTo (center.X - d, bounds.Bottom);
				path.LineTo (center.X - d, bounds.Top - d);
				path.LineTo (center.X - w, bounds.Top - d);
				path.LineTo (center.X, bounds.Top);
				path.LineTo (center.X + w, bounds.Top - d);
				path.LineTo (center.X + d, bounds.Top - d);
				path.LineTo (center.X + d, bounds.Bottom);
				path.Close ();

				graphics.Color = this.BackColor;
				graphics.PaintSurface (path);
			}
			
			using (Path path = new Path ())
			{
				path.MoveTo (center.X - d - 0.5, bounds.Bottom);
				path.LineTo (center.X - d - 0.5, bounds.Top - d - 0.5);
				path.LineTo (center.X - w,       bounds.Top - d - 0.5);
				path.LineTo (center.X,           bounds.Top + 0.5);
				path.LineTo (center.X + w,       bounds.Top - d - 0.5);
				path.LineTo (center.X + d + 0.5, bounds.Top - d - 0.5);
				path.LineTo (center.X + d + 0.5, bounds.Bottom);

				graphics.Color = adorner.ColorBorder;
				graphics.PaintOutline (path);
			}
		}

		private double arrowWidth;
	}
}
