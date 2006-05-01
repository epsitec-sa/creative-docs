//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The rasterizer transforms paths and glyphs into coverage information
	/// in AGG. This coverage information is then used by the renderer to
	/// produce actual pixels.
	/// </summary>
	public class Rasterizer : AbstractRasterizer
	{
		public Rasterizer()
		{
		}
		
		~Rasterizer()
		{
			this.Dispose (false);
		}
		
		
		public override void SetClipBox(double x1, double y1, double x2, double y2)
		{
			//	The clip box is specified in destination pixel coordinates (without any transform
			//	matrix).
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.SetClipBox (this.agg_ras, x1, y1, x2, y2);
		}
		
		public override void ResetClipBox()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.ResetClipBox (this.agg_ras);
		}
		
		
		public override void AddSurface(Path path)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPath (this.agg_ras, path.Handle, path.ContainsCurves);
			}
		}
		
		public override void AddOutline(Path path, double width)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPathStroke1 (this.agg_ras, path.Handle, width, path.ContainsCurves);
			}
		}
		
		public override void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPathStroke2 (this.agg_ras, path.Handle, width, (int) cap, (int) join, miter_limit, path.ContainsCurves);
			}
		}
		
		public override void AddGlyph(Font font, int glyph, double x, double y, double scale)
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
		
		public override void AddGlyph(Font font, int glyph, double x, double y, double scale, double sx, double sy)
		{
			if ((sx == 1.0) &&
				(sy == 1.0))
			{
				this.AddGlyph (font, glyph, x, y, scale);
			}
			else
			{
				this.CreateOnTheFly ();
				
				Transform font_transform = font.SyntheticTransform;
				
				font_transform.TX = x;
				font_transform.TY = y;
				font_transform.MultiplyBy (this.transform);
				font_transform.MultiplyByPostfix (Transform.FromScale (sx, sy));
				
				AntiGrain.Rasterizer.SetTransform (this.agg_ras, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
				AntiGrain.Rasterizer.AddGlyph(this.agg_ras, font.Handle, glyph, 0, 0, scale);
				AntiGrain.Rasterizer.SetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
			}
		}
		
		
		public override void Render(Renderers.Solid renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderSolid (this.agg_ras, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.agg_ras);
		}
		
		public override void Render(Renderers.Image renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderImage (this.agg_ras, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.agg_ras);
		}
		
		public override void Render(Renderers.Gradient renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderGradient (this.agg_ras, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.agg_ras);
		}

		public override bool HitTest(double x, double y)
		{
			int xx = (int) (x + 0.5);
			int yy = (int) (y + 0.5);
			
			this.CreateOnTheFly ();
			return AntiGrain.Rasterizer.HitTest (this.agg_ras, xx, yy);
		}
		
		
		protected override void SyncFillMode()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.FillingRule (this.agg_ras, (int) this.fill_mode);
		}
		
		protected override void SyncGamma()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.Gamma (this.agg_ras, this.gamma);
		}
		
		protected override void SyncTransform()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.SetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}
		
		
		protected override void AddPlainGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.AddGlyphs (this.agg_ras, font.Handle, scale, glyphs, x, y, sx);
		}
		
		protected override double AddPlainText(Font font, string text, double xx, double xy, double yx, double yy, double tx, double ty)
		{
			this.CreateOnTheFly ();
			return AntiGrain.Rasterizer.AddText (this.agg_ras, font.Handle, text, 0, xx, xy, yx, yy, tx, ty);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			//	Free unmanaged resources :
			
			if (this.agg_ras != System.IntPtr.Zero)
			{
				AntiGrain.Rasterizer.Delete (this.agg_ras);
				this.agg_ras = System.IntPtr.Zero;
			}
		}
		
		
		private void CreateOnTheFly()
		{
			if (this.agg_ras == System.IntPtr.Zero)
			{
				this.agg_ras = AntiGrain.Rasterizer.New ();
			}
		}
		
		
		private System.IntPtr					agg_ras;
	}
}
