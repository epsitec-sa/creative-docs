//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.handle = new Agg.SafeRasterizerHandle ();
		}
		

		public override void SetClipBox(double x1, double y1, double x2, double y2)
		{
			//	The clip box is specified in destination pixel coordinates (without any transform
			//	matrix).
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.SetClipBox (this.handle, x1, y1, x2, y2);
		}
		
		public override void ResetClipBox()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.ResetClipBox (this.handle);
		}
		
		
		public override void AddSurface(Path path)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPath (this.handle, path.Handle, path.ContainsCurves);
			}
		}
		
		public override void AddOutline(Path path, double width)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPathStroke1 (this.handle, path.Handle, width, path.ContainsCurves);
			}
		}
		
		public override void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			if (path != null)
			{
				this.CreateOnTheFly ();
				AntiGrain.Rasterizer.AddPathStroke2 (this.handle, path.Handle, width, (int) cap, (int) join, miter_limit, path.ContainsCurves);
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
						AntiGrain.Rasterizer.SetTransform (this.handle, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
						AntiGrain.Rasterizer.AddGlyph(this.handle, font.Handle, glyph, 0, 0, scale);
						AntiGrain.Rasterizer.SetTransform (this.handle, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
						return;
					
					default:
						break;
				}
			}
			
			AntiGrain.Rasterizer.AddGlyph(this.handle, font.Handle, glyph, x, y, scale);
		}

		protected override void AddPlainGlyphs(Font font, ushort[] glyphs, double[] x, double xx, double xy, double yx, double yy, double tx, double ty)
		{
            if ((glyphs == null) ||
                (glyphs.Length == 0))
            {
                return;
            }
            
			Transform transform = new Transform (xx, xy, yx, yy, tx, ty);

			transform.MultiplyBy (this.transform);
			AntiGrain.Rasterizer.SetTransform (this.handle, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY);
			AntiGrain.Rasterizer.AddGlyphs (this.handle, font.Handle, 1.0, glyphs, x, null, null);
			AntiGrain.Rasterizer.SetTransform (this.handle, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
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
				
				AntiGrain.Rasterizer.SetTransform (this.handle, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
				AntiGrain.Rasterizer.AddGlyph(this.handle, font.Handle, glyph, 0, 0, scale);
				AntiGrain.Rasterizer.SetTransform (this.handle, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
			}
		}
		
		
		public override void Render(Renderers.Solid renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderSolid (this.handle, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.handle);
		}
		
		public override void Render(Renderers.Image renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderImage (this.handle, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.handle);
		}
		
		public override void Render(Renderers.Gradient renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.RenderGradient (this.handle, renderer.Handle);
			AntiGrain.Rasterizer.Clear (this.handle);
		}

		public override bool HitTest(double x, double y)
		{
			int xx = (int) (x + 0.5);
			int yy = (int) (y + 0.5);
			
			this.CreateOnTheFly ();
			return AntiGrain.Rasterizer.HitTest (this.handle, xx, yy);
		}
		
		
		protected override void SyncFillMode()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.FillingRule (this.handle, (int) this.fill_mode);
		}
		
		protected override void SyncGamma()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.Gamma (this.handle, this.gamma);
		}
		
		protected override void SyncTransform()
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.SetTransform (this.handle, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}
		
		
		protected override void AddPlainGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx)
		{
			this.CreateOnTheFly ();
			AntiGrain.Rasterizer.AddGlyphs (this.handle, font.Handle, scale, glyphs, x, y, sx);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			//	Free unmanaged resources :

			this.handle.Close ();
		}
		
		
		private void CreateOnTheFly()
		{
			if (this.handle.IsInvalid)
			{
				this.handle.Create ();
			}
		}
		
		
		private readonly Agg.SafeRasterizerHandle	handle;
	}
}
