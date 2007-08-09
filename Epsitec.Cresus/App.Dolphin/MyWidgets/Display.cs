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
	/// Optimis� pour 32x24 pixels, avec une taille physique de 258x202.
	/// </summary>
	public class Display : Widget
	{
		public enum Type
		{
			CRT,		// tube cathodique monochrome
			LCD,		// dalle LCD
		}


		public Display() : base()
		{
			this.type = Type.CRT;
		}

		public Display(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void SetMemory(Components.Memory memory, int firstAddress, int dx, int dy)
		{
			//	Initialise la zone m�moire correspondant � l'�cran.
			System.Diagnostics.Debug.Assert(memory != null);
			System.Diagnostics.Debug.Assert(dx != 0);
			System.Diagnostics.Debug.Assert(dy != 0);
			System.Diagnostics.Debug.Assert((dx%8) == 0);
			this.memory = memory;
			this.firstAddress = firstAddress;
			this.dx = dx;
			this.dy = dy;
		}

		public Type Technology
		{
			//	Choix de la technologie simul�e.
			get
			{
				return this.type;
			}
			set
			{
				if (this.type != value)
				{
					this.type = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;
			Path path;

			if (this.type == Type.CRT)
			{
				rect.Deflate(0.5);
				path = new Path();
				path.AppendRoundedRectangle(rect, 23);
				graphics.Rasterizer.AddSurface(path);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.5), this.FromBrightness(0.8));
				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(this.FromBrightness(0));
				path.Dispose();

				rect.Deflate(3.0);
				path = new Path();
				path.AppendRoundedRectangle(rect, 20);
				graphics.Rasterizer.AddSurface(path);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.1), this.FromBrightness(0.3));
				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(this.FromBrightness(0));
				path.Dispose();

				rect.Deflate(9.5);
				path = new Path();
				path.AppendRoundedRectangle(rect, 10);
				graphics.Rasterizer.AddSurface(path);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.0), this.FromBrightness(0.2));
				path.Dispose();

				rect.Deflate(4.0);

				//	Dessine quelques reflets.
				Rectangle reflect = rect;
				reflect.Width *= 0.8;
				reflect.Bottom = reflect.Top-reflect.Width;
				reflect.Offset(-reflect.Width*0.2, reflect.Width*0.2);
				graphics.AddFilledRectangle(reflect);
				Geometry.RenderCircularGradient(graphics, reflect.Center, reflect.Width/2, Color.FromAlphaRgb(0.0, 0.2, 0.2, 0.2), Color.FromBrightness(0.2));

				reflect = rect;
				reflect.Width *= 0.2;
				reflect.Bottom = reflect.Top-reflect.Width;
				reflect.Offset(-reflect.Width*0.2, reflect.Width*0.2);
				graphics.AddFilledRectangle(reflect);
				Geometry.RenderCircularGradient(graphics, reflect.Center, reflect.Width/2, Color.FromAlphaRgb(0.0, 0.3, 0.3, 0.3), Color.FromBrightness(0.3));
			}

			if (this.type == Type.LCD)
			{
				rect.Deflate(0.5);
				path = new Path();
				path.AppendRoundedRectangle(rect, 10);
				graphics.Rasterizer.AddSurface(path);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.6), this.FromBrightness(0.8));
				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(this.FromBrightness(0));
				path.Dispose();

				rect.Deflate(11.0);
				path = new Path();
				path.AppendRoundedRectangle(rect, 7);
				graphics.Rasterizer.AddSurface(path);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.8), this.FromBrightness(0.5));
				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(this.FromBrightness(0));
				path.Dispose();

				rect.Deflate(4.0);
				path = new Path();
				path.AppendRoundedRectangle(rect, 2);
				graphics.Rasterizer.AddSurface(path);
				Geometry.RenderVerticalGradient(graphics, rect, this.FromBrightness(0.0), this.FromBrightness(0.2));
				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(this.FromBrightness(0));
				path.Dispose();

				rect.Deflate(1.5);
			}

			if (this.memory != null)
			{
				int address = this.firstAddress;

				if (this.type == Type.CRT)
				{
					for (int y=0; y<this.dy; y++)
					{
						bool state = false;
						int start = 0;

						for (int x=0; x<this.dx; x+=8)
						{
							int value = this.memory.ReadForDebug(address++);
							for (int b=0; b<8; b++)
							{
								if ((value & (1 << (7-b))) != 0)  // bit allum� ?
								{
									if (!state)
									{
										state = true;
										start = x+b;
									}
								}
								else  // bit �teint ?
								{
									if (state)
									{
										state = false;
										this.DrawPixelLine(graphics, rect, start, x+b-1, y);
									}
								}
							}
						}

						if (state)
						{
							this.DrawPixelLine(graphics, rect, start, this.dx-1, y);
						}
					}
				}

				if (this.type == Type.LCD)
				{
					double px = rect.Width/this.dx;
					double py = rect.Height/this.dy;

					for (int y=0; y<this.dy; y++)
					{
						for (int x=0; x<this.dx; x+=8)
						{
							int value = this.memory.ReadForDebug(address++);
							for (int b=0; b<8; b++)
							{
								if ((value & (1 << (7-b))) != 0)  // bit allum� ?
								{
									Rectangle pixel = new Rectangle(rect.Left+px*(x+b), rect.Top-py*(y+1), px-1, py-1);
									graphics.AddFilledRectangle(pixel);  // dessine un pixel carr�
									graphics.RenderSolid(Color.FromRgb(1.0, 0.9, 0.6));  // ambre (jaune-orange p�le)
								}
							}
						}
					}
				}
			}
		}

		protected void DrawPixelLine(Graphics graphics, Rectangle rect, int x1, int x2, int y)
		{
			//	Dessine une ligne horizontale de pixels allum�s.
			//	On simule le faisceau d'�lectrons qui s'allume sur le premier point de gauche, pour 
			//	s'�teindre seulement sur le dernier point de droite.
			double px = rect.Width/this.dx;
			double py = rect.Height/this.dy;

			Rectangle r1 = new Rectangle(rect.Left+px*x1, rect.Top-py*(y+1), px-1, py-1);
			Rectangle r2 = new Rectangle(rect.Left+px*x2, rect.Top-py*(y+1), px-1, py-1);

			double k = Path.Kappa*r1.Height/2;

			Path path = new Path();
			path.MoveTo(r1.Left, r1.Center.Y);
			path.CurveTo(r1.Left, r1.Center.Y+k, r1.Center.X-k, r1.Top, r1.Center.X, r1.Top);
			path.LineTo(r2.Center.X, r2.Top);
			path.CurveTo(r2.Center.X+k, r2.Top, r2.Right, r2.Center.Y+k, r2.Right, r2.Center.Y);
			path.CurveTo(r2.Right, r2.Center.Y-k, r2.Center.X+k, r2.Bottom, r2.Center.X, r2.Bottom);
			path.LineTo(r1.Center.X, r1.Bottom);
			path.CurveTo(r1.Center.X-k, r1.Bottom, r1.Left, r1.Center.Y-k, r1.Left, r1.Center.Y);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromRgb(0, 1, 0));  // vert

			path.Dispose();
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

		
		protected Type type;
		protected Components.Memory memory;
		protected int firstAddress;
		protected int dx;
		protected int dy;
	}
}
