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
			Agg.Library.AggRasterizerFillingRule (this.agg_ras, (int) this.fill_mode);
		}
		
		protected override void SyncGamma()
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerGamma (this.agg_ras, this.gamma);
		}
		
		protected override void SyncTransform()
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerSetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}
		
		
		
		public override void SetClipBox(double x1, double y1, double x2, double y2)
		{
			//	The clip box is specified in destination pixel coordinates (without any transform
			//	matrix).
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerSetClipBox (this.agg_ras, x1, y1, x2, y2);
		}
		
		public override void ResetClipBox()
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerResetClipBox (this.agg_ras);
		}
		
		
		public override void AddSurface(Path path)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerAddPath (this.agg_ras, path.Handle, path.ContainsCurves);
		}
		
		public override void AddOutline(Path path, double width)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerAddPathStroke1 (this.agg_ras, path.Handle, width, path.ContainsCurves);
		}
		
		public override void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerAddPathStroke2 (this.agg_ras, path.Handle, width, (int) cap, (int) join, miter_limit, path.ContainsCurves);
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
						Agg.Library.AggRasterizerSetTransform (this.agg_ras, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
						Agg.Library.AggRasterizerAddGlyph(this.agg_ras, font.Handle, glyph, 0, 0, scale);
						Agg.Library.AggRasterizerSetTransform (this.agg_ras, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
						return;
					
					default:
						break;
				}
			}
			
			Agg.Library.AggRasterizerAddGlyph(this.agg_ras, font.Handle, glyph, x, y, scale);
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
						return Agg.Library.AggRasterizerAddText (this.agg_ras, font.Handle, text, 0, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
					
					default:
						break;
				}
			}
			
			return Agg.Library.AggRasterizerAddText (this.agg_ras, font.Handle, text, 0, scale, 0, 0, scale, x, y);
		}
		
		
		public override void Render(Renderer.Solid renderer)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerRenderSolid (this.agg_ras, renderer.Handle);
			Agg.Library.AggRasterizerClear (this.agg_ras);
		}
		
		public override void Render(Renderer.Image renderer)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerRenderImage (this.agg_ras, renderer.Handle);
			Agg.Library.AggRasterizerClear (this.agg_ras);
		}
		
		public override void Render(Renderer.Gradient renderer)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggRasterizerRenderGradient (this.agg_ras, renderer.Handle);
			Agg.Library.AggRasterizerClear (this.agg_ras);
		}
		
		
		protected override void Dispose(bool disposing)
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
		
		
		protected void CreateOnTheFly()
		{
			if (this.agg_ras == System.IntPtr.Zero)
			{
				this.agg_ras = Agg.Library.AggRasterizerNew ();
			}
		}
		
		
		
		private System.IntPtr					agg_ras;
	}
}
