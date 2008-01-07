//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Tools
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// The <c>Magnifier</c> class manages the rounded magnifier window
	/// which is used by the color picker.
	/// </summary>
	public class Magnifier
	{
		public Magnifier()
		{
			this.zoom_window = new Window ();
			this.zoom_view   = new MagnifierZoomView (this);
			
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

		internal MagnifierZoomView				ZoomView
		{
			get
			{
				return this.zoom_view;
			}
		}

		internal Window							ZoomWindow
		{
			get
			{
				return this.zoom_window;
			}
		}

		internal Drawing.Image					Bitmap
		{
			get
			{
				return this.bitmap;
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
		
		
		public event EventHandler				HotColorChanged;
		public event EventHandler				SampleRadiusChanged;
		public event EventHandler				DisplayRadiusChanged;
		
		
		private Drawing.Color					hot_color;
		private double							display_radius;
		private int								sample_radius;
		private Drawing.Image					bitmap;
		private MagnifierZoomView				zoom_view;
		private Window							zoom_window;
		private bool							color_picker;
	}
}
