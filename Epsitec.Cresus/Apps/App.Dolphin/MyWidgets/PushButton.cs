//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule un bouton poussoir généralement carré.
	/// </summary>
	public class PushButton : Button
	{
		public PushButton() : base()
		{
		}

		public PushButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			WidgetPaintState state = this.PaintState;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);
			bool little = rect.Width <= 30;

			Point pos = this.GetTextLayoutOffset();
			pos.Y += this.GetBaseLineVerticalOffset();

			if ((state & WidgetPaintState.Engaged) != 0)
			{
				Path pathExt = new Path();
				pathExt.AppendRoundedRectangle(rect, little?3:5);

				graphics.Rasterizer.AddSurface(pathExt);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.5), this.FromBrightness(0.2));

				graphics.Rasterizer.AddOutline(pathExt);
				graphics.RenderSolid(this.FromBrightness(0));

				double offset = little?1:2;
				rect.Deflate(offset);

				pos.X += offset/2;
				pos.Y -= offset/2;
			}

			if ((state & WidgetPaintState.Enabled) == 0)
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}
			
			if ( this.BackColor.IsTransparent )
			{
				//	Ne peint pas le fond du bouton si celui-ci a un fond explicitement défini
				//	comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'état sélectionné si on peint nous-même le fond du bouton.
				Rectangle rectExt = rect;
				Path pathExt = new Path();
				pathExt.AppendRoundedRectangle(rectExt, little?3:5);

				Rectangle rectInt = rect;
				rectInt.Deflate(little?2:3);
				Path pathInt = new Path();
				pathInt.AppendRoundedRectangle(rectInt, little?2:3);

				graphics.Rasterizer.AddSurface(pathExt);
				Geometry.RenderVerticalGradient(graphics, rectExt, this.FromBrightness(0.3, true), this.FromBrightness(0.9, true));

				if (this.ActiveState == ActiveState.Yes)
				{
					graphics.Rasterizer.AddSurface(pathInt);
					graphics.RenderSolid((state & WidgetPaintState.Enabled) != 0 ? DolphinApplication.ColorHilite : DolphinApplication.FromBrightness(0.7));
				}
				else
				{
					graphics.Rasterizer.AddSurface(pathInt);
					Geometry.RenderVerticalGradient(graphics, rectInt, this.FromBrightness(0.9), this.FromBrightness(0.7));
				}

				graphics.Rasterizer.AddOutline(pathExt);
				graphics.Rasterizer.AddOutline(pathInt);
				graphics.RenderSolid(this.FromBrightness(0));
				
				pathExt.Dispose();
				pathInt.Dispose();
			}

			if ( this.innerZoom != 1.0 )
			{
				Transform transform = graphics.Transform;
				graphics.ScaleTransform(this.innerZoom, this.innerZoom, this.Client.Size.Width/2, this.Client.Size.Height/2);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.ButtonStyle);
				graphics.Transform = transform;
			}
			else
			{
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.ButtonStyle);
			}
		}

		protected Color FromBrightness(double brightness)
		{
			return this.FromBrightness(brightness, false);
		}

		protected Color FromBrightness(double brightness, bool entered)
		{
			if ((this.PaintState & WidgetPaintState.Enabled) == 0)
			{
				brightness = 0.7 + brightness*0.2;  // plus clair
			}
			else if (entered && this.IsEntered)
			{
				return Color.FromHsv(35, 1.0-brightness*0.5, 1);  // orange
			}

			return DolphinApplication.FromBrightness(brightness);
		}
	}
}
