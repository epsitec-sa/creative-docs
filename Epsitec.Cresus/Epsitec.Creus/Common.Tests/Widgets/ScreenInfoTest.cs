using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Widgets
{
	[TestFixture]
	public class ScreenInfoTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckAllScreens()
		{
			ScreenInfo[] screens = ScreenInfo.AllScreens;
			
			System.Console.Out.WriteLine ("Found {0} screens. GlobalArea={1}", screens.Length, ScreenInfo.GlobalArea);
			System.Console.Out.WriteLine ();
			
			foreach (ScreenInfo info in screens)
			{
				System.Console.Out.WriteLine ("{0} :", info.DeviceName);
				System.Console.Out.WriteLine (" - Bounds={0}", info.Bounds);
				System.Console.Out.WriteLine (" - WorkingArea={0}", info.WorkingArea);
				System.Console.Out.WriteLine (" - IsPrimary={0}", info.IsPrimary);
				System.Console.Out.WriteLine ();
				
				Assert.IsTrue (info.Bounds.Contains (info.WorkingArea));
				Assert.IsTrue (ScreenInfo.GlobalArea.Contains (info.Bounds));
			}
		}
		
		[Test] public void CheckFindPoint()
		{
			ScreenInfo[] screens = ScreenInfo.AllScreens;
			
			foreach (ScreenInfo info in screens)
			{
				ScreenInfo found = ScreenInfo.Find (new Point (info.Bounds.Left + info.Bounds.Width / 2, info.Bounds.Bottom + info.Bounds.Height / 2));
				Assert.AreEqual (info.Bounds, found.Bounds);
			}
		}
		
		[Test] public void CheckFindRectangle()
		{
			ScreenInfo[] screens = ScreenInfo.AllScreens;
			
			foreach (ScreenInfo info in screens)
			{
				Rectangle rect1 = new Rectangle (info.Bounds.X - 100, info.Bounds.Bottom + info.Bounds.Height / 2 - 100, 120, 200);
				Rectangle rect2 = new Rectangle (info.Bounds.X -  20, info.Bounds.Bottom + info.Bounds.Height / 2 - 100, 120, 200);
				
				ScreenInfo found1 = ScreenInfo.Find (rect1);
				ScreenInfo found2 = ScreenInfo.Find (rect2);
				
				System.Console.Out.WriteLine ("First rect found on {0}, Bounds={1}", found1.DeviceName, found1.Bounds.ToString ());
				System.Console.Out.WriteLine ("Second rect found on {0}, Bounds={1}", found2.DeviceName, found2.Bounds.ToString ());
				System.Console.Out.WriteLine ();
			}
		}
		
		[Test]
		[Ignore ("Broken under Vista")]
		public void CheckGrabScreen()
		{
			Window window = new Window ();
			
			window.MakeTopLevelWindow ();
			window.MakeFramelessWindow ();
			window.MakeLayeredWindow ();
			
			window.ClientSize = new Size (110, 110);
			window.Root.BackColor = Color.Transparent;
			
			Magnifier glass = new Magnifier ();
			
			glass.Dock   = DockStyle.Fill;
			glass.SetParent (window.Root);
			glass.Focus();
			
			glass.HotColorChanged += this.HandleMagnifierHotColorChanged;
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test]
		[Ignore ("Broken under Vista")]
		public void CheckMagnifier()
		{
			Epsitec.Common.Widgets.Tools.Magnifier magnifier = new Epsitec.Common.Widgets.Tools.Magnifier ();
			
			magnifier.Show ();
			Window.RunInTestEnvironment (magnifier.ZoomWindow);
		}
		
		[Test]
		[Ignore ("Broken under Vista")]
		public void CheckColorPicker()
		{
			Epsitec.Common.Widgets.Tools.Magnifier magnifier = new Epsitec.Common.Widgets.Tools.Magnifier ();
			
			magnifier.DisplayRadius = 110;
			magnifier.IsColorPicker = true;
			magnifier.Show ();
			Window.RunInTestEnvironment (magnifier.ZoomWindow);
		}
		
		
		private class Magnifier : Widget
		{
			public Magnifier()
			{
				this.bitmap = Bitmap.FromNativeBitmap (11, 11);
				this.scale  = 11;
			}
			
			
			public Color				HotColor
			{
				get
				{
					return this.hot_color;
				}
				
				set
				{
					if (this.hot_color != value)
					{
						this.hot_color = value;
						this.OnHotColorChanged ();
					}
				}
			}
			
			
			protected override void ProcessMessage(Message message, Point pos)
			{
				switch (message.MessageType)
				{
					case MessageType.MouseDown:
						if (message.IsLeftButton)
						{
							this.is_dragging = true;
							this.origin      = this.MapClientToScreen (pos);
							MouseCursor.Hide ();
							message.Captured = true;
						}
						break;
					
					case MessageType.MouseMove:
						if (this.is_dragging)
						{
							pos = this.MapClientToScreen (pos);
							
							this.Window.WindowLocation += pos - this.origin;
							this.Invalidate ();
							
							this.origin = pos;
						}
						break;
				
					case MessageType.MouseUp:
						MouseCursor.Show ();
						this.is_dragging = false;
						break;
					
					case MessageType.MouseWheel:
						int wheel = message.Wheel < 0 ? -2 : 2;
						int scale = this.scale + wheel;
						
						if (scale < 5)  scale = 5;
						if (scale > 21) scale = 21;
						
						double dx = this.Client.Size.Width;
						double dy = this.Client.Size.Height;
						
						this.bitmap = Bitmap.FromNativeBitmap (scale, scale);
						this.scale  = scale;
						this.Invalidate ();
						break;
					
					case MessageType.KeyDown:
						switch (message.KeyCode)
						{
							case KeyCode.ArrowUp:
								this.Window.WindowLocation += new Point (0, 1);
								this.Invalidate ();
								break;
							
							case KeyCode.ArrowDown:
								this.Window.WindowLocation += new Point (0, -1);
								this.Invalidate ();
								break;
							
							case KeyCode.ArrowLeft:
								this.Window.WindowLocation += new Point (-1, 0);
								this.Invalidate ();
								break;
							
							case KeyCode.ArrowRight:
								this.Window.WindowLocation += new Point (1, 0);
								this.Invalidate ();
								break;
						}
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
				}
				
				base.Dispose (disposing);
			}

			
			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				if (this.timer == null)
				{
					this.timer = new Timer ();
					this.timer.AutoRepeat = 0.1;
					this.timer.Delay = 0.1;
					this.timer.TimeElapsed += HandleTimerTimeElapsed;
					this.timer.Start ();
				}
				
				double dx = this.Client.Size.Width;
				double dy = this.Client.Size.Height;
				
				Path path;
				
				if ((this.mask == null) ||
					(this.mask_dx != dx) ||
					(this.mask_dy != dy))
				{
					if (this.mask != null)
					{
						this.mask.Dispose ();
						this.mask = null;
					}
					
					this.mask = graphics.CreateAlphaMask ();
					this.mask_dx = dx;
					this.mask_dy = dy;
					
					path = Path.FromCircle (dx/2, dy/2, dx/2, dy/2);
					
					this.mask.Color = Color.FromRgb (1, 0, 0);
					this.mask.PaintSurface (path);
					
					path.Dispose ();
				}
				
				Point pos = this.MapClientToScreen (new Point (dx/2, dy/2));
				
				int pix_x = (int) (pos.X - this.bitmap.Width /2);
				int pix_y = (int) (pos.Y - this.bitmap.Height/2);
				
				Window.GrabScreen (bitmap, pix_x, pix_y);
				
				double sx = dx / this.bitmap.Width;
				double sy = dy / this.bitmap.Height;
				
				graphics.SolidRenderer.SetAlphaMask (this.mask.Pixmap, MaskComponent.R);
				
				using (Pixmap.RawData raw = new Pixmap.RawData (this.bitmap))
				{
					int nx = (int) this.bitmap.Width;
					int ny = (int) this.bitmap.Height;
					
					double x = 0;
					double y = 0;
					
					for (int iy = 0; iy < ny; iy++)
					{
						x = 0;
						
						for (int ix = 0; ix < nx; ix++)
						{
							path = Path.FromRectangle (x, dy-y-sy, sx+1, sy+1);
							graphics.Color = raw[ix, iy];
							graphics.PaintSurface (path);
							path.Dispose ();
							x += sx;
						}
						y += sy;
					}
					
					this.HotColor = raw[nx/2, ny/2];
				}
				
				double ox = System.Math.Floor (this.bitmap.Width / 2)  * sx - 0.5;
				double oy = System.Math.Floor (this.bitmap.Height / 2) * sy + 0.5;
				
				path = new Path ();
				
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
				path.AppendCircle (dx/2, dy/2, dx/2-0.5, dy/2-0.5);
				path.AppendCircle (dx/2, dy/2, dx/2, dy/2);
				
				graphics.Color = Color.FromAlphaRgb (0.5, 0, 0, 0.8);
				graphics.LineWidth = 1.0;
				graphics.PaintOutline (path);
				
				path.Dispose ();
				
				graphics.SolidRenderer.SetAlphaMask (null, MaskComponent.None);
				
				System.Text.StringBuilder buffer = new System.Text.StringBuilder();
				
				buffer.AppendFormat ("{0:X2}", (int)(this.HotColor.R * 255.5));
				buffer.Append (":");
				buffer.AppendFormat ("{0:X2}", (int)(this.HotColor.G * 255.5));
				buffer.Append (":");
				buffer.AppendFormat ("{0:X2}", (int)(this.HotColor.B * 255.5));
				
				this.PaintText (graphics, buffer.ToString ());
			}
			
			protected virtual void PaintText(Graphics graphics, string text)
			{
				Path path = new Path ();
				Font font = Font.GetFont ("Tahoma", "Regular");
				double       size = 10;
				
				double dx = font.GetTextAdvance (text) * size;
				
				double ox = (this.Client.Size.Width - dx) / 2;
				double oy = 2;
				
				graphics.AddFilledRectangle (ox - 1, oy - 2, dx + 2, font.LineHeight * size - 0);
				graphics.AddFilledRectangle (ox - 2, oy - 1, dx + 4, font.LineHeight * size - 2);
				graphics.RenderSolid (Color.FromAlphaRgb (0.8, 1, 1, 1));
				
				graphics.AddText (ox, oy, text, font, size);
				graphics.RenderSolid (Color.FromBrightness (0));
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
			
			protected virtual void OnHotColorChanged()
			{
				if (this.HotColorChanged != null)
				{
					this.HotColorChanged (this);
				}
			}
			
			
			public event EventHandler	HotColorChanged;
			
			
			private Color				hot_color;
			private int							scale;
			
			private bool						is_dragging;
			
			private double						mask_dx, mask_dy;
			private Point				origin;
			private Image				bitmap;
			private Graphics			mask;
			
			private Timer						timer;
		}

		private void HandleMagnifierHotColorChanged(object sender)
		{
			Magnifier glass = sender as Magnifier;
			
			System.Diagnostics.Debug.WriteLine ("Hot color: " + glass.HotColor.ToString ());
		}
	}
}
