namespace Epsitec.Common.Drawing
{
	public enum CapStyle
	{
		Butt   = 0,
		Square = 1,
		Round  = 2
	}
	
	public enum JoinStyle
	{
		Miter = 0,
		Round = 2,
		Bevel = 3,
	}
	
	public enum FillMode
	{
		EvenOdd		= 1,
		NonZero		= 2
	}
	
	
	public class Rasterizer : System.IDisposable
	{
		public Rasterizer()
		{
		}
		
		~Rasterizer()
		{
			this.Dispose (false);
		}
		
		
		public FillMode					FillMode
		{
			get
			{
				return this.fill_mode;
			}
			set
			{
				if (this.fill_mode != value)
				{
					this.fill_mode = value;
					this.SyncFillMode ();
				}
			}
		}
		
		public double					Gamma
		{
			get
			{
				return this.gamma;
			}
			set
			{
				if (this.gamma != value)
				{
					this.gamma = value;
					this.SyncGamma ();
				}
			}
		}
		
		public Transform				Transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException ("Rasterizer.Transform");
				}
				
				if (this.transform != value)
				{
					this.transform = new Transform (value);
					this.SyncTransform ();
				}
			}
		}
		
		
		protected void SyncFillMode()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.FillingRule (this.agg_ras, (int) this.fill_mode);
		}
		
		protected void SyncGamma()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.Gamma (this.agg_ras, this.gamma);
		}
		
		protected void SyncTransform()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.SetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}
		
		
		internal void SetClipBox(double x1, double y1, double x2, double y2)
		{
			//	The clip box is specified in destination pixel coordinates (without any transform
			//	matrix).
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.SetClipBox (this.agg_ras, x1, y1, x2, y2);
		}
		
		internal void ResetClipBox()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.ResetClipBox (this.agg_ras);
		}
		
		
		public void AddSurface(Path path)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPath (this.agg_ras, path.Handle, path.ContainsCurves);
			}
		}
		
		public void AddOutline(Path path)
		{
			this.AddOutline (path, 1);
		}
		
		public void AddOutline(Path path, double width)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPathStroke1 (this.agg_ras, path.Handle, width, path.ContainsCurves);
			}
		}
		
		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join)
		{
			this.AddOutline (path, width, cap, join, 4.0);
		}

		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPathStroke2 (this.agg_ras, path.Handle, width, (int) cap, (int) join, miter_limit, path.ContainsCurves);
			}
		}
		
		public void AddGlyph(Font font, int glyph, double x, double y, double scale)
		{
			this.CreateOnTheFly ();
			
			if (font.IsSynthetic)
			{
				Transform font_transform = font.SyntheticTransform;
				
				font_transform.TX = x;
				font_transform.TY = y;
				
				switch (font.SyntheticFontMode)
				{
					case SyntheticFontMode.Oblique:
						font_transform.MultiplyBy (this.transform);
						AntiGrain.Rasterizer.SetTransform (this.agg_ras, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
						AntiGrain.Rasterizer.AddGlyph(this.agg_ras, font.Handle, glyph, 0, 0, scale);
						AntiGrain.Rasterizer.SetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
						return;
					
					default:
						break;
				}
			}
			
			AntiGrain.Rasterizer.AddGlyph(this.agg_ras, font.Handle, glyph, x, y, scale);
		}
		
		public double AddText(Font font, string text, double x, double y, double scale)
		{
			this.CreateOnTheFly ();
			
			if (font.IsSynthetic)
			{
				Transform font_transform = font.SyntheticTransform;
				
				font_transform.Scale (scale);
				
				font_transform.TX = x;
				font_transform.TY = y;
				
				switch (font.SyntheticFontMode)
				{
					case SyntheticFontMode.Oblique:
						return AntiGrain.Rasterizer.AddText (this.agg_ras, font.Handle, text, 0, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
					
					default:
						break;
				}
			}
			
			return AntiGrain.Rasterizer.AddText (this.agg_ras, font.Handle, text, 0, scale, 0, 0, scale, x, y);
		}
		
		
		public void Render(Renderers.Solid renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderSolid (this.agg_ras, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.agg_ras);
		}
		
		public void Render(Renderers.Image renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderImage (this.agg_ras, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.agg_ras);
		}
		
		public void Render(Renderers.Gradient renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderGradient (this.agg_ras, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.agg_ras);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (this.agg_ras != System.IntPtr.Zero)
			{
				AntiGrain.Rasterizer.Delete (this.agg_ras);
				this.agg_ras = System.IntPtr.Zero;
			}
		}
		
		protected void CreateOnTheFly()
		{
			if (this.agg_ras == System.IntPtr.Zero)
			{
				this.agg_ras = AntiGrain.Rasterizer.New ();
			}
		}
		
		
		
		
		
		private FillMode						fill_mode = FillMode.NonZero;
		private double							gamma     = 1.0;
		private Transform						transform = new Transform ();
		private System.IntPtr					agg_ras;
	}
}
