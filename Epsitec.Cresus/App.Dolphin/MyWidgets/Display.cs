//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule un petit �cran bitmap monochrome.
	/// </summary>
	public class Display : Widget
	{
		public Display() : base()
		{
		}

		public Display(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void SetMemory(Components.Memory memory, int firstAddress, int dx, int dy)
		{
			System.Diagnostics.Debug.Assert(memory != null);
			System.Diagnostics.Debug.Assert(dx != 0);
			System.Diagnostics.Debug.Assert(dy != 0);
			System.Diagnostics.Debug.Assert((dx%8) == 0);
			this.memory = memory;
			this.firstAddress = firstAddress;
			this.dx = dx;
			this.dy = dy;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;
			Path path;
			
			rect.Deflate(0.5);
			path = new Path();
			path.AppendRoundedRectangle(rect, 15);
			graphics.Rasterizer.AddSurface(path);
			Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.5), this.FromBrightness(0.8));
			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.FromBrightness(0));
			path.Dispose();

			rect.Deflate(3.0);
			path = new Path();
			path.AppendRoundedRectangle(rect, 12);
			graphics.Rasterizer.AddSurface(path);
			Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.2), this.FromBrightness(0.4));
			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.FromBrightness(0));
			path.Dispose();

			rect.Deflate(10.5);
			path = new Path();
			path.AppendRoundedRectangle(rect, 5);
			graphics.Rasterizer.AddSurface(path);
			Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.2), this.FromBrightness(0.0));
			//?graphics.Rasterizer.AddOutline(path);
			//?graphics.RenderSolid(this.FromBrightness(0));
			path.Dispose();

			rect.Deflate(4.0);

			double px = rect.Width/this.dx;
			double py = rect.Height/this.dy;
			
			int address = this.firstAddress;
			for (int y=0; y<this.dy; y++)
			{
				for (int x=0; x<this.dx; x+=8)
				{
					int value = this.memory.ReadForDebug(address++);
					for (int b=0; b<8; b++)
					{
#if true
						if ((value & (1 << (7-b))) != 0)
						{
							Rectangle pixel = new Rectangle(rect.Left+px*(x+b), rect.Top-py*(y+1), px-1, py-1);
							graphics.AddFilledRectangle(pixel);
							graphics.RenderSolid(Display.ColorSet);
						}
#else
						Rectangle pixel = new Rectangle(rect.Left+px*(x+b), rect.Top-py*(y+1), px-1, py-1);
						Color color = (value & (1 << (7-b))) == 0 ? Display.ColorClr : Display.ColorSet;

						graphics.AddFilledRectangle(pixel);
						graphics.RenderSolid(color);
#endif
					}
				}
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

			return Color.FromBrightness(brightness);
		}

		
		protected static readonly Color ColorClr = Color.FromRgb(0, 0, 0);  // noir
		protected static readonly Color ColorSet = Color.FromRgb(0, 1, 0);  // vert

		protected Components.Memory memory;
		protected int firstAddress;
		protected int dx;
		protected int dy;
	}
}
