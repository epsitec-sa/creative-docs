namespace Epsitec.Common.Drawing
{
	public class Path : System.IDisposable
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
			AntiGrain.Path.RemoveAll (this.agg_path);
		}
		
		public void MoveTo(double x, double y)
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.MoveTo (this.agg_path, x, y);
		}
		
		public void LineTo(double x, double y)
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.LineTo (this.agg_path, x, y);
		}
		
		public void CurveTo(double x_c1, double y_c1, double x_c2, double y_c2, double x, double y)
		{
			this.CreateOnTheFly ();
			this.has_curve = true;
			AntiGrain.Path.Curve4 (this.agg_path, x_c1, y_c1, x_c2, y_c2, x, y);
		}
		
		public void CurveTo(double x_c, double y_c, double x, double y)
		{
			this.CreateOnTheFly ();
			this.has_curve = true;
			AntiGrain.Path.Curve3 (this.agg_path, x_c, y_c, x, y);
		}
		
		public void Close()
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.Close (this.agg_path);
		}
		
		public void StartNewPath()
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.AddNewPath (this.agg_path);
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
			this.has_curve |= path.has_curve;
			AntiGrain.Path.AppendPath (this.agg_path, path.agg_path, xx, xy, yx, yy, tx, ty, 0);
		}
		
		public void Append(Path path, double xx, double xy, double yx, double yy, double tx, double ty, double bold_width)
		{
			this.CreateOnTheFly ();
			this.has_curve |= path.has_curve;
			AntiGrain.Path.AppendPath (this.agg_path, path.agg_path, xx, xy, yx, yy, tx, ty, bold_width);
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
			this.has_curve = true;
			AntiGrain.Path.AppendGlyph (this.agg_path, font.Handle, glyph, xx, xy, yx, yy, tx, ty, 0);
		}
		
		public void Append(Font font, int glyph, double xx, double xy, double yx, double yy, double tx, double ty, double bold_width)
		{
			this.CreateOnTheFly ();
			this.has_curve = true;
			AntiGrain.Path.AppendGlyph (this.agg_path, font.Handle, glyph, xx, xy, yx, yy, tx, ty, bold_width);
		}
		
		
		public void ComputeBounds(out double x1, out double y1, out double x2, out double y2)
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.ComputeBounds (this.agg_path, out x1, out y1, out x2, out y2);
		}
		
		public Rectangle ComputeBounds()
		{
			double x1, y1, x2, y2;
			this.ComputeBounds (out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2-x1, y2-y1);
		}
		
		
		public void GetElements(out PathElement[] elements, out Point[] points)
		{
			this.CreateOnTheFly ();
			
			int n = AntiGrain.Path.ElemCount (this.agg_path);
			
			int[]    e = new int[n];
			double[] x = new double[n];
			double[] y = new double[n];
			
			AntiGrain.Path.ElemGet (this.agg_path, n, e, x, y);
			
			elements = new PathElement[n];
			points   = new Point[n];
			
			for (int i = 0; i < n; i++)
			{
				elements[i] = (PathElement) e[i];
				points[i]   = new Point (x[i], y[i]);
			}
		}
		

		public override string ToString()
		{
			PathElement[] elements;
			Point[] points;
			
			this.GetElements (out elements, out points);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			bool add_space = false;
			
			for (int i = 0; i < elements.Length; i++)
			{
				if (add_space)
				{
					buffer.Append (";\r\n");
				}
				
				add_space = true;
				
				switch (elements[i] & PathElement.MaskCommand)
				{
					case PathElement.MoveTo:
						buffer.Append ("move_to ");
						buffer.Append (System.String.Format ("({0:0.00}, {1:0.00})", points[i].X, points[i].Y));
						break;
					case PathElement.LineTo:
						buffer.Append ("line_to ");
						buffer.Append (System.String.Format ("({0:0.00}, {1:0.00})", points[i].X, points[i].Y));
						break;
					case PathElement.Curve3:
						buffer.Append ("curve3 ");
						buffer.Append (System.String.Format ("({0:0.00}, {1:0.00})", points[i].X, points[i].Y));
						break;
					case PathElement.Curve4:
						buffer.Append ("curve4 ");
						buffer.Append (System.String.Format ("({0:0.00}, {1:0.00})", points[i].X, points[i].Y));
						break;
					case PathElement.Arc:
						buffer.Append ("arc ");
						buffer.Append (System.String.Format ("({0:0.00}, {1:0.00})", points[i].X, points[i].Y));
						break;
					default:
						add_space = false;
						break;
				}
				
				if ((elements[i] & PathElement.FlagClose) != 0)
				{
					if (add_space)
					{
						buffer.Append (";\r\n");
					}
					
					buffer.Append ("close");
					add_space = true;
				}
			}
			
			buffer.Append ("\r\n");
			return buffer.ToString ();
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
				AntiGrain.Path.Delete (this.agg_path);
				this.agg_path = System.IntPtr.Zero;
			}
		}
		
		protected virtual void CreateOnTheFly()
		{
			if (this.agg_path == System.IntPtr.Zero)
			{
				this.agg_path = AntiGrain.Path.New ();
			}
		}
		
		
		private System.IntPtr			agg_path;
		private bool					has_curve = false;
	}
	
	[System.Flags]
	public enum PathElement
	{
		Stop		= 0,
		MoveTo		= 1,
		LineTo		= 2,
		Curve3		= 3,
		Curve4		= 4,
		Arc			= 5,
		
		MaskCommand	= 0x0f,
		MaskFlags	= 0xf0,
		
		FlagCCW		= 0x10,
		FlagCW		= 0x20,
		FlagClose	= 0x40,
	}
}
