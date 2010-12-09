//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class VStaticText: StaticText
	{
		public VStaticText()
		{
		}

		public VStaticText(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner          adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle         rect    = this.Client.Bounds;
			WidgetPaintState  state   = this.GetPaintState ();
			Point             pos     = Point.Zero;

			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}

			if (this.TextLayout != null)
			{
				var iTransform = graphics.Transform;

				graphics.RotateTransformDeg (90, this.Client.Bounds.Width/2, this.Client.Bounds.Width/2);

				this.TextLayout.LayoutSize = new Size (rect.Height, rect.Width);
				adorner.PaintGeneralTextLayout (graphics, Rectangle.MaxValue, pos, this.TextLayout, state, this.paintTextStyle, TextFieldDisplayMode.Default, this.BackColor);

				graphics.Transform = iTransform;
			}
		}
	}
}
