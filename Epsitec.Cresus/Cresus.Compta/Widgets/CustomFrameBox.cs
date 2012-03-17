//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class CustomFrameBox : FrameBox
	{
		public CustomFrameBox()
		{
		}


		public bool Hilited
		{
			get
			{
				return this.hilited;
			}
			set
			{
				if (this.hilited != value)
				{
					this.hilited = value;
					this.Invalidate ();
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.BackColor);
			}

			Rectangle rect = this.GetFrameRectangle ();

			if (this.hilited)
			{
#if false
				using (Path path = new Path (rect))
				{
					graphics.PaintDashedOutline (path, 1, 4, 4, CapStyle.Square, Color.FromName ("Red"));
				}
#else
				graphics.LineWidth = 7;

				for (double x = rect.Left-rect.Height; x < rect.Right; x+=20)
				{
					graphics.AddLine (x, rect.Bottom, x+rect.Height, rect.Top);
				}

				graphics.RenderSolid (Color.FromAlphaRgb (0.05, 0, 0, 0));
				//?graphics.RenderSolid (Color.FromAlphaRgb (1.0, 1, 1, 1));
				graphics.LineWidth = 1;
#endif
			}
		}


		private bool			hilited;
	}
}
