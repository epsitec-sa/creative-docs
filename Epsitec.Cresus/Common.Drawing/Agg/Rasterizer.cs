namespace Epsitec.Common.Drawing.Agg
{
	/// <summary>
	/// Implémentation de la classe Rasterizer basée sur AGG.
	/// </summary>
	public class Rasterizer : Epsitec.Common.Drawing.Rasterizer
	{
		public Rasterizer()
		{
		}
		
		
		protected override void SyncFillMode()
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerFillingRule (this.agg_ras, (int) this.fill_mode);
		}
		
		protected override void SyncGamma()
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerGamma (this.agg_ras, this.gamma);
		}
		
		protected override void SyncTransform()
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerSetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}
		
		
		
		public override void SetClipBox(double x1, double y1, double x2, double y2)
		{
			//	The clip box is specified in destination pixel coordinates (without any transform
			//	matrix).
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerSetClipBox (this.agg_ras, x1, y1, x2, y2);
		}
		
		public override void ResetClipBox()
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerResetClipBox (this.agg_ras);
		}
		
		
		public override void AddSurface(Path path)
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerAddPath (this.agg_ras, path.Handle, path.ContainsCurves);
		}
		
		public override void AddOutline(Path path, double width)
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerAddPathStroke1 (this.agg_ras, path.Handle, width, path.ContainsCurves);
		}
		
		public override void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerAddPathStroke2 (this.agg_ras, path.Handle, width, (int) cap, (int) join, miter_limit, path.ContainsCurves);
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
						AntiGrain.Interface.RasterizerSetTransform (this.agg_ras, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
						AntiGrain.Interface.RasterizerAddGlyph(this.agg_ras, font.Handle, glyph, 0, 0, scale);
						AntiGrain.Interface.RasterizerSetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
						return;
					
					default:
						break;
				}
			}
			
			AntiGrain.Interface.RasterizerAddGlyph(this.agg_ras, font.Handle, glyph, x, y, scale);
		}
		
		public override double AddText(Font font, string text, double x, double y, double scale)
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
						return AntiGrain.Interface.RasterizerAddText (this.agg_ras, font.Handle, text, 0, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
					
					default:
						break;
				}
			}
			
			return AntiGrain.Interface.RasterizerAddText (this.agg_ras, font.Handle, text, 0, scale, 0, 0, scale, x, y);
		}
		
		
		public override void Render(Renderer.Solid renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerRenderSolid (this.agg_ras, renderer.Handle);
			AntiGrain.Interface.RasterizerClear (this.agg_ras);
		}
		
		public override void Render(Renderer.Image renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerRenderImage (this.agg_ras, renderer.Handle);
			AntiGrain.Interface.RasterizerClear (this.agg_ras);
		}
		
		public override void Render(Renderer.Gradient renderer)
		{
			this.CreateOnTheFly ();
			AntiGrain.Interface.RasterizerRenderGradient (this.agg_ras, renderer.Handle);
			AntiGrain.Interface.RasterizerClear (this.agg_ras);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	nothing
			}
			
			if (this.agg_ras != System.IntPtr.Zero)
			{
				AntiGrain.Interface.RasterizerDelete (this.agg_ras);
				this.agg_ras = System.IntPtr.Zero;
			}
		}
		
		
		protected void CreateOnTheFly()
		{
			if (this.agg_ras == System.IntPtr.Zero)
			{
				this.agg_ras = AntiGrain.Interface.RasterizerNew ();
			}
		}
		
		
		
		private System.IntPtr					agg_ras;
	}
}
