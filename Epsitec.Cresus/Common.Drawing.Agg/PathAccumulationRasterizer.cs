//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The path accumulation rasterizer is a dummy rasterizer; it simply
	/// accumulates all paths that are to be painted.
	/// </summary>
	public class PathAccumulationRasterizer : AbstractRasterizer
	{
		public PathAccumulationRasterizer()
		{
		}
		
		
		public Path[] GetPaths()
		{
			return (Path[]) this.list.ToArray (typeof (Path));
		}
		
		
		public override void SetClipBox(double x1, double y1, double x2, double y2)
		{
		}
		
		public override void ResetClipBox()
		{
		}
		
		
		public override void AddSurface(Path path)
		{
			if ((path != null) &&
				(path.IsValid))
			{
				Path temp = new Path ();
				temp.Append (path, this.transform, this.approximation);
				this.list.Add (temp);
			}
		}
		
		public override void AddOutline(Path path, double width)
		{
			if ((path != null) &&
				(path.IsValid))
			{
				Path temp = new Path ();
				temp.Append (path, this.transform, this.approximation, width);
				this.list.Add (temp);
			}
		}
		
		public override void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
			if ((path != null) &&
				(path.IsValid))
			{
				Path temp1 = new Path ();
				Path temp2 = new Path ();
				temp1.Append (path, this.transform, this.approximation);
				temp2.Append (temp1, width, cap, join, miter_limit, this.approximation, false);
				this.list.Add (temp2);
				temp1.Dispose ();
			}
		}
		
		public override void AddGlyph(Font font, int glyph, double x, double y, double scale)
		{
			Path temp = new Path ();
			temp.Append (font, glyph, x, y, scale);
			this.AddSurface (temp);
			temp.Dispose ();
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
				Transform ft = font.SyntheticTransform;
				
				ft.TX = x;
				ft.TY = y;
				
				ft.MultiplyByPostfix (Transform.FromScale (sx * scale, sy * scale));
				
				Path temp = new Path ();
				temp.Append (font, glyph, ft.XX, ft.XY, ft.YX, ft.YY, ft.TX, ft.TY);
				this.AddSurface (temp);
				temp.Dispose ();
			}
		}
		
		
		public override void Render(Renderers.Solid renderer)
		{
		}
		
		public override void Render(Renderers.Image renderer)
		{
		}
		
		public override void Render(Renderers.Gradient renderer)
		{
		}
		
		
		protected override void SyncFillMode()
		{
		}
		
		protected override void SyncGamma()
		{
		}
		
		protected override void SyncTransform()
		{
		}
		
		
		protected override void AddPlainGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx)
		{
			for (int i = 0; i < glyphs.Length; i++)
			{
				this.AddGlyph (font, glyphs[i], x[i], y[i], scale, sx == null ? 1.0 : sx[i], 1.0);
			}
		}
		
		protected override double AddPlainText(Font font, string text, double xx, double xy, double yx, double yy, double tx, double ty)
		{
			throw new System.NotImplementedException ();
		}
		
		
		protected override void Dispose(bool disposing)
		{
		}
		
		
		
		private double							approximation = 0;
		private System.Collections.ArrayList	list = new System.Collections.ArrayList();
	}
}
