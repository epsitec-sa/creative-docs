namespace Epsitec.Common.Drawing
{
	public enum CapStyle
	{
		Butt, Square, Round
	}
	
	public enum JoinStyle
	{
		Miter, Round, Bevel
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
					this.CreateOnTheFly ();
					Agg.Library.AggRasterizerFillingRule (this.agg_ras, (int) value);
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
					this.CreateOnTheFly ();
					Agg.Library.AggRasterizerGamma (this.agg_ras, value);
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
					this.CreateOnTheFly ();
					this.transform = new Transform (value);
					Agg.Library.AggRasterizerSetTransform (this.agg_ras, value.XX, value.XY, value.YX, value.YY, value.TX, value.TY);
				}
			}
		}
		
		
		public void SetClipBox(double x1, double y1, double x2, double y2)
		{
			//	The clip box is specified in destination pixel coordinates (without any transform
			//	matrix).
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerSetClipBox (this.agg_ras, x1, y1, x2, y2);
		}
		
		public void ResetClipBox()
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerResetClipBox (this.agg_ras);
		}
		
		
		public void AddSurface(Path path)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerAddPath (this.agg_ras, path.Handle, path.ContainsCurves);
		}
		
		public void AddOutline(Path path)
		{
			this.AddOutline (path, 1);
		}
		
		public void AddOutline(Path path, double width)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerAddPathStroke1 (this.agg_ras, path.Handle, width, path.ContainsCurves);
		}
		
		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join)
		{
			this.AddOutline (path, width, cap, join, 4.0);
		}

		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerAddPathStroke2 (this.agg_ras, path.Handle, width, (int) cap, (int) join, miter_limit, path.ContainsCurves);
		}
		
		public void AddGlyph(Font font, int glyph, double x, double y, double scale)
		{
			this.CreateOnTheFly ();
			
			if (font.IsSynthetic)
			{
				Transform transform = new Transform (this.transform);
				
				switch (font.SyntheticFontMode)
				{
					case SyntheticFontMode.Oblique:
						transform.MultiplyBy (new Transform (1, System.Math.Sin (Font.ObliqueAngle * System.Math.PI / 180.0), 0, 1, x, y));
						Agg.Library.AggRasterizerSetTransform (this.agg_ras, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY);
						Agg.Library.AggRasterizerAddGlyph(this.agg_ras, font.Handle, glyph, 0, 0, scale);
						transform = this.transform;
						Agg.Library.AggRasterizerSetTransform (this.agg_ras, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY);
						return;
					
					default:
						break;
				}
			}
			
			Agg.Library.AggRasterizerAddGlyph(this.agg_ras, font.Handle, glyph, x, y, scale);
		}
		
		
		public void Render(Renderer.Solid renderer)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerRenderSolid (this.agg_ras, renderer.Handle);
			Agg.Library.AggRasterizerClear (this.agg_ras);
		}
		
		public void Render(Renderer.Image renderer)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerRenderImage (this.agg_ras, renderer.Handle);
			Agg.Library.AggRasterizerClear (this.agg_ras);
		}
		
		public void Render(Renderer.Gradient renderer)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerRenderGradient (this.agg_ras, renderer.Handle);
			Agg.Library.AggRasterizerClear (this.agg_ras);
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	nothing
			}
			
			if (this.agg_ras != System.IntPtr.Zero)
			{
				Agg.Library.AggRasterizerDelete (this.agg_ras);
				this.agg_ras = System.IntPtr.Zero;
			}
		}
		
		
		protected virtual void CreateOnTheFly()
		{
			if (this.agg_ras == System.IntPtr.Zero)
			{
				this.agg_ras = Agg.Library.AggRasterizerNew ();
			}
		}
		
		
		private System.IntPtr					agg_ras;
		private FillMode						fill_mode = FillMode.NonZero;
		private Transform						transform = new Transform ();
		private double							gamma = 1.0;
	}
}
