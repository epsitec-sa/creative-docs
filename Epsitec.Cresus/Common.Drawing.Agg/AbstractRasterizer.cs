//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The AbstractRasterizer is the base classe used by all rasterizers. See the
	/// Rasterizer class for details.
	/// </summary>
	public abstract class AbstractRasterizer : System.IDisposable
	{
		public FillMode							FillMode
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
		
		public double							Gamma
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
		
		public Transform						Transform
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
		
		
		public abstract void SetClipBox(double x1, double y1, double x2, double y2);
		public abstract void ResetClipBox();
		
		public abstract void AddSurface(Path path);
		
		public abstract void AddOutline(Path path, double width);
		public abstract void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit);
		
		public abstract void AddGlyph(Font font, int glyph, double x, double y, double scale);
		public abstract void AddGlyph(Font font, int glyph, double x, double y, double scale, double sx, double sy);
		
		public abstract void Render(Renderers.Solid renderer);
		public abstract void Render(Renderers.Image renderer);
		public abstract void Render(Renderers.Gradient renderer);

		public abstract bool HitTest(double x, double y);
		
		public void AddOutline(Path path)
		{
			this.AddOutline (path, 1);
		}
		
		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join)
		{
			this.AddOutline (path, width, cap, join, 4.0);
		}

		
		public void AddGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx)
		{
			if (font.IsSynthetic)
			{
				if (sx == null)
				{
					for (int i = 0; i < glyphs.Length; i++)
					{
						this.AddGlyph (font, glyphs[i], x[i], y[i], scale);
					}
				}
				else
				{
					for (int i = 0; i < glyphs.Length; i++)
					{
						this.AddGlyph (font, glyphs[i], x[i], y[i], scale, sx[i], 1.0);
					}
				}
			}
			else
			{
				this.AddPlainGlyphs (font, scale, glyphs, x, y, sx);
			}
		}
		
		
		public double AddText(Font font, string text, double x, double y, double scale)
		{
			if (font.IsSynthetic)
			{
				Transform font_transform = font.SyntheticTransform;
				
				font_transform.Scale (scale);
				
				font_transform.TX = x;
				font_transform.TY = y;
				
				switch (font.SyntheticFontMode)
				{
					case SyntheticFontMode.Oblique:
						return this.AddPlainText (font, text, font_transform.XX, font_transform.XY, font_transform.YX, font_transform.YY, font_transform.TX, font_transform.TY);
					
					default:
						break;
				}
			}
			
			return this.AddPlainText (font, text, scale, 0, 0, scale, x, y);
		}
		
		
		protected abstract void SyncFillMode();
		protected abstract void SyncGamma();
		protected abstract void SyncTransform();
		
		protected abstract void AddPlainGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx);
		protected abstract double AddPlainText(Font font, string text, double xx, double xy, double yx, double yy, double tx, double ty);
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected abstract void Dispose(bool disposing);
		
		protected FillMode						fill_mode = FillMode.NonZero;
		protected double						gamma     = 1.0;
		protected Transform						transform = new Transform ();
	}
}
