//	Copyright � 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		public bool						IsEmpty
		{
			get { return this.is_empty; }
		}
		
		public bool						IsValid
		{
			get { return ! this.is_empty; }
		}
		
		public bool						IsCurrentPointValid
		{
			get { return this.has_current_point; }
		}
		
		public Point					CurrentPoint
		{
			get
			{
				if (this.has_current_point)
				{
					return this.current_point;
				}
				
				throw new System.InvalidOperationException ("No current point defined.");
			}
		}
		
		public double					DefaultZoom
		{
			get
			{
				return this.default_zoom;
			}
			set
			{
				this.default_zoom = value;
			}
		}
		
		
		public void Clear()
		{
			this.CreateOnTheFly ();
			this.has_curve = false;
			this.is_empty  = true;
			this.has_current_point = false;
			AntiGrain.Path.RemoveAll (this.agg_path);
		}
		
		public void MoveTo(Point p)
		{
			this.MoveTo (p.X, p.Y);
		}
		
		public void MoveTo(double x, double y)
		{
			this.CreateOnTheFly ();
			this.is_empty = false;
			this.has_current_point = true;
			this.current_point = new Point (x, y);
			AntiGrain.Path.MoveTo (this.agg_path, x, y);
		}
		
		public void LineTo(Point p)
		{
			this.LineTo (p.X, p.Y);
		}
		
		public void LineTo(double x, double y)
		{
			this.CreateOnTheFly ();
			this.is_empty = false;
			this.has_current_point = true;
			this.current_point = new Point (x, y);
			AntiGrain.Path.LineTo (this.agg_path, x, y);
		}
		
		public void CurveTo(Point c1, Point c2, Point p)
		{
			this.CurveTo (c1.X, c1.Y, c2.X, c2.Y, p.X, p.Y);
		}
		
		public void CurveTo(double x_c1, double y_c1, double x_c2, double y_c2, double x, double y)
		{
			this.CreateOnTheFly ();
			this.is_empty  = false;
			this.has_curve = true;
			this.has_current_point = true;
			this.current_point = new Point (x, y);
			AntiGrain.Path.Curve4 (this.agg_path, x_c1, y_c1, x_c2, y_c2, x, y);
		}
		
		public void CurveTo(Point c, Point p)
		{
			this.CurveTo (c.X, c.Y, p.X, p.Y);
		}
		
		public void CurveTo(double x_c, double y_c, double x, double y)
		{
			this.CreateOnTheFly ();
			this.is_empty  = false;
			this.has_curve = true;
			this.has_current_point = true;
			this.current_point = new Point (x, y);
			AntiGrain.Path.Curve3 (this.agg_path, x_c, y_c, x, y);
		}

		public void ArcTo(double x_c, double y_c, double x, double y)
		{
			this.ArcTo (new Point(x_c, y_c), new Point(x, y));
		}
		
		public void ArcTo(Point c, Point p)
		{
			Point p1 = this.CurrentPoint;
			Point s1 = Point.Scale (p1, c, Path.Kappa);
			Point s2 = Point.Scale (p, c, Path.Kappa);
			this.CurveTo (s1, s2, p);
		}
		
		
		public void ArcToDeg(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcToDeg (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcToDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcToDeg (x, y, rx, ry, a1, a2, ccw, this.default_zoom);
		}
		
		public void ArcToDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximation_zoom)
		{
			a1 = Math.DegToRad(a1);
			a2 = Math.DegToRad(a2);

			this.current_point = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, true);
			this.has_current_point = true;
			this.is_empty = false;
		}
		
		public void ArcToRad(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcToRad (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcToRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcToRad (x, y, rx, ry, a1, a2, ccw, this.default_zoom);
		}
		
		public void ArcToRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximation_zoom)
		{
			this.current_point = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, true);
			this.has_current_point = true;
			this.is_empty = false;
		}
		
		public void ArcDeg(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcDeg (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcDeg (x, y, rx, ry, a1, a2, ccw, this.default_zoom);
		}
		
		public void ArcDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximation_zoom)
		{
			a1 = Math.DegToRad(a1);
			a2 = Math.DegToRad(a2);

			this.current_point = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, false);
			this.has_current_point = true;
			this.is_empty = false;
		}
		
		public void ArcRad(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcRad (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcRad (x, y, rx, ry, a1, a2, ccw, this.default_zoom);
		}
		
		public void ArcRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximation_zoom)
		{
			this.current_point = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, false);
			this.has_current_point = true;
			this.is_empty = false;
		}

		
		public void Close()
		{
			if (! this.is_empty)
			{
				this.CreateOnTheFly ();
				this.has_current_point = false;
				AntiGrain.Path.Close (this.agg_path);
			}
		}
		
		public void StartNewPath()
		{
			if (! this.is_empty)
			{
				this.CreateOnTheFly ();
				this.has_current_point = false;
				AntiGrain.Path.AddNewPath (this.agg_path);
			}
		}
		
		
		public void Append(Path path)
		{
			this.Append (path, 1, 0, 0, 1, 0, 0, this.default_zoom);
		}
		
		public void Append(Path path, double approximation_zoom)
		{
			this.Append (path, 1, 0, 0, 1, 0, 0, approximation_zoom);
		}
		
		public void Append(Path path, Transform transform, double approximation_zoom)
		{
			this.Append (path, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY, approximation_zoom);
		}
		
		public void Append(Path path, double xx, double xy, double yx, double yy, double tx, double ty, double approximation_zoom)
		{
			this.CreateOnTheFly ();
			this.has_curve |= path.has_curve;
			this.is_empty  &= path.is_empty;
			AntiGrain.Path.AppendPath (this.agg_path, path.agg_path, xx, xy, yx, yy, tx, ty, approximation_zoom, 0);
		}
		
		public void Append(Path path, double xx, double xy, double yx, double yy, double tx, double ty, double approximation_zoom, double bold_width)
		{
			this.CreateOnTheFly ();
			this.has_curve |= path.has_curve;
			this.is_empty  &= path.is_empty;
			AntiGrain.Path.AppendPath (this.agg_path, path.agg_path, xx, xy, yx, yy, tx, ty, approximation_zoom, bold_width);
		}
		
		public void Append(Path path, double approximation_zoom, double bold_width)
		{
			this.CreateOnTheFly ();
			this.has_curve |= path.has_curve;
			this.is_empty  &= path.is_empty;
			AntiGrain.Path.AppendPath (this.agg_path, path.agg_path, 1, 0, 0, 1, 0, 0, approximation_zoom, bold_width);
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
			this.is_empty  = false;
			AntiGrain.Path.AppendGlyph (this.agg_path, font.Handle, glyph, xx, xy, yx, yy, tx, ty, 0);
		}
		
		public void Append(Font font, int glyph, double xx, double xy, double yx, double yy, double tx, double ty, double bold_width)
		{
			this.CreateOnTheFly ();
			this.has_curve = true;
			this.is_empty  = false;
			AntiGrain.Path.AppendGlyph (this.agg_path, font.Handle, glyph, xx, xy, yx, yy, tx, ty, bold_width);
		}
		
		
		public void AppendRectangle(double x, double y, double width, double height)
		{
			this.MoveTo (x, y);
			this.LineTo (x+width, y);
			this.LineTo (x+width, y+height);
			this.LineTo (x, y+height);
			this.Close ();
		}
		
		public void AppendRectangle(Rectangle rect)
		{
			this.AppendRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void AppendRectangle(Point p, Size s)
		{
			this.AppendRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AppendCircle(Point c, double r)
		{
			this.AppendCircle (c.X, c.Y, r, r);
		}
		
		public void AppendCircle(Point c, double rx, double ry)
		{
			this.AppendCircle (c.X, c.Y, rx, ry);
		}
		
		public void AppendCircle(double cx, double cy, double r)
		{
			this.AppendCircle (cx, cy, r, r);
		}
		
		public void AppendCircle(double cx, double cy, double rx, double ry)
		{
			Drawing.Path path = new Drawing.Path();
			
			path.MoveTo (cx-rx, cy);
			path.CurveTo (cx-rx, cy+ry*Path.Kappa, cx-rx*Path.Kappa, cy+ry, cx, cy+ry);
			path.CurveTo (cx+rx*Path.Kappa, cy+ry, cx+rx, cy+ry*Path.Kappa, cx+rx, cy);
			path.CurveTo (cx+rx, cy-ry*Path.Kappa, cx+rx*Path.Kappa, cy-ry, cx, cy-ry);
			path.CurveTo (cx-rx*Path.Kappa, cy-ry, cx-rx, cy-ry*Path.Kappa, cx-rx, cy);
			path.Close ();
			
			this.Append (path, 0);
		}
		
		
		public void ComputeBounds(out double x1, out double y1, out double x2, out double y2)
		{
			if (this.is_empty)
			{
				x1 = 0;
				y1 = 0;
				x2 = -1;
				y2 = -1;
			}
			else
			{
				this.CreateOnTheFly ();
				AntiGrain.Path.ComputeBounds (this.agg_path, out x1, out y1, out x2, out y2);
			}
		}
		
		public Rectangle ComputeBounds()
		{
			if (this.is_empty)
			{
				return Rectangle.Empty;
			}
			else
			{
				double x1, y1, x2, y2;
				this.ComputeBounds (out x1, out y1, out x2, out y2);
				return new Rectangle (x1, y1, x2-x1, y2-y1);
			}
		}
		
		
		public void GetElements(out PathElement[] elements, out Point[] points)
		{
			if (this.is_empty)
			{
				elements = new PathElement[0];
				points   = new Point[0];
			}
			else
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
		}
		

		public override string ToString()
		{
			if (this.is_empty)
			{
				return "empty\r\n";
			}
			
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
					case PathElement.EndPoly:
						buffer.Append ("end ");
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
		
		
		public System.Drawing.Drawing2D.GraphicsPath CreateSystemPath()
		{
			PathElement[] elements;
			Point[]       points;
			
			this.GetElements (out elements, out points);
			
			System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath ();
			
			int n = elements.Length;
			
			float ox = 0;
			float oy = 0;
			float x1,x2,x3,xc;
			float y1,y2,y3,yc;
			
			for (int i = 0; i < n; i++)
			{
				switch (elements[i] & PathElement.MaskCommand)
				{
					case PathElement.MoveTo:
						ox = (float) points[i].X;
						oy = (float) points[i].Y;
						gp.StartFigure ();
						break;
					
					case PathElement.LineTo:
						x1 = (float) points[i].X;
						y1 = (float) points[i].Y;
						gp.AddLine (ox, oy, x1, y1);
						ox = x1;
						oy = y1;
						break;
					
					case PathElement.Curve3:
						xc = (float) points[i+0].X;
						yc = (float) points[i+0].Y;
						x3 = (float) points[i+1].X;
						y3 = (float) points[i+1].Y;
						
						//	Formules de conversion trouv�es ici: http://ungwe.org/blog/2004/02/22/15:50/
						
						x1 = (ox + 2 * xc) / 3;
						y1 = (oy + 2 * yc) / 3;
						x2 = (x3 + 2 * xc) / 3;
						y2 = (y3 + 2 * yc) / 3;
						
						gp.AddBezier (ox, oy, x1, y1, x2, y2, x3, y3);
						
						ox = x3;
						oy = y3;
						i += 1;
						break;
					
					case PathElement.Curve4:
						x1 = (float) points[i+0].X;
						y1 = (float) points[i+0].Y;
						x2 = (float) points[i+1].X;
						y2 = (float) points[i+1].Y;
						x3 = (float) points[i+2].X;
						y3 = (float) points[i+2].Y;
						gp.AddBezier (ox, oy, x1, y1, x2, y2, x3, y3);
						ox = x3;
						oy = y3;
						i += 2;
						break;
					
					case PathElement.Stop:
						break;
					
					default:
						if ((elements[i] & PathElement.MaskFlags) != PathElement.FlagClose)
						{
							throw new System.InvalidOperationException (string.Format ("Path cannot be converted, element {0} set to {1}.", i, elements[i]));
						}
						break;
				}
				
				if ((elements[i] & PathElement.FlagClose) != 0)
				{
					gp.CloseFigure ();
				}
			}
			
			return gp;
		}
		
		
		public static Path Combine(Path a, Path b, PathOperation operation)
		{
			Path result = new Path ();
			
			a.CreateOnTheFly ();
			b.CreateOnTheFly ();
			
			result.CreateOnTheFly ();
			
			AntiGrain.Path.CombinePathsUsingGpc (a.agg_path, b.agg_path, result.agg_path, (int) operation);
			
			result.has_curve = false;
			result.is_empty  = false;
			
			return result;
		}
		
		
		public static Path FromLine(double x1, double y1, double x2, double y2)
		{
			Path path = new Path ();
			path.MoveTo (x1, y1);
			path.LineTo (x2, y2);
			return path;
		}
		
		public static Path FromLine(Point p1, Point p2)
		{
			Path path = new Path ();
			path.MoveTo (p1.X, p1.Y);
			path.LineTo (p2.X, p2.Y);
			return path;
		}
		
		public static Path FromRectangle(double x, double y, double width, double height)
		{
			Path path = new Path ();
			path.AppendRectangle (x, y, width, height);
			return path;
		}
		
		public static Path FromRectangle(Rectangle rect)
		{
			return Path.FromRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public static Path FromRectangle(Point p, Size s)
		{
			return Path.FromRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public static Path FromCircle(double x, double y, double r)
		{
			Path path = new Path ();
			path.AppendCircle (x, y, r);
			return path;
		}
		
		public static Path FromCircle(double x, double y, double rx, double ry)
		{
			Path path = new Path ();
			path.AppendCircle (x, y, rx, ry);
			return path;
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
		
		
		protected Point ArcBezierRad(double cx, double cy, double rx, double ry, double a1, double a2, bool ccw, bool continue_path)
		{
			//	G�n�re un arc de cercle � l'aide d'un maximun de 4 courbes de B�zier.
			//	Retourne le point d'arriv�e.
			
			Point c = new Point (cx, cy);
			Point r = new Point (rx, ry);
			
			//	Par d�faut, le point d'arriv�e est �gal au point courant, si rien n'est
			//	dessin� :
			
			Point p2 = this.current_point;
			
			a1 = Math.ClipAngleRad (a1);
			a2 = Math.ClipAngleRad (a2);
			
			if (System.Math.Abs (a1-a2) < 0.0000001)
			{
				a2 = a1;
			}
			
			if (ccw)
			{
				if (a2 <= a1)
				{
					a2 += System.Math.PI * 2.0;
				}
				
				while (a1 < a2)
				{
					double aa = System.Math.Min (a1 + System.Math.PI/2.0, a2);
					double k  = Path.GetArcBezierKappaRad (aa - a1);
					
					Point p1, s1, s2;
					
					Path.ArcBezierPSRad (a1, k,  ccw, out p1, out s1);
					Path.ArcBezierPSRad (aa, k, !ccw, out p2, out s2);
					
					p1 = c + Point.ScaleMul (p1, r);
					s1 = c + Point.ScaleMul (s1, r);
					s2 = c + Point.ScaleMul (s2, r);
					p2 = c + Point.ScaleMul (p2, r);

					if (!continue_path)
					{
						continue_path = true;
						this.MoveTo (p1);
					}
					
					this.CurveTo (s1, s2, p2);
					
					a1 = aa;
				}
			}
			else
			{
				if (a2 >= a1)
				{
					a2 -= System.Math.PI * 2.0;
				}
				
				while (a1 > a2)
				{
					double aa = System.Math.Max (a1 - System.Math.PI/2.0, a2);
					double k  = Path.GetArcBezierKappaRad (a1 - aa);
					
					Point p1, s1, s2;
					
					Path.ArcBezierPSRad (a1, k,  ccw, out p1, out s1);
					Path.ArcBezierPSRad (aa, k, !ccw, out p2, out s2);
					
					p1 = c + Point.ScaleMul (p1, r);
					s1 = c + Point.ScaleMul (s1, r);
					s2 = c + Point.ScaleMul (s2, r);
					p2 = c + Point.ScaleMul (p2, r);
					
					if (!continue_path)
					{
						continue_path = true;
						this.MoveTo (p1);
					}
					this.CurveTo (s1, s2, p2);
					
					a1 = aa;
				}
			}
			
			return p2;
		}

		protected static void ArcBezierPSRad(double a, double k, bool ccw, out Point p, out Point s)
		{
			//	Calcule le point principal et le point secondaire d'un arc de B�zier
			//	de rayon 1 et de centre (0;0).

			p = new Point (System.Math.Cos (a), System.Math.Sin (a));
			s = (ccw) ? new Point (p.X - System.Math.Sin(a)*k, p.Y + System.Math.Cos(a)*k)
				/**/  : new Point (p.X + System.Math.Sin(a)*k, p.Y - System.Math.Cos(a)*k);
		}

		protected static double GetArcBezierKappaRad(double a)
		{
			//	D�termine le facteur kappa en fonction de l'angle (0..PI/2).
			
			double sin = System.Math.Sin (a/2.0);
			double cos = System.Math.Cos (a/2.0);
			
			double dx = (4.0-4.0*cos)/3.0;
			double dy = sin+(1.0-cos)*(cos-3.0)/(3.0*sin);
			
			return System.Math.Sqrt (dx*dx + dy*dy);
		}

		
		//	Le param�tre kappa permet de calculer la position des points secondaires d'une courbe de B�zier
		//	pour simuler un quart de cercle.
		//
		//	kappa = ((sqr(2)-1)/3)*4
		//
		//	Cf l'article http://www.whizkidtech.redprince.net/bezier/circle/
		
		protected const double					Kappa = 0.552284749828;
		
		
		internal void InternalCreateNonEmpty()
		{
			this.CreateOnTheFly ();
			this.is_empty = false;
		}
		
		
		protected System.IntPtr			agg_path;
		protected double				default_zoom = 1.0;
		
		private bool					has_curve = false;
		private bool					is_empty = true;
		private Point					current_point = Point.Empty;
		private bool					has_current_point = false;
	}
	
	public class DashedPath : Path
	{
		public DashedPath()
		{
		}
		
		
		public double					DashOffset
		{
			get
			{
				return this.start;
			}
			set
			{
				this.CreateOnTheFly ();
				this.start = value;
				AntiGrain.Path.SetDashOffset (this.agg_path, this.start);
			}
		}
		
		
		public void ResetDash()
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.ResetDash (this.agg_path);
		}
		
		public void AddDash(double dash_length, double gap_length)
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.AddDash (this.agg_path, dash_length, gap_length);
		}
		
		
		
		public Path GenerateDashedPath()
		{
			return this.GenerateDashedPath (this.default_zoom);
		}
		
		public Path GenerateDashedPath(double approximation_zoom)
		{
			if (this.IsEmpty)
			{
				return null;
			}
			
			Path path = new Path ();
			
			this.CreateOnTheFly ();
			path.InternalCreateNonEmpty ();
			
			AntiGrain.Path.AppendDashedPath (path.Handle, this.agg_path, approximation_zoom);
			
			return path;
		}
		
		private double					start;
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
		EndPoly		= 6,
		
		MaskCommand	= 0x0f,
		MaskFlags	= 0xf0,
		
		FlagCCW		= 0x10,
		FlagCW		= 0x20,
		FlagClose	= 0x40,
	}
	
	public enum PathOperation
	{
		Or = 0, And = 1, Xor = 2, AMinusB = 3, BMinusA = 4
	}
}
