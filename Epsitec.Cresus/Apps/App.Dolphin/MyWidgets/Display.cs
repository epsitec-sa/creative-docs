//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule un petit écran bitmap monochrome.
	/// Optimisé pour 32x24 pixels, avec une taille physique de 258x202.
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
			this.hx = -1;
			this.hy = -1;
		}

		public Display(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void SetMemory(Components.Memory memory, int firstAddress, int dx, int dy)
		{
			//	Initialise la zone mémoire correspondant à l'écran.
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
			//	Choix de la technologie simulée.
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


		protected int HX
		{
			get
			{
				return this.hx;
			}
			set
			{
				if (this.hx != value)
				{
					this.hx = value;
					this.Invalidate();
				}
			}
		}

		protected int HY
		{
			get
			{
				return this.hy;
			}
			set
			{
				if (this.hy != value)
				{
					this.hy = value;
					this.Invalidate();
				}
			}
		}


		protected Rectangle GetPixel(int x, int y)
		{
			Rectangle rect = this.Client.Bounds;
			rect.Deflate(17);

			y = this.dy-y-1;

			double px = rect.Width/this.dx;
			double py = rect.Height/this.dy;

			return new Rectangle(rect.Left+px*x, rect.Bottom+py*y, px, py);
		}

		protected bool Detect(Point pos, out int x, out int y)
		{
			//	Détecte le point visé par la souris.
			Rectangle rect = this.Client.Bounds;
			pos.Y = rect.Height-pos.Y-1;
			rect.Deflate(17);

			if (!rect.Contains(pos))
			{
				x = -1;
				y = -1;
				return false;
			}

			double px = rect.Width/this.dx;
			double py = rect.Height/this.dy;

			x = System.Math.Min((int) ((pos.X-rect.Left)/px), this.dx);
			y = System.Math.Min((int) ((pos.Y-rect.Bottom)/py), this.dy);
			return true;
		}

		protected void Convert(int x, int y, out int address, out int bit)
		{
			address = this.firstAddress;

			address += y*(this.dx/8);
			address += x/8;

			bit = 7-x%8;
		}

		protected bool TestPixel(int x, int y)
		{
			int address, bit;
			this.Convert(x, y, out address, out bit);

			int data = this.memory.ReadForDebug(address);
			return (data &= (1 << bit)) != 0;
		}

		protected void SetPixel(int x, int y, bool state)
		{
			int address, bit;
			this.Convert(x, y, out address, out bit);

			int data = this.memory.ReadForDebug(address);
			if (state)
			{
				data |= (1 << bit);
			}
			else
			{
				data &= ~(1 << bit);
			}
			this.memory.Write(address, data);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			int x, y;

			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if (this.Detect(pos, out x, out y))
					{
						this.firstState = this.TestPixel(x, y);
						this.SetPixel(x, y, !this.firstState);
						this.mouseDown = true;
					}
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if (this.mouseDown)
					{
						if (this.Detect(pos, out x, out y))
						{
							this.SetPixel(x, y, !this.firstState);
						}
					}

					this.Detect(pos, out x, out y);
					this.HX = x;
					this.HY = y;

					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.mouseDown = false;
					message.Consumer = this;
					break;

				case MessageType.MouseLeave:
					this.HX = -1;
					this.HY = -1;
					break;
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
				Geometry.RenderCircularGradient(graphics, reflect.Center, reflect.Width/2, Color.FromAlphaRgb(0.0, 0.2, 0.2, 0.2), DolphinApplication.FromBrightness(0.2));

				reflect = rect;
				reflect.Width *= 0.2;
				reflect.Bottom = reflect.Top-reflect.Width;
				reflect.Offset(-reflect.Width*0.2, reflect.Width*0.2);
				graphics.AddFilledRectangle(reflect);
				Geometry.RenderCircularGradient(graphics, reflect.Center, reflect.Width/2, Color.FromAlphaRgb(0.0, 0.3, 0.3, 0.3), DolphinApplication.FromBrightness(0.3));
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
								if ((value & (1 << (7-b))) != 0)  // bit allumé ?
								{
									if (!state)
									{
										state = true;
										start = x+b;
									}
								}
								else  // bit éteint ?
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
								//?if ((value & (1 << (7-b))) != 0)  // bit allumé ?
								if ((value & (1 << (7-b))) == 0)  // bit éteint ?
								{
									Rectangle pixel = new Rectangle(rect.Left+px*(x+b), rect.Top-py*(y+1), px-1, py-1);
									graphics.AddFilledRectangle(pixel);  // dessine un pixel carré
								}
							}
						}
					}
					graphics.RenderSolid(Color.FromRgb(1.0, 0.9, 0.6));  // ambre (jaune-orange pâle)
				}
			}

			//	Dessine le pixel survolé par la souris.
			if (this.hx != -1 && this.hy != -1)
			{
				Rectangle pixel = this.GetPixel(this.hx, this.hy);

				if (this.type == Type.CRT)
				{
					pixel.Left--;
					pixel.Bottom--;
					graphics.AddFilledCircle(pixel.Center, pixel.Width/2);
					graphics.RenderSolid(Color.FromAlphaRgb(0.7, 1.0, 0.7, 0.0));
				}

				if (this.type == Type.LCD)
				{
					graphics.AddFilledRectangle(pixel);
					graphics.RenderSolid(Color.FromAlphaRgb(0.7, 1.0, 0.7, 0.0));
				}
			}
		}

		protected void DrawPixelLine(Graphics graphics, Rectangle rect, int x1, int x2, int y)
		{
			//	Dessine une ligne horizontale de pixels allumés.
			//	On simule le faisceau d'électrons qui s'allume sur le premier point de gauche, pour 
			//	s'éteindre seulement sur le dernier point de droite.
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

			return DolphinApplication.FromBrightness(brightness);
		}

		
		protected Type						type;
		protected Components.Memory			memory;
		protected int						firstAddress;
		protected int						dx, dy;
		protected int						hx, hy;
		protected bool						mouseDown;
		protected bool						firstState;
	}
}
