//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Tools
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// La classe Magnifier permet de gérer une loupe.
	/// </summary>
	public class Magnifier
	{
		public Magnifier()
		{
			this.zoom_window = new Window ();
			this.zoom_view   = new ZoomView (this);
			
			this.zoom_window.MakeTopLevelWindow ();
			this.zoom_window.MakeFramelessWindow ();
			this.zoom_window.MakeLayeredWindow ();
			
			this.zoom_window.ClientSize = new Drawing.Size (111, 111);
			this.zoom_window.Root.BackColor = Drawing.Color.Transparent;
			
			this.zoom_view.Dock   = DockStyle.Fill;
			this.zoom_view.SetParent (this.zoom_window.Root);
			this.zoom_view.SetFocused (true);
			
			this.DisplayRadius = 55;
			this.SampleRadius  = 3;
		}
		
		
		public void Show()
		{
			this.zoom_window.Show ();
		}
		
		public void Hide()
		{
			this.zoom_window.Hide ();
		}
		
		
		public double							DisplayRadius
		{
			get
			{
				return this.display_radius;
			}
			set
			{
				if (value < 40)  value = 40;
				if (value > 200) value = 200;
				
				if (this.display_radius != value)
				{
					this.display_radius = value;
					this.OnDisplayRadiusChanged ();
				}
			}
		}
		
		public int								SampleRadius
		{
			get
			{
				return this.sample_radius;
			}
			set
			{
				if (value < 2)  value = 2;
				if (value > 10) value = 10;
				
				if (this.sample_radius != value)
				{
					this.sample_radius = value;
					this.OnSampleRadiusChanged ();
				}
			}
		}
		
		public Drawing.Color					HotColor
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
		
		public bool								IsColorPicker
		{
			get
			{
				return this.color_picker;
			}
			set
			{
				if (this.color_picker != value)
				{
					this.color_picker = value;
					this.zoom_view.Invalidate ();
				}
			}
		}
		
		
		protected virtual void OnHotColorChanged()
		{
			if (this.HotColorChanged != null)
			{
				this.HotColorChanged (this);
			}
		}
		
		protected virtual void OnSampleRadiusChanged()
		{
			this.UpdateBitmapSize ();
			
			if (this.SampleRadiusChanged != null)
			{
				this.SampleRadiusChanged (this);
			}
		}
		
		protected virtual void OnDisplayRadiusChanged()
		{
			this.UpdateWindowSize ();
			
			if (this.DisplayRadiusChanged != null)
			{
				this.DisplayRadiusChanged (this);
			}
		}
		
		
		protected virtual void UpdateBitmapSize()
		{
			int dx = this.SampleRadius * 2 + 1;
			int dy = dx;
			
			if ((this.bitmap == null) ||
				(this.bitmap.BitmapImage.PixelWidth != dx) ||
				(this.bitmap.BitmapImage.PixelHeight != dy))
			{
				if (this.bitmap != null)
				{
					this.bitmap.Dispose ();
					this.bitmap = null;
				}
				
				this.bitmap = Drawing.Bitmap.FromNativeBitmap (dx, dy);
				
				this.zoom_view.Invalidate ();
			}
		}
		
		protected virtual void UpdateWindowSize()
		{
			double dx = this.DisplayRadius * 2 + 1;
			double dy = dx;
			
			this.zoom_window.ClientSize = new Drawing.Size (dx, dy);
		}
		
		
		public event Support.EventHandler		HotColorChanged;
		public event Support.EventHandler		SampleRadiusChanged;
		public event Support.EventHandler		DisplayRadiusChanged;
		
		
		private Drawing.Color					hot_color;
		private double							display_radius = 0;
		private int								sample_radius  = 0;
		private Drawing.Image					bitmap;
		private ZoomView						zoom_view;
		private Window							zoom_window;
		private bool							color_picker;
		
		public class ZoomView : Widget
		{
			public ZoomView(Magnifier magnifier)
			{
				this.magnifier = magnifier;
			}
			
			
			protected override void ProcessMessage(Message message, Drawing.Point pos)
			{
				switch (message.Type)
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
						this.magnifier.SampleRadius += message.Wheel < 0 ? -1 : 1;
						break;
					
					case MessageType.KeyDown:
						switch (message.KeyCode)
						{
							case KeyCode.AlphaL:
								this.is_rgb_lcd = ! this.is_rgb_lcd;
								this.Invalidate ();
								break;
							
							case KeyCode.AlphaM:
								this.is_rgb_mono = ! this.is_rgb_mono;
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
					
					default:
						return;
				}
				
				message.Consumer = this;
			}

			protected override bool AboutToLoseFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode)
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

			protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
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
					
					path = Drawing.Path.FromCircle (cx, cy, cx, cy);
					
					this.mask.Color = Drawing.Color.FromRgb (1, 0, 0);
					this.mask.PaintSurface (path);
					
					path.Dispose ();
				}
				
				Drawing.Point  pos    = this.MapClientToScreen (new Drawing.Point (cx, cy));
				Drawing.Bitmap bitmap = this.magnifier.bitmap.BitmapImage;
				
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
						
						if (this.is_rgb_lcd)
						{
							if (this.is_rgb_mono)
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
					this.timer.TimeElapsed += new Support.EventHandler(HandleTimerTimeElapsed);
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
			
			private bool						is_rgb_lcd;
			private bool						is_rgb_mono;
			private bool						is_dragging;
			private Drawing.Point				origin;
			private double						mask_dx, mask_dy;
			private Drawing.Graphics			mask;
			
			private Timer						timer;
		}
		
		public class DragSource : Widget, Behaviors.IDragBehaviorHost
		{
			public DragSource()
			{
				this.drag_behavior = new Behaviors.DragBehavior (this);
			}
			
			public DragSource(Widget embedder) : this ()
			{
				this.SetEmbedder (embedder);
			}
			
			
			public Drawing.Color				HotColor
			{
				get
				{
					return this.color;
				}
			}
			
			protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
			{
				double dx = this.Client.Size.Width;
				double dy = this.Client.Size.Height;
				double cx = dx / 2;
				double cy = dy / 2;
				
				double r = System.Math.Min (cx, cy) - 1;
				
				Drawing.Color color_1 = Drawing.Color.FromAlphaRgb (0.3, 0.3, 0.8, 1.0);
				Drawing.Color color_2 = Drawing.Color.FromRgb (0, 0, 0.7);
				
				if ((this.PaintState & WidgetPaintState.Enabled) == 0)
				{
					double bright = color_1.GetBrightness ();
					
					color_1.R = bright;
					color_1.G = bright;
					color_1.B = bright;
					
					color_2 = Adorners.Factory.Active.ColorTextFieldBorder (false);
				}
				
				graphics.AddFilledCircle (cx, cy, r);
				graphics.RenderSolid (color_1);
				graphics.LineWidth = 0.5;
				graphics.AddCircle (cx, cy, r);
				
				double sx = 5;
				double sy = 5;
				double ox = cx - sx/2 - 0.5;
				double oy = cy - sy/2 - 0.5;
				
				using (Drawing.Path path = new Drawing.Path ())
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
					
					graphics.Rasterizer.AddOutline (path, 0.5);
				}
				
				graphics.RenderSolid (color_2);
			}
			
			protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
			{
				if (! this.drag_behavior.ProcessMessage (message, pos))
				{
					base.ProcessMessage (message, pos);
				}
			}
			
			
			#region IDragBehaviorHost Members
			public Drawing.Point				DragLocation
			{
				get
				{
					Drawing.Point pos = this.MapClientToScreen (new Drawing.Point (this.Client.Size.Width / 2, this.Client.Size.Height / 2));
					
					pos.X = pos.X - this.magnifier.zoom_window.ClientSize.Width  / 2 - 2;
					pos.Y = pos.Y - this.magnifier.zoom_window.ClientSize.Height / 2 - 2;
					
					return pos;
				}
			}

			public bool OnDragBegin(Drawing.Point cursor)
			{
				if (this.magnifier == null)
				{
					this.magnifier = new Magnifier ();
					this.magnifier.IsColorPicker = true;
					this.magnifier.HotColorChanged += new Support.EventHandler (this.HandleMagnifierHotColorChanged);
				}
				
				MouseCursor.Hide ();
				
				this.magnifier.zoom_window.WindowLocation = this.DragLocation;
				this.magnifier.zoom_view.Invalidate ();
				this.magnifier.Show ();
				
				this.is_sampling = true;
				
				return true;
			}

			public void OnDragging(DragEventArgs e)
			{
				this.magnifier.zoom_window.WindowLocation = this.DragLocation + e.Offset;
				this.magnifier.zoom_view.Invalidate ();
			}

			public void OnDragEnd()
			{
				this.is_sampling = false;
				
				this.magnifier.Hide ();
				
				MouseCursor.Show ();
			}
			#endregion
			
			private void HandleMagnifierHotColorChanged(object sender)
			{
				if (this.is_sampling)
				{
					this.color = this.magnifier.HotColor;
					this.OnHotColorChanged ();
				}
			}
			
			protected virtual void OnHotColorChanged()
			{
				if (this.HotColorChanged != null)
				{
					this.HotColorChanged (this);
				}
			}
			
			
			public event Support.EventHandler	HotColorChanged;
			
			
			private bool						is_sampling;
			private Behaviors.DragBehavior		drag_behavior;
			private Magnifier					magnifier;
			private Drawing.Color				color = Drawing.Color.Empty;
		}
	}
}
