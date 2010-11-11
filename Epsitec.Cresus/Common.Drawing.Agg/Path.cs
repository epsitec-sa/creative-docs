//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using System;
namespace Epsitec.Common.Drawing
{
	public class Path : System.IDisposable
	{
		public Path()
		{
		}

		public Path(Rectangle rect)
		{
			this.AppendRectangle (rect);
		}

		~Path()
		{
			this.Dispose (false);
		}
		
		
		public System.IntPtr					Handle
		{
			get { return this.aggPath; }
		}
		
		public bool								ContainsCurves
		{
			get { return this.hasCurve; }
		}
		
		public bool								IsEmpty
		{
			get { return this.isEmpty; }
		}
		
		public bool								IsValid
		{
			get { return ! this.isEmpty; }
		}
		
		public bool								IsCurrentPointValid
		{
			get { return this.hasCurrentPoint; }
		}
		
		public Point							CurrentPoint
		{
			get
			{
				if (this.hasCurrentPoint)
				{
					return this.currentPoint;
				}
				
				throw new System.InvalidOperationException ("No current point defined.");
			}
		}
		
		public double							DefaultZoom
		{
			get
			{
				return this.defaultZoom;
			}
			set
			{
				this.defaultZoom = value;
			}
		}
		
		
		public void Clear()
		{
			this.CreateOnTheFly ();
			this.hasCurve = false;
			this.isEmpty  = true;
			this.hasCurrentPoint = false;
			AntiGrain.Path.RemoveAll (this.aggPath);
		}
		
		public void MoveTo(Point p)
		{
			this.MoveTo (p.X, p.Y);
		}
		
		public void MoveTo(double x, double y)
		{
			this.CreateOnTheFly ();
			this.isEmpty = false;
			this.hasCurrentPoint = true;
			this.currentPoint = new Point (x, y);
			AntiGrain.Path.MoveTo (this.aggPath, x, y);
		}
		
		public void LineTo(Point p)
		{
			this.LineTo (p.X, p.Y);
		}
		
		public void LineTo(double x, double y)
		{
			this.CreateOnTheFly ();
			this.isEmpty = false;
			this.hasCurrentPoint = true;
			this.currentPoint = new Point (x, y);
			AntiGrain.Path.LineTo (this.aggPath, x, y);
		}
		
		public void CurveTo(Point c1, Point c2, Point p)
		{
			this.CurveTo (c1.X, c1.Y, c2.X, c2.Y, p.X, p.Y);
		}
		
		public void CurveTo(double xC1, double yC1, double xC2, double yC2, double x, double y)
		{
			this.CreateOnTheFly ();
			this.isEmpty  = false;
			this.hasCurve = true;
			this.hasCurrentPoint = true;
			this.currentPoint = new Point (x, y);
			AntiGrain.Path.Curve4 (this.aggPath, xC1, yC1, xC2, yC2, x, y);
		}
		
		public void CurveTo(Point c, Point p)
		{
			this.CurveTo (c.X, c.Y, p.X, p.Y);
		}
		
		public void CurveTo(double xC, double yC, double x, double y)
		{
			this.CreateOnTheFly ();
			this.isEmpty  = false;
			this.hasCurve = true;
			this.hasCurrentPoint = true;
			this.currentPoint = new Point (x, y);
			AntiGrain.Path.Curve3 (this.aggPath, xC, yC, x, y);
		}

		public void ArcTo(double xC, double yC, double x, double y)
		{
			this.ArcTo (new Point(xC, yC), new Point(x, y));
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
			this.ArcToDeg (x, y, rx, ry, a1, a2, ccw, this.defaultZoom);
		}

		public void ArcToDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximationZoom)
		{
			a1 = Math.DegToRad (a1);
			a2 = Math.DegToRad (a2);

			this.ArcToRad (x, y, rx, ry, a1, a2, ccw, approximationZoom);
		}
		
		public void ArcToRad(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcToRad (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcToRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcToRad (x, y, rx, ry, a1, a2, ccw, this.defaultZoom);
		}
		
		public void ArcToRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximationZoom)
		{
			this.LineTo (new Point (x + System.Math.Cos (a1) * rx, y + System.Math.Sin (a1) * ry));
			this.currentPoint = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, true);
			this.hasCurrentPoint = true;
			this.isEmpty = false;
		}
		
		public void ArcDeg(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcDeg (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcDeg (x, y, rx, ry, a1, a2, ccw, this.defaultZoom);
		}
		
		public void ArcDeg(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximationZoom)
		{
			a1 = Math.DegToRad(a1);
			a2 = Math.DegToRad(a2);

			this.currentPoint = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, false);
			this.hasCurrentPoint = true;
			this.isEmpty = false;
		}
		
		public void ArcRad(Point c, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcRad (c.X, c.Y, rx, ry, a1, a2, ccw);
		}
		
		public void ArcRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw)
		{
			this.ArcRad (x, y, rx, ry, a1, a2, ccw, this.defaultZoom);
		}
		
		public void ArcRad(double x, double y, double rx, double ry, double a1, double a2, bool ccw, double approximationZoom)
		{
			this.currentPoint = this.ArcBezierRad (x, y, rx, ry, a1, a2, ccw, false);
			this.hasCurrentPoint = true;
			this.isEmpty = false;
		}

		
		public void Close()
		{
			if (! this.isEmpty)
			{
				this.CreateOnTheFly ();
				this.hasCurrentPoint = false;
				AntiGrain.Path.Close (this.aggPath);
			}
		}
		
		public void StartNewPath()
		{
			if (! this.isEmpty)
			{
				this.CreateOnTheFly ();
				this.hasCurrentPoint = false;
				AntiGrain.Path.AddNewPath (this.aggPath);
			}
		}
		
		
		public void Append(Path path)
		{
			this.Append (path, 1, 0, 0, 1, 0, 0, this.defaultZoom);
		}

		//	approximationZoom :
		//	Pour un dessin à l'échelle 1:1, utiliser 1.0 comme valeur pour approximationZoom.
		//	Si un zoom x10 est appliqué au chemin avant de le transformer en pixels, il faut utiliser un approximationZoom de 10.
		//	1.0 = dessin avec l'unité égale au pixel
		//	0.1 = dessin avec l'unité égale à 0.1 pixel
		//	0.0 = division par zéro dans le code interne !

		public void Append(Path path, double approximationZoom)
		{
			this.Append (path, 1, 0, 0, 1, 0, 0, approximationZoom);
		}
		
		public void Append(Path path, double width, CapStyle cap, JoinStyle join, double miterLimit, double approximationZoom)
		{
			this.Append (path, width, cap, join, miterLimit, approximationZoom, path.hasCurve);
		}
		
		public void Append(Path path, double width, CapStyle cap, JoinStyle join, double miterLimit, double approximationZoom, bool flattenCurves)
		{
			this.CreateOnTheFly ();
			this.isEmpty &= path.isEmpty;
			AntiGrain.Path.AppendPath (this.aggPath, path.aggPath, width, (int) cap, (int) join, miterLimit, approximationZoom, flattenCurves);
		}
		
		public void Append(Path path, Transform transform, double approximationZoom)
		{
			this.Append (path, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY, approximationZoom);
		}
		
		public void Append(Path path, Transform transform, double approximationZoom, double boldWidth)
		{
			this.Append (path, transform.XX, transform.XY, transform.YX, transform.YY, transform.TX, transform.TY, approximationZoom, boldWidth);
		}
		
		public void Append(Path path, double xx, double xy, double yx, double yy, double tx, double ty, double approximationZoom)
		{
			this.CreateOnTheFly ();
			this.hasCurve |= path.hasCurve;
			this.isEmpty  &= path.isEmpty;
			AntiGrain.Path.AppendPath (this.aggPath, path.aggPath, xx, xy, yx, yy, tx, ty, approximationZoom, 0);
		}
		
		public void Append(Path path, double xx, double xy, double yx, double yy, double tx, double ty, double approximationZoom, double boldWidth)
		{
			this.CreateOnTheFly ();
			this.hasCurve |= path.hasCurve;
			this.isEmpty  &= path.isEmpty;
			AntiGrain.Path.AppendPath (this.aggPath, path.aggPath, xx, xy, yx, yy, tx, ty, approximationZoom, boldWidth);
		}
		
		public void Append(Path path, double approximationZoom, double boldWidth)
		{
			this.CreateOnTheFly ();
			this.hasCurve |= path.hasCurve;
			this.isEmpty  &= path.isEmpty;
			AntiGrain.Path.AppendPath (this.aggPath, path.aggPath, 1, 0, 0, 1, 0, 0, approximationZoom, boldWidth);
		}

		public void Append(Font font, string text, double x, double y, double size)
		{
			if ((font != null) &&
				(!string.IsNullOrEmpty (text)))
			{
				foreach (char c in text)
				{
					ushort glyph   = font.GetGlyphIndex (c);
					double advance = font.GetGlyphAdvance (glyph) * size;

					this.Append (font, glyph, x, y, size);

					x += advance;
				}
			}
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
			if (font == null)
			{
				return;
			}

			this.CreateOnTheFly ();
			this.hasCurve = true;
			this.isEmpty  = false;
			
			if (font.IsSynthetic)
			{
				Transform ft = font.SyntheticTransform;
				
				ft = ft.MultiplyBy (new Transform (xx, xy, yx, yy, tx, ty));
				
				xx = ft.XX;
				xy = ft.XY;
				yx = ft.YX;
				yy = ft.YY;
				tx = ft.TX;
				ty = ft.TY;
			}
			
			if (glyph < 0xfff0)
			{
				AntiGrain.Path.AppendGlyph (this.aggPath, font.Handle, glyph, xx, xy, yx, yy, tx, ty, 0);
			}
		}
		
		public void Append(Font font, int glyph, double xx, double xy, double yx, double yy, double tx, double ty, double boldWidth)
		{
			this.CreateOnTheFly ();
			this.hasCurve = true;
			this.isEmpty  = false;
			
			if (font.IsSynthetic)
			{
				Transform ft = font.SyntheticTransform;
				
				ft = ft.MultiplyBy (new Transform (xx, xy, yx, yy, tx, ty));
				
				xx = ft.XX;
				xy = ft.XY;
				yx = ft.YX;
				yy = ft.YY;
				tx = ft.TX;
				ty = ft.TY;
			}
			
			if (glyph < 0xfff0)
			{
				AntiGrain.Path.AppendGlyph (this.aggPath, font.Handle, glyph, xx, xy, yx, yy, tx, ty, boldWidth);
			}
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

		public void AppendRoundedRectangle(double x, double y, double width, double height, double r)
		{
			if (width < r)
			{
				r = width;
			}
			if (height < r)
			{
				r = height;
			}

			this.MoveTo (x, y+height-r);
			this.LineTo (x, y+r);
			this.ArcTo (x, y, x+r, y);
			this.LineTo (x+width-r, y);
			this.ArcTo (x+width, y, x+width, y+r);
			this.LineTo (x+width, y+height-r);
			this.ArcTo (x+width, y+height, x+width-r, y+height);
			this.LineTo (x+r, y+height);
			this.ArcTo (x, y+height, x, y+height-r);
			this.Close ();
		}
		
		public void AppendRoundedRectangle(Rectangle rect, double r)
		{
			this.AppendRoundedRectangle (rect.X, rect.Y, rect.Width, rect.Height, r);
		}

		public void AppendRoundedRectangle(Point p, Size s, double r)
		{
			this.AppendRoundedRectangle (p.X, p.Y, s.Width, s.Height, r);
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
			this.Append (Path.CreateCircle (cx, cy, rx, ry), 0);
		}
		
		
		public static Path CreateCircle(Point c, double rx, double ry)
		{
			return Path.CreateCircle (c.X, c.Y, rx, ry);
		}
		
		public static Path CreateCircle(double cx, double cy, double rx, double ry)
		{
			Drawing.Path path = new Drawing.Path();
			
			path.MoveTo (cx-rx, cy);
			path.CurveTo (cx-rx, cy+ry*Path.Kappa, cx-rx*Path.Kappa, cy+ry, cx, cy+ry);
			path.CurveTo (cx+rx*Path.Kappa, cy+ry, cx+rx, cy+ry*Path.Kappa, cx+rx, cy);
			path.CurveTo (cx+rx, cy-ry*Path.Kappa, cx+rx*Path.Kappa, cy-ry, cx, cy-ry);
			path.CurveTo (cx-rx*Path.Kappa, cy-ry, cx-rx, cy-ry*Path.Kappa, cx-rx, cy);
			path.Close ();
			
			return path;
		}

		public static Path CreateRoundedRectangle(Rectangle rect, double rx, double ry)
		{
			Drawing.Path path = new Drawing.Path();

			double kx = rx * Path.Kappa;
			double ky = ry * Path.Kappa;
			
			path.MoveTo (rect.Right-rx, rect.Bottom);
			path.CurveTo (rect.Right-rx+kx, rect.Bottom, rect.Right, rect.Bottom+ry-ky, rect.Right, rect.Bottom+ry);
			path.LineTo (rect.Right, rect.Top-ry);
			path.CurveTo (rect.Right, rect.Top-ry+ky, rect.Right-rx+kx, rect.Top, rect.Right-rx, rect.Top);
			path.LineTo (rect.Left+rx, rect.Top);
			path.CurveTo (rect.Left+rx-kx, rect.Top, rect.Left, rect.Top-ry+ky, rect.Left, rect.Top-ry);
			path.LineTo (rect.Left, rect.Bottom+ry);
			path.CurveTo (rect.Left, rect.Bottom+ry-ky, rect.Left+ry-ky, rect.Bottom, rect.Left+ry, rect.Bottom);
			path.Close ();
			
			return path;
		}
		
		
		public void ComputeBounds(out double x1, out double y1, out double x2, out double y2)
		{
			if (this.isEmpty)
			{
				x1 = 0;
				y1 = 0;
				x2 = -1;
				y2 = -1;
			}
			else
			{
				this.CreateOnTheFly ();
				AntiGrain.Path.ComputeBounds (this.aggPath, out x1, out y1, out x2, out y2);
			}
		}
		
		public Rectangle ComputeBounds()
		{
			if (this.isEmpty)
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

        public bool SurfaceContainsPoint(double x, double y, double approximationZoom)
		{

            if (this.ContainsCurves)
            {
                using (Path path = new Path())
                {
                    path.Append(this, approximationZoom);
                    
                    /* The path detection doesn't work with curves. 
                     * They are firstly converted into other shapes that can be handled.
                     * Since we use the Append() method - which sets ContainsCurves to true,
                     * since the path *has* curves - we call another method which doesn't 
                     * check the curves to prevent a infinite recursive loop. 
                     */
                    return path.SurfaceContainsPointWithoutCurves(x, y, approximationZoom);
                }
            }
            else
            {
                return this.SurfaceContainsPointWithoutCurves(x, y, approximationZoom);
            }
			
		}

		#region Helper Class : InsideSurfaceAnalyzer

		private sealed class InsideSurfaceAnalyzer
		{
			public InsideSurfaceAnalyzer(Point p)
			{
				this.p = p;
				this.list = new List<double> ();
			}

			public void AddSegment(Point a, Point b)
			{
				Point i;
				
				if (Point.IntersectsWithHorizontal (a, b, this.p.Y, out i))
				{
					if (a.Y == b.Y)
					{
						//	Skip horizontal lines; we cannot find intersections with
						//	those points.
						
						return;
					}

					this.list.Add (i.X);
				}
			}

			public bool IsInside()
			{
				int count = 0;
				
				foreach (double x in this.list)
				{
					if (this.p.X < x)
					{
						count++;
					}
				}

				//	An odd number of right-hand intersections means that we are inside of the
				//	path (using the even/odd rule) :
				
				return (count%2 != 0);
			}


			private readonly Point			p;
			private readonly List<double>	list;
		}

		#endregion


		public bool								HasZeroElements
		{
			get
			{
				if (this.isEmpty)
				{
					return true;
				}
				else
				{
					this.CreateOnTheFly ();
				
					int n = AntiGrain.Path.ElemCount (this.aggPath);
					if (n == 0)  return true;
					if (n != 1)  return false;
				
					int[]    e = new int[n];
					double[] x = new double[n];
					double[] y = new double[n];
				
					AntiGrain.Path.ElemGet (this.aggPath, n, e, x, y);
					PathElement element = (PathElement) e[0];
					return (element == PathElement.Stop);
				}
			}
		}

		public void GetElements(out PathElement[] elements, out Point[] points)
		{
			if (this.isEmpty)
			{
				elements = new PathElement[0];
				points   = new Point[0];
			}
			else
			{
				this.CreateOnTheFly ();
				
				int n = AntiGrain.Path.ElemCount (this.aggPath);
				
				int[]    e = new int[n];
				double[] x = new double[n];
				double[] y = new double[n];

				if (n > 1000)
				{
					System.Diagnostics.Debug.WriteLine ("Huge path, n = " + n.ToString ());
				}
				
				AntiGrain.Path.ElemGet (this.aggPath, n, e, x, y);
				
				elements = new PathElement[n];
				points   = new Point[n];
				
				for (int i = 0; i < n; i++)
				{
					elements[i] = (PathElement) e[i];
					points[i]   = new Point (x[i], y[i]);
				}
			}
		}
		
		public void SetElements(PathElement[] elements, Point[] points)
		{
			System.Diagnostics.Debug.Assert (elements.Length == points.Length);
			
			int n = elements.Length;
			
			if (n == 0)
			{
				this.isEmpty = true;
			}
			else
			{
				this.Clear ();
				
				for (int i = 0; i < n; i++)
				{
					switch (elements[i] & PathElement.MaskCommand)
					{
						case PathElement.MoveTo:
							this.MoveTo (points[i]);
							break;
						case PathElement.LineTo:
							this.LineTo (points[i]);
							break;
						case PathElement.Curve3:
							this.CurveTo (points[i], points[i+1]);
							i += 1;
							break;
						case PathElement.Curve4:
							this.CurveTo (points[i], points[i+1], points[i+2]);
							i += 2;
							break;
					}
					
					if ((elements[i] & PathElement.FlagClose) != 0)
					{
						this.Close ();
					}
				}
			}
		}
		
		
		public byte[] GetBlobOfElements()
		{
			PathElement[] elements;
			Point[]       points;
			double[]      values;
			
			this.GetElements (out elements, out points);
			
			int n = elements.Length;
			
			if (n > 0xfff0)
			{
				//	Just in case... We wouldn't want to overflow the
				//	size stored in the header (unsigned 16-bit value).
				
				n = 0;
			}
			
			values = new double[2*n];
			
			System.Diagnostics.Debug.Assert (System.Buffer.ByteLength (values) == 16*n);
			
			byte[] blob = new byte[2+n+16*n];
			
			blob[0] = (byte)(n >> 8);
			blob[1] = (byte)(n & 0xff);
			
			if (n > 0)
			{
				for (int i = 0; i < n; i++)
				{
					blob[2+i] = (byte) elements[i];
					values[2*i+0] = points[i].X;
					values[2*i+1] = points[i].Y;
				}
				
				System.Buffer.BlockCopy (values, 0, blob, 2+n, 16*n);
			}
			
			return blob;
		}
		
		public void SetBlobOfElements(byte[] blob)
		{
			int n   = (blob[0] << 8) | (blob[1]);
			int len = 2+n+16*n;
			
			if (blob.Length != len)
			{
				//	Aboid crashing if we get a blob which was messed up by the
				//	caller (size of blob must match size based on element count).
				
				return;
			}
			
			PathElement[] elements = new PathElement[n];
			double[]      values   = new double[2*n];
			Point[]       points   = new Point[n];
			
			if (n > 0)
			{
				System.Buffer.BlockCopy (blob, 2+n, values, 0, 16*n);
			}
			
			for (int i = 0; i < n; i++)
			{
				elements[i] = (PathElement) blob[2+i];
				points[i].X = values[2*i+0];
				points[i].Y = values[2*i+1];
			}
			
			this.SetElements (elements, points);
		}
		

		public override string ToString()
		{
			if (this.isEmpty)
			{
				return "empty\r\n";
			}
			
			PathElement[] elements;
			Point[] points;
			
			this.GetElements (out elements, out points);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			bool addSpace = false;
			
			for (int i = 0; i < elements.Length; i++)
			{
				if (addSpace)
				{
					buffer.Append (";\r\n");
				}
				
				addSpace = true;
				
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
						addSpace = false;
						break;
				}
				
				if ((elements[i] & PathElement.FlagClose) != 0)
				{
					if (addSpace)
					{
						buffer.Append (";\r\n");
					}
					
					buffer.Append ("close");
					addSpace = true;
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
						
						//	Formules de conversion trouvées ici: http://ungwe.org/blog/2004/02/22/15:50/
						
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
			
			AntiGrain.Path.CombinePathsUsingGpc (a.aggPath, b.aggPath, result.aggPath, (int) operation);
			
			result.hasCurve = false;
			result.isEmpty  = false;
			
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

		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	No managed stuff here...
			}
			
			if (this.aggPath != System.IntPtr.Zero)
			{
				AntiGrain.Path.Delete (this.aggPath);
				this.aggPath = System.IntPtr.Zero;
			}
		}
		
		protected virtual void CreateOnTheFly()
		{
			if (this.aggPath == System.IntPtr.Zero)
			{
				this.aggPath = AntiGrain.Path.New ();
			}
		}
		
		
		protected Point ArcBezierRad(double cx, double cy, double rx, double ry, double a1, double a2, bool ccw, bool continuePath)
		{
			//	Génère un arc de cercle à l'aide d'un maximun de 4 courbes de Bézier.
			//	Retourne le point d'arrivée.
			
			Point c = new Point (cx, cy);
			Point r = new Point (rx, ry);
			
			//	Par défaut, le point d'arrivée est égal au point courant, si rien n'est
			//	dessiné :
			
			Point p2 = this.currentPoint;
			
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

					if (!continuePath)
					{
						continuePath = true;
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
					
					if (!continuePath)
					{
						continuePath = true;
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
			//	Calcule le point principal et le point secondaire d'un arc de Bézier
			//	de rayon 1 et de centre (0;0).

			p = new Point (System.Math.Cos (a), System.Math.Sin (a));
			s = (ccw) ? new Point (p.X - System.Math.Sin(a)*k, p.Y + System.Math.Cos(a)*k)
				/**/  : new Point (p.X + System.Math.Sin(a)*k, p.Y - System.Math.Cos(a)*k);
		}

		protected static double GetArcBezierKappaRad(double a)
		{
			//	Détermine le facteur kappa en fonction de l'angle (0..PI/2).
			
			double sin = System.Math.Sin (a/2.0);
			double cos = System.Math.Cos (a/2.0);
			
			double dx = (4.0-4.0*cos)/3.0;
			double dy = sin+(1.0-cos)*(cos-3.0)/(3.0*sin);
			
			return System.Math.Sqrt (dx*dx + dy*dy);
		}

		
		internal void InternalCreateNonEmpty()
		{
			this.CreateOnTheFly ();
			this.isEmpty = false;
		}


        /// <summary>
        /// Check if a certain point is inside the current path.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="approximationZoom">Zoom in the figure</param>
        /// <returns>Whether the point is inside the path</returns>
        private bool SurfaceContainsPointWithoutCurves(double x, double y, double approximationZoom)
        {
            var analyzer = new InsideSurfaceAnalyzer(new Point(x, y));

            Point start = Point.Zero;
            Point current = Point.Zero;
            Point p1 = Point.Zero;
            bool closed = false;

            PathElement[] elements;
            Point[] points;

            this.GetElements(out elements, out points);

            for (int i = 0; i < elements.Length; i++)
            {
                switch (elements[i] & PathElement.MaskCommand)
                {
                    case PathElement.MoveTo:
                        start = current = points[i];
                        closed = false;
                        break;

                    case PathElement.LineTo:
                        p1 = points[i];
                        analyzer.AddSegment(current, p1);
                        current = p1;
                        break;

                    case PathElement.Curve3:
                    case PathElement.Curve4:
                        throw new System.InvalidOperationException("Flattened path still contains curves");

                    default:
                        break;
                }

                if ((elements[i] & PathElement.FlagClose) != 0)
                {
                    analyzer.AddSegment(current, start);
                    closed = true;
                }
            }

            if (!closed)
            {
                analyzer.AddSegment(current, start);
            }

            return analyzer.IsInside();
        }
		
		
		//	Le paramètre kappa permet de calculer la position des points secondaires d'une courbe de Bézier
		//	pour simuler un quart de cercle.
		//
		//	kappa = ((sqr(2)-1)/3)*4
		//
		//	Cf l'article http://www.whizkidtech.redprince.net/bezier/circle/
		
		public const double				Kappa = 0.552284749828;
		
		protected System.IntPtr			aggPath;
		protected double				defaultZoom = 1.0;
		
		private bool					hasCurve = false;
		private bool					isEmpty = true;
		private Point					currentPoint = Point.Zero;
		private bool					hasCurrentPoint = false;
	}
	
	[System.Flags]
	public enum PathElement : byte
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
		Or		= 0,
		And		= 1,
		Xor		= 2,
		AMinusB = 3,
		BMinusA = 4
	}
}
