namespace Epsitec.Common.Drawing
{
	public class Path
	{
		public Path()
		{
		}
		
		~Path()
		{
			this.Dispose (false);
		}
		
		
		public System.IntPtr			Handle
		{
			get { return this.agg_path; }
		}
		
		public bool						ContainsCurves
		{
			get { return this.has_curve; }
		}
		
		
		public void Clear()
		{
			this.CreateOnTheFly ();
			this.has_curve = false;
			Agg.Library.AggPathRemoveAll (this.agg_path);
		}
		
		public void MoveTo(double x, double y)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathMoveTo (this.agg_path, x, y);
		}
		
		public void LineTo(double x, double y)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathLineTo (this.agg_path, x, y);
		}
		
		public void CurveTo(double x_c1, double y_c1, double x_c2, double y_c2, double x, double y)
		{
			this.CreateOnTheFly ();
			this.has_curve = true;
			Agg.Library.AggPathCurve4 (this.agg_path, x_c1, y_c1, x_c2, y_c2, x, y);
		}
		
		public void CurveTo(double x_c, double y_c, double x, double y)
		{
			this.CreateOnTheFly ();
			this.has_curve = true;
			Agg.Library.AggPathCurve3 (this.agg_path, x_c, y_c, x, y);
		}
		
		public void Close()
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathClose (this.agg_path);
		}
		
		public void StartNewPath()
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathAddNewPath (this.agg_path);
		}
		
		
		public void Append(Path path)
		{
			this.Append (path, 1, 0, 0, 1, 0, 0);
		}
		
		public void Append(Path path, Transform transform)
		{
			this.Append (path, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY);
		}
		
		public void Append(Path path, double xx, double xy, double yx, double yy, double tx, double ty)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathAppendPath (this.agg_path, path.agg_path, xx, xy, yx, yy, tx, ty);
		}
		
		public void Append(Font font, int glyph, double x, double y, double size)
		{
			this.Append (font, glyph, size, 0, 0, size, x, y);
		}
		
		public void Append(Font font, int glyph, Transform transform)
		{
			this.Append (font, glyph, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY);
		}
		
		public void Append(Font font, int glyph, double xx, double xy, double yx, double yy, double tx, double ty)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathAppendGlyph (this.agg_path, font.Handle, glyph, xx, xy, yx, yy, tx, ty);
		}
		
		
		public void ComputeBounds(out double x1, out double y1, out double x2, out double y2)
		{
			this.CreateOnTheFly ();
			Agg.Library.AggPathComputeBounds (this.agg_path, out x1, out y1, out x2, out y2);
		}
		
		public Rectangle ComputeBounds()
		{
			double x1, y1, x2, y2;
			this.ComputeBounds (out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2-x1, y2-y1);
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
				//	No managed stuff here...
			}
			
			if (this.agg_path != System.IntPtr.Zero)
			{
				Agg.Library.AggPathDelete (this.agg_path);
				this.agg_path = System.IntPtr.Zero;
			}
		}
		
		protected virtual void CreateOnTheFly()
		{
			if (this.agg_path == System.IntPtr.Zero)
			{
				this.agg_path = Agg.Library.AggPathNew ();
			}
		}
		
		private System.IntPtr			agg_path;
		private bool					has_curve = false;
	}
}
