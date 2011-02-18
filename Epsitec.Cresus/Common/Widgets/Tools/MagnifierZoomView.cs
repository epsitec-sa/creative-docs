//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Tools
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// The <c>Magnifier</c> class manages the rounded magnifier window
	/// which is used by the color picker.
	/// </summary>
	public class MagnifierZoomView : Widget
	{
		public MagnifierZoomView(Magnifier magnifier)
		{
			this.magnifier = magnifier;
		}
		
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if (message.IsLeftButton)
					{
						this.isDragging = true;
						this.origin      = this.MapClientToScreen (pos);
						
						MouseCursor.Hide ();
						message.Captured = true;
					}
					break;
				
				case MessageType.MouseMove:
					if (this.isDragging)
					{
						pos = this.MapClientToScreen (pos);
						
						this.Window.WindowLocation += pos - this.origin;
						this.Invalidate ();
						
						this.origin = pos;
					}
					break;
			
				case MessageType.MouseUp:
					MouseCursor.Show ();
					this.isDragging = false;
					break;
				
				case MessageType.MouseWheel:
					this.magnifier.SampleRadius += message.Wheel < 0 ? -1 : 1;
					break;
				
				case MessageType.KeyDown:
					switch (message.KeyCode)
					{
						case KeyCode.AlphaL:
							this.isRgbLcd = ! this.isRgbLcd;
							this.Invalidate ();
							break;
						
						case KeyCode.AlphaM:
							this.isRgbMono = ! this.isRgbMono;
							this.Invalidate ();
							break;
						
						case KeyCode.ArrowUp:
							this.Window.WindowLocation += new Drawing.Point (0, 1);
							this.Invalidate ();
							break;
							
						case KeyCode.ArrowDown:
							this.Window.WindowLocation += new Drawing.Point (0, -1);
							this.Invalidate ();
							break;
							
						case KeyCode.ArrowLeft:
							this.Window.WindowLocation += new Drawing.Point (-1, 0);
							this.Invalidate ();
							break;
							
						case KeyCode.ArrowRight:
							this.Window.WindowLocation += new Drawing.Point (1, 0);
							this.Invalidate ();
							break;
					}

					break;

				case MessageType.KeyPress:
				case MessageType.KeyUp:
					break;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}

		protected override bool AboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.timer != null)
				{
					this.timer.Stop ();
					this.timer.Dispose ();
					this.timer = null;
				}
				if (this.mask != null)
				{
					this.mask.Dispose ();
					this.mask = null;
				}
			}
			
			base.Dispose (disposing);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (this.magnifier.SampleRadius < 1)
			{
				return;
			}
			
			this.ConfigureTimer ();

			double dx = this.Client.Size.Width;
			double dy = this.Client.Size.Height;
			
			double cx = dx / 2;
			double cy = dy / 2;
			
			Drawing.Path path;
			
			if ((this.mask == null) ||
				(this.maskDx != dx) ||
				(this.maskDy != dy))
			{
				if (this.mask != null)
				{
					this.mask.Dispose ();
					this.mask = null;
				}
				
				this.mask = graphics.CreateAlphaMask ();
				this.maskDx = dx;
				this.maskDy = dy;
				
				path = Drawing.Path.FromCircle (cx, cy, cx, cy);
				
				this.mask.Color = Drawing.Color.FromRgb (1, 0, 0);
				this.mask.PaintSurface (path);
				
				path.Dispose ();
			}
			
			Drawing.Point  pos    = this.MapClientToScreen (new Drawing.Point (cx, cy));
			Drawing.Bitmap bitmap = this.magnifier.Bitmap.BitmapImage;
			
			int nx = bitmap.PixelWidth;
			int ny = bitmap.PixelHeight;
			
			int px = (int) (pos.X) - nx / 2;
			int py = (int) (pos.Y) - ny / 2;
			
			Win32Api.GrabScreen (bitmap, px, py);
			
			double sx = dx / nx;
			double sy = dy / ny;
			
			graphics.SolidRenderer.SetAlphaMask (this.mask.Pixmap, Drawing.MaskComponent.R);
			
			using (Drawing.Pixmap.RawData raw = new Drawing.Pixmap.RawData (bitmap))
			{
				double x = 0;
				double y = 0;
				
				for (int iy = 0; iy < ny; iy++)
				{
					x = 0;
					
					if (this.isRgbLcd)
					{
						if (this.isRgbMono)
						{
							for (int ix = 0; ix < nx; ix++)
							{
								Drawing.Color sample = raw[ix, iy];
								path = Drawing.Path.FromRectangle (x, dy-y-sy, sx/3+1, sy+1);
								graphics.Color = Drawing.Color.FromBrightness (sample.R);
								graphics.PaintSurface (path);
								path.Dispose ();
								path = Drawing.Path.FromRectangle (x+sx/3, dy-y-sy, sx/3+1, sy+1);
								graphics.Color = Drawing.Color.FromBrightness (sample.G);
								graphics.PaintSurface (path);
								path.Dispose ();
								path = Drawing.Path.FromRectangle (x+2*sx/3, dy-y-sy, sx/3+1, sy+1);
								graphics.Color = Drawing.Color.FromBrightness (sample.B);
								graphics.PaintSurface (path);
								path.Dispose ();
								x += sx;
							}
						}
						else
						{
							for (int ix = 0; ix < nx; ix++)
							{
								Drawing.Color sample = raw[ix, iy];
								path = Drawing.Path.FromRectangle (x, dy-y-sy, sx/3+1, sy+1);
								graphics.Color = Drawing.Color.FromRgb (sample.R, 0, 0);
								graphics.PaintSurface (path);
								path.Dispose ();
								path = Drawing.Path.FromRectangle (x+sx/3, dy-y-sy, sx/3+1, sy+1);
								graphics.Color = Drawing.Color.FromRgb (0, sample.G, 0);
								graphics.PaintSurface (path);
								path.Dispose ();
								path = Drawing.Path.FromRectangle (x+2*sx/3, dy-y-sy, sx/3+1, sy+1);
								graphics.Color = Drawing.Color.FromRgb (0, 0, sample.B);
								graphics.PaintSurface (path);
								path.Dispose ();
								x += sx;
							}
						}
					}
					else
					{
						for (int ix = 0; ix < nx; ix++)
						{
							path = Drawing.Path.FromRectangle (x, dy-y-sy, sx+1, sy+1);
							graphics.Color = raw[ix, iy];
							graphics.PaintSurface (path);
							path.Dispose ();
							x += sx;
						}
					}
					y += sy;
				}
				
				this.magnifier.HotColor = raw[nx/2, ny/2];
			}
			
			double ox = (nx / 2) * sx - 0.5;
			double oy = (ny / 2) * sy + 0.5;
			
			path = new Drawing.Path ();
			
			if (this.magnifier.IsColorPicker)
			{
				path.MoveTo (ox+2, oy+0);
				path.LineTo (ox+0, oy+0);
				path.LineTo (ox+0, oy+2);
				path.MoveTo (ox+0, oy+sy+1-2);
				path.LineTo (ox+0, oy+sy+1-0);
				path.LineTo (ox+2, oy+sy+1-0);
				path.MoveTo (ox+sx+1-2, oy+sy+1-0);
				path.LineTo (ox+sx+1-0, oy+sy+1-0);
				path.LineTo (ox+sx+1-0, oy+sy+1-2);
				path.MoveTo (ox+sx+1-0, oy+2);
				path.LineTo (ox+sx+1-0, oy+0);
				path.LineTo (ox+sx+1-2, oy+0);
			}
			
			path.AppendCircle (dx/2, dy/2, dx/2-0.5, dy/2-0.5);
			path.AppendCircle (dx/2, dy/2, dx/2, dy/2);
			
			graphics.Color = Drawing.Color.FromAlphaRgb (0.5, 0, 0, 0.8);
			graphics.LineWidth = 1.0;
			graphics.PaintOutline (path);
			
			path.Dispose ();
			
			graphics.SolidRenderer.SetAlphaMask (null, Drawing.MaskComponent.None);
			
			if (this.magnifier.IsColorPicker)
			{
				Drawing.Color color = this.magnifier.HotColor;
				
				int r = (int)(color.R * 255.5);
				int g = (int)(color.G * 255.5);
				int b = (int)(color.B * 255.5);
				
				this.PaintText (graphics, string.Format ("{0:X2}:{1:X2}:{2:X2}", r, g, b));
			}
		}
		
		
		protected void PaintText(Drawing.Graphics graphics, string text)
		{
			Drawing.Path path = new Drawing.Path ();
			Drawing.Font font = Drawing.Font.GetFont ("Tahoma", "Regular");
			double       size = 10;
			
			double dx = font.GetTextAdvance (text) * size;

			double ox = (this.Client.Size.Width - dx) / 2;
			double oy = 2;
			
			graphics.AddFilledRectangle (ox - 1, oy - 2, dx + 2, font.LineHeight * size - 0);
			graphics.AddFilledRectangle (ox - 2, oy - 1, dx + 4, font.LineHeight * size - 2);
			graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.8, 1, 1, 1));
			
			graphics.AddText (ox, oy, text, font, size);
			graphics.RenderSolid (Drawing.Color.FromBrightness (0));
		}
		
		
		private void ConfigureTimer()
		{
			if (this.timer == null)
			{
				this.timer = new Timer ();
				this.timer.AutoRepeat = 0.1;
				this.timer.Delay = 0.1;
				this.timer.TimeElapsed += HandleTimerTimeElapsed;
				this.timer.Start ();
			}
		}
		
		private void HandleTimerTimeElapsed(object sender)
		{
			if (this.IsVisible)
			{
				this.Invalidate ();
			}
			else if (this.timer != null)
			{
				this.timer.Stop ();
				this.timer.Dispose ();
				this.timer = null;
			}
		}
		
		
		private Magnifier					magnifier;
		
		private bool						isRgbLcd;
		private bool						isRgbMono;
		private bool						isDragging;
		private Drawing.Point				origin;
		private double						maskDx, maskDy;
		private Drawing.Graphics			mask;
		
		private Timer						timer;
	}
}
