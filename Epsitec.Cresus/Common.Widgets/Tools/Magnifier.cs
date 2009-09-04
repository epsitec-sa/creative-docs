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
	public class Magnifier
	{
		public Magnifier()
		{
			this.zoomWindow = new Window ();
			this.zoomView   = new MagnifierZoomView (this);
			
			this.zoomWindow.MakeTopLevelWindow ();
			this.zoomWindow.MakeFramelessWindow ();
			this.zoomWindow.MakeLayeredWindow ();
			
			this.zoomWindow.ClientSize = new Drawing.Size (111, 111);
			this.zoomWindow.Root.BackColor = Drawing.Color.Transparent;
			
			this.zoomView.Dock   = DockStyle.Fill;
			this.zoomView.SetParent (this.zoomWindow.Root);
			this.zoomView.Focus ();
			
			this.DisplayRadius = 55;
			this.SampleRadius  = 3;
		}
		
		
		public void Show()
		{
			this.zoomWindow.Show ();
		}
		
		public void Hide()
		{
			this.zoomWindow.Hide ();
		}
		
		
		public double							DisplayRadius
		{
			get
			{
				return this.displayRadius;
			}
			set
			{
				if (value < 40)  value = 40;
				if (value > 200) value = 200;
				
				if (this.displayRadius != value)
				{
					this.displayRadius = value;
					this.OnDisplayRadiusChanged ();
				}
			}
		}
		
		public int								SampleRadius
		{
			get
			{
				return this.sampleRadius;
			}
			set
			{
				if (value < 2)  value = 2;
				if (value > 10) value = 10;
				
				if (this.sampleRadius != value)
				{
					this.sampleRadius = value;
					this.OnSampleRadiusChanged ();
				}
			}
		}
		
		public Drawing.Color					HotColor
		{
			get
			{
				return this.hotColor;
			}
				
			set
			{
				if (this.hotColor != value)
				{
					this.hotColor = value;
					this.OnHotColorChanged ();
				}
			}
		}
		
		public bool								IsColorPicker
		{
			get
			{
				return this.colorPicker;
			}
			set
			{
				if (this.colorPicker != value)
				{
					this.colorPicker = value;
					this.zoomView.Invalidate ();
				}
			}
		}

		internal MagnifierZoomView				ZoomView
		{
			get
			{
				return this.zoomView;
			}
		}

		public Window							ZoomWindow
		{
			get
			{
				return this.zoomWindow;
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
				
				this.zoomView.Invalidate ();
			}
		}
		
		protected virtual void UpdateWindowSize()
		{
			double dx = this.DisplayRadius * 2 + 1;
			double dy = dx;
			
			this.zoomWindow.ClientSize = new Drawing.Size (dx, dy);
		}
		
		
		public event EventHandler				HotColorChanged;
		public event EventHandler				SampleRadiusChanged;
		public event EventHandler				DisplayRadiusChanged;
		
		
		private Drawing.Color					hotColor;
		private double							displayRadius;
		private int								sampleRadius;
		private Drawing.Image					bitmap;
		private MagnifierZoomView				zoomView;
		private Window							zoomWindow;
		private bool							colorPicker;
	}
}
