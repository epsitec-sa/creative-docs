//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule un interrrupteur à 2 positions.
	/// </summary>
	public class Switch : AbstractButton
	{
		public Switch() : base()
		{
		}

		public Switch(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rectExt = this.Client.Bounds;
			rectExt.Deflate(0.5);

			bool little = rectExt.Width <= 10;

			Rectangle rectInt = rectExt;
			rectInt.Deflate(little ? 2:3);

			Rectangle activator = rectInt;
			activator.Deflate(little ? 0:1);
			
			Rectangle rectShadow = rectInt;
			Path pathExt = new Path();

			double addH      = little ? 2:4;
			double shadowYes = little ? 4:10;
			double shadowNo  = little ? 3:6;
			double rr        = little ? 2:4;

			Color activatorColor = this.FromBrightness(0.9);
			if (little && this.ActiveState == ActiveState.Yes)
			{
				activatorColor = DolphinApplication.ColorHilite;  // rouge
			}

			if (rectExt.Width < rectExt.Height)  // bouton vertical ?
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					activator.Bottom = activator.Top-activator.Width-addH;
					rectShadow.Top = activator.Bottom;
					rectShadow.Bottom = rectShadow.Top-shadowYes;
				}
				else
				{
					activator.Top = activator.Bottom+activator.Width+addH;
					rectShadow.Bottom = rectShadow.Top-shadowNo;
				}

				pathExt.AppendRoundedRectangle(rectExt, rr);
				graphics.Rasterizer.AddSurface(pathExt);
				Geometry.RenderVerticalGradient(graphics, rectExt, this.FromBrightness(0.7, true), this.FromBrightness(1.0, true));

				graphics.AddFilledRectangle(rectInt);
				graphics.RenderSolid(this.FromBrightness(0.3));
				graphics.AddFilledRectangle(rectShadow);
				Geometry.RenderVerticalGradient(graphics, rectShadow, this.FromBrightness(0.3), this.FromBrightness(0.1));

				graphics.AddFilledRectangle(activator);
				graphics.RenderSolid(activatorColor);

				graphics.Rasterizer.AddOutline(pathExt);
				graphics.AddRectangle(rectInt);
				graphics.AddRectangle(activator);

				if (!little)
				{
					for (double y=activator.Bottom+4; y<activator.Top-4; y+=2)
					{
						graphics.AddLine(activator.Left, y, activator.Right, y);
					}
				}

				graphics.RenderSolid(this.FromBrightness(0));
			}
			else  // bouton horizontal ?
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					activator.Left = activator.Right-activator.Height-addH;
					rectShadow.Right = rectShadow.Left+shadowNo;
				}
				else
				{
					activator.Right = activator.Left+activator.Height+addH;
					rectShadow.Left = activator.Right;
					rectShadow.Right = rectShadow.Left+shadowYes;
				}

				pathExt.AppendRoundedRectangle(rectExt, rr);
				graphics.Rasterizer.AddSurface(pathExt);
				Geometry.RenderHorizontalGradient(graphics, rectExt, this.FromBrightness(1.0, true), this.FromBrightness(0.7, true));

				graphics.AddFilledRectangle(rectInt);
				graphics.RenderSolid(this.FromBrightness(0.3));
				graphics.AddFilledRectangle(rectShadow);
				Geometry.RenderHorizontalGradient(graphics, rectShadow, this.FromBrightness(0.1), this.FromBrightness(0.3));

				graphics.AddFilledRectangle(activator);
				graphics.RenderSolid(activatorColor);

				graphics.Rasterizer.AddOutline(pathExt);
				graphics.AddRectangle(rectInt);
				graphics.AddRectangle(activator);

				if (!little)
				{
					for (double x=activator.Left+4; x<activator.Right-4; x+=2)
					{
						graphics.AddLine(x, activator.Bottom, x, activator.Top);
					}
				}

				graphics.RenderSolid(this.FromBrightness(0));
			}

			pathExt.Dispose();
		}

		protected Color FromBrightness(double brightness)
		{
			return this.FromBrightness(brightness, false);
		}

		protected Color FromBrightness(double brightness, bool entered)
		{
			if (!this.Enable)
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
