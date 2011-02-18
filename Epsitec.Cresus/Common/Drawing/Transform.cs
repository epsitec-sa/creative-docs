//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Transform représente une transformation 2D (matrice 2x2 et vecteur de
	/// translation). Elle supporte les opérations telles que la translation simple, la
	/// rotation, le changement d'échelle, ainsi que leur combinaison.
	/// </summary>
	public struct Transform : System.IComparable
	{
		public Transform(double xx, double xy, double yx, double yy, double tx, double ty)
		{
			this.xx = xx;
			this.xy = xy;
			this.yx = yx;
			this.yy = yy;
			this.tx = tx;
			this.ty = ty;
		}
		
		
		public bool								OnlyTranslate
		{
			get
			{
				if ((Transform.IsOne (this.xx)) &&
					(Transform.IsOne (this.yy)) &&
					(Transform.IsZero (this.xy)) &&
					(Transform.IsZero (this.yx)))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool								OnlyScaleOrTranslate
		{
			get
			{
				if ((Transform.IsZero (this.xy)) &&
					(Transform.IsZero (this.yx)))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		
		public double							XX
		{
			get
			{
				return this.xx;
			}
		}
		
		public double							XY
		{
			get
			{
				return this.xy;
			}
		}
		
		public double							YX
		{
			get
			{
				return this.yx;
			}
		}
		
		public double							YY
		{
			get
			{
				return this.yy;
			}
		}
		
		public double							TX
		{
			get
			{
				return this.tx;
			}
		}
		
		public double							TY
		{
			get
			{
				return this.ty;
			}
		}


		public static readonly Transform		Identity = new Transform (1, 0, 0, 1, 0, 0);
		
		
		public double GetZoom()
		{
			//	Détermine le zoom approximatif en vigueur dans la transformation actuelle.
			//	Calcule la longueur d'un segment diagonal [1 1] après transformation pour
			//	connaître ce zoom.
			
			double a = this.xx + this.xy;
			double b = this.yx + this.yy;
			
			return System.Math.Sqrt ((a*a + b*b) / 2);
		}
		
		
		public Point TransformDirect(Point pt)
		{
			double x = pt.X;
			double y = pt.Y;
			
			pt.X = this.xx * x + this.xy * y + this.tx;
			pt.Y = this.yx * x + this.yy * y + this.ty;
			
			return pt;
		}

		public Rectangle ScaleOrTranslateDirect(Rectangle rect)
		{
			System.Diagnostics.Debug.Assert (this.OnlyScaleOrTranslate);

			if ((rect.IsEmpty) ||
				(rect.IsInfinite))
			{
				return rect;
			}
			else
			{
				rect.Left   = this.xx * rect.Left   + this.tx;
				rect.Right  = this.xx * rect.Right  + this.tx;
				rect.Bottom = this.yy * rect.Bottom + this.ty;
				rect.Top    = this.yy * rect.Top    + this.ty;

				return rect;
			}
		}

		public void TransformDirect(ref double px, ref double py)
		{
			double x = px;
			double y = py;
			
			px = this.xx * x + this.xy * y + this.tx;
			py = this.yx * x + this.yy * y + this.ty;
		}
		
		public Point TransformInverse(Point pt)
		{
			double det = this.xx * this.yy - this.xy * this.yx;
			
			System.Diagnostics.Debug.Assert (det != 0.0f);
			
			double x = pt.X - this.tx;
			double y = pt.Y - this.ty;
			
			pt.X = (  this.yy * x - this.xy * y) / det;
			pt.Y = (- this.yx * x + this.xx * y) / det;
			
			return pt;
		}
		
		public void  TransformInverse(ref double px, ref double py)
		{
			double det = this.xx * this.yy - this.xy * this.yx;
			
			System.Diagnostics.Debug.Assert (det != 0.0f);
			
			double x = px - this.tx;
			double y = py - this.ty;
			
			px = (  this.yy * x - this.xy * y) / det;
			py = (- this.yx * x + this.xx * y) / det;
		}
		
		
		public Transform MultiplyBy(Transform t)
		{
			return Transform.Multiply (t, this);
		}
		
		public Transform MultiplyByPostfix(Transform t)
		{
			return Transform.Multiply (this, t);
		}
		
		public Transform Translate(double tx, double ty)
		{
			return new Transform (this.xx, this.xy, this.yx, this.yy, this.tx + tx, this.ty + ty);
		}
		
		public Transform Translate(Point offset)
		{
			return new Transform (this.xx, this.xy, this.yx, this.yy, this.tx + offset.X, this.ty + offset.Y);
		}
		
		public Transform RotateDeg(double angle)
		{
			if (angle != 0)
			{
				return this.MultiplyBy (Transform.CreateRotationDegTransform (angle));
			}
			else
			{
				return this;
			}
		}
		
		public Transform RotateDeg(double angle, Point center)
		{
			if (angle != 0)
			{
				return this.MultiplyBy (Transform.CreateRotationDegTransform (angle, center.X, center.Y));
			}
			else
			{
				return this;
			}
		}
		
		public Transform RotateDeg(double angle, double x, double y)
		{
			if (angle != 0)
			{
				return this.MultiplyBy (Transform.CreateRotationDegTransform (angle, x, y));
			}
			else
			{
				return this;
			}
		}
		
		public Transform Scale(double s)
		{
			if (s != 1.0f)
			{
				return this.Scale (s, s);
			}
			else
			{
				return this;
			}
		}
		
		public Transform Scale(double sx, double sy)
		{
			return new Transform (this.xx * sx, this.xy * sy, this.yx * sx, this.yy * sy, this.tx * sx, this.ty * sy);
		}
		
		public Transform Scale(double sx, double sy, double cx, double cy)
		{
			return new Transform (this.xx * sx, this.xy * sy, this.yx * sx, this.yy * sy, (this.tx-cx) * sx + cx, (this.ty-cy) * sy + cy);
		}
		
		
		public static Transform CreateScaleTransform(double sx, double sy)
		{
			return new Transform (sx, 0, 0, sy, 0, 0);
		}
		
		public static Transform CreateScaleTransform(double sx, double sy, double cx, double cy)
		{
			return new Transform (sx, 0, 0, sy, cx-sx*cx, cy-sy*cy);
		}
		
		public static Transform CreateTranslationTransform(double tx, double ty)
		{
			return new Transform (1, 0, 0, 1, tx, ty);
		}
		
		public static Transform CreateTranslationTransform(Point offset)
		{
			return new Transform (1, 0, 0, 1, offset.X, offset.Y);
		}
		
		public static Transform CreateRotationDegTransform(double angle)
		{
			double alpha = Math.DegToRad (angle);
			
			double sin = System.Math.Sin (alpha);
			double cos = System.Math.Cos (alpha);
			
			return new Transform (cos, -sin, sin, cos, 0, 0);
		}
		
		public static Transform CreateRotationDegTransform(double angle, Point center)
		{
			return Transform.CreateRotationDegTransform (angle, center.X, center.Y);
		}
		
		public static Transform CreateRotationDegTransform(double angle, double cx, double cy)
		{
			Transform m = Transform.CreateRotationDegTransform (angle);
			return new Transform (m.xx, m.xy, m.yx, m.yy, cx - m.xx * cx - m.xy * cy, cy - m.yx * cx - m.yy * cy);
		}
		
		public static Transform CreateRotationRadTransform(double angle)
		{
			double sin = System.Math.Sin (angle);
			double cos = System.Math.Cos (angle);
			
			return new Transform (cos, -sin, sin, cos, 0, 0);
		}
		
		public static Transform CreateRotationRadTransform(double angle, Point center)
		{
			return Transform.CreateRotationRadTransform (angle, center.X, center.Y);
		}
		
		public static Transform CreateRotationRadTransform(double angle, double cx, double cy)
		{
			Transform m = Transform.CreateRotationRadTransform (angle);
			return new Transform (m.xx, m.xy, m.yx, m.yy, cx - m.xx * cx - m.xy * cy, cy - m.yx * cx - m.yy * cy);
		}
		
		
		public static Transform Inverse(Transform m)
		{
			double det = m.xx * m.yy - m.xy * m.yx;
			
			if (det == 0)
			{
				throw new System.DivideByZeroException ("Transform matrix cannot be inverted.");
			}
			
			System.Diagnostics.Debug.Assert (det != 0.0);
			
			double det1 = 1.0f / det;
			
			double xx =   m.yy * det1;
			double xy = - m.xy * det1;
			double yx = - m.yx * det1;
			double yy =   m.xx * det1;
			
			double tx = - xx * m.tx - xy * m.ty;
			double ty = - yx * m.tx - yy * m.ty;
			
			return new Transform (xx, xy, yx, yy, tx, ty);
		}
		
		public static Transform Multiply(Transform a, Transform b)
		{
			return new Transform (a.xx * b.xx + a.xy * b.yx,
								  a.xx * b.xy + a.xy * b.yy,
								  a.yx * b.xx + a.yy * b.yx,
								  a.yx * b.xy + a.yy * b.yy,
								  a.xx * b.tx + a.xy * b.ty + a.tx,
								  a.yx * b.tx + a.yy * b.ty + a.ty);
		}
		
		public static Point Multiply(Transform a, Point b)
		{
			return new Point (a.xx * b.X + a.xy * b.Y + a.tx,
							  a.yx * b.X + a.yy * b.Y + a.ty);
		}
		
		
		public static Point RotatePointRad(Point center, double angle, Point p)
		{
			//	Fait tourner un point autour d'un centre.
			//	L'angle est exprimé en radians.
			//	Un angle positif est anti-horaire (CCW).
			
			Point a = new Point();
			Point b = new Point();

			a.X = p.X-center.X;
			a.Y = p.Y-center.Y;
			
			double sin = System.Math.Sin (angle);
			double cos = System.Math.Cos (angle);

			b.X = a.X * cos - a.Y * sin;
			b.Y = a.X * sin + a.Y * cos;

			b.X += center.X;
			b.Y += center.Y;
			
			return b;
		}

		public static Point RotatePointRad(double angle, Point p)
		{
			//	Fait tourner un point autour de l'origine.
			//	L'angle est exprimé en radians.
			//	Un angle positif est anti-horaire (CCW).
			
			Drawing.Point a = new Drawing.Point();

			double sin = System.Math.Sin (angle);
			double cos = System.Math.Cos (angle);

			a.X = p.X * cos - p.Y * sin;
			a.Y = p.X * sin + p.Y * cos;

			return a;
		}

		public static Point RotatePointDeg(Point center, double angle, Point p)
		{
			return Transform.RotatePointRad (center, Math.DegToRad (angle), p);
		}

		public static Point RotatePointDeg(double angle, Point p)
		{
			return Transform.RotatePointRad (Math.DegToRad (angle), p);
		}


		#region Comparison Support
		public static bool Equal(double a, double b)
		{
			double δ = a - b;
			return (δ < Transform.ε) && (δ > -Transform.ε);
		}
		
		public static bool Equal(Point a, Point b)
		{
			return Transform.Equal (a.X, b.X) && Transform.Equal (a.Y, b.Y);
		}
		
		public static bool Equal(Size a, Size b)
		{
			return Transform.Equal (a.Width, b.Width) && Transform.Equal (a.Height, b.Height);
		}
		
		public static bool Equal(Rectangle a, Rectangle b)
		{
			return Transform.Equal (a.Location, b.Location) && Transform.Equal (a.Size, b.Size);
		}
		
		public static bool IsZero(double a)
		{
			return (a < Transform.ε) && (a > -Transform.ε);
		}
		
		public static bool IsOne(double a)
		{
			return (a < 1+Transform.ε) && (a > 1-Transform.ε);
		}
		
		public static void Round(ref double a)
		{
			if (Transform.IsZero (a))
			{
				a = 0;
			}
			else if (Transform.IsOne (a))
			{
				a = 1;
			}
		}
		#endregion
		
		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj is Transform)
			{
				Transform t = (Transform) obj;
				
				if (Transform.Equal (this.XX, t.XX) &&
					Transform.Equal (this.XY, t.XY) &&
					Transform.Equal (this.YX, t.YX) &&
					Transform.Equal (this.YY, t.YY) &&
					Transform.Equal (this.TX, t.TX) &&
					Transform.Equal (this.TY, t.TY))
				{
					return 0;
				}
				
				if ((this.XX < t.XX) ||
					((this.XX == t.XX) &&
					 ((this.XY < t.XY) ||
					  ((this.XY == t.XY) &&
					   ((this.YX < t.YX) ||
					    ((this.YX == t.YX) &&
					     ((this.YY < t.YY) ||
					      ((this.YY == t.YY) &&
					       ((this.TX < t.TX) ||
					        ((this.TX == t.TX) &&
					         (this.TY < t.TY)))))))))))
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}
			
			throw new System.ArgumentException ("object is not a Transform");
		}
		#endregion
		
		public static bool operator ==(Transform a, Transform b)
		{
			if (Transform.Equal (a.XX, b.XX) &&
				Transform.Equal (a.XY, b.XY) &&
				Transform.Equal (a.YX, b.YX) &&
				Transform.Equal (a.YY, b.YY) &&
				Transform.Equal (a.TX, b.TX) &&
				Transform.Equal (a.TY, b.TY))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static bool operator !=(Transform a, Transform b)
		{
			if (Transform.Equal (a.XX, b.XX) &&
				Transform.Equal (a.XY, b.XY) &&
				Transform.Equal (a.YX, b.YX) &&
				Transform.Equal (a.YY, b.YY) &&
				Transform.Equal (a.TX, b.TX) &&
				Transform.Equal (a.TY, b.TY))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		
		public override bool Equals(object obj)
		{
			if (obj is Transform)
			{
				return this == (Transform) obj;
			}
			else
			{
				return false;
			}
		}
		
		public bool EqualsStrictly(object obj)
		{
			if (obj is Transform)
			{
				Transform t = (Transform) obj;
				
				if ((this.XX == t.XX) &&
					(this.XY == t.XY) &&
					(this.YX == t.YX) &&
					(this.YY == t.YY) &&
					(this.TX == t.TX) &&
					(this.TY == t.TY))
				{
					return true;
				}
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append ("[ ");
			buffer.Append (this.xx.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (" ");
			buffer.Append (this.xy.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (" ");
			buffer.Append (this.yx.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (" ");
			buffer.Append (this.yy.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (" ");
			buffer.Append (this.tx.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (" ");
			buffer.Append (this.ty.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (" ]");
			
			return buffer.ToString ();
		}


		private static readonly double			ε = 0.00001;
		
		private readonly double					xx, xy, yx, yy;
		private readonly double					tx, ty;
	}
}
