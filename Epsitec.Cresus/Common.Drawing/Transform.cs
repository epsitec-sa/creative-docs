//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Transform représente une transformation 2D (matrice 2x2 et vecteur de
	/// translation). Elle supporte les opérations telles que la translation simple, la
	/// rotation, le changement d'échelle, ainsi que leur combinaison.
	/// </summary>
	public class Transform : System.IComparable
	{
		public Transform() : this (1, 0, 0, 1, 0, 0)
		{
		}
		
		public Transform(double xx, double xy, double yx, double yy, double tx, double ty)
		{
			this.xx = xx;
			this.xy = xy;
			this.yx = yx;
			this.yy = yy;
			this.tx = tx;
			this.ty = ty;
		}
		
		public Transform(Transform transform)
		{
			this.xx = transform.xx;
			this.xy = transform.xy;
			this.yx = transform.yx;
			this.yy = transform.yy;
			this.tx = transform.tx;
			this.ty = transform.ty;
		}
		
		
		public bool					OnlyTranslate
		{
			get
			{
				if ((this.xx == 1.0) &&
					(this.yy == 1.0) &&
					(this.xy == 0.0) &&
					(this.yx == 0.0))
				{
					return true;
				}
				
				return false;
			}
		}
		
		public double				XX
		{
			get { return this.xx; }
			set
			{
				if (this.xx != value)
				{
					this.xx = value;
					this.OnChanged ();
				}
			}
		}
		
		public double				XY
		{
			get { return this.xy; }
			set
			{
				if (this.xy != value)
				{
					this.xy = value;
					this.OnChanged ();
				}
			}
		}
		
		public double				YX
		{
			get { return this.yx; }
			set
			{
				if (this.yx != value)
				{
					this.yx = value;
					this.OnChanged ();
				}
			}
		}
		
		public double				YY
		{
			get { return this.yy; }
			set
			{
				if (this.yy != value)
				{
					this.yy = value;
					this.OnChanged ();
				}
			}
		}
		
		public double				TX
		{
			get { return this.tx; }
			set
			{
				if (this.tx != value)
				{
					this.tx = value;
					this.OnChanged ();
				}
			}
		}
		
		public double				TY
		{
			get { return this.ty; }
			set
			{
				if (this.ty != value)
				{
					this.ty = value;
					this.OnChanged ();
				}
			}
		}
		
		
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
		
		public void TransformInverse(ref double px, ref double py)
		{
			double det = this.xx * this.yy - this.xy * this.yx;
			
			System.Diagnostics.Debug.Assert (det != 0.0f);
			
			double x = px - this.tx;
			double y = py - this.ty;
			
			px = (  this.yy * x - this.xy * y) / det;
			py = (- this.yx * x + this.xx * y) / det;
		}
		
		
		public void Reset()
		{
			this.xx = 1;
			this.xy = 0;
			this.tx = 0;
			this.yx = 0;
			this.yy = 1;
			this.ty = 0;
			this.OnChanged ();
		}
		
		public void Reset(Transform model)
		{
			this.xx = model.xx;
			this.xy = model.xy;
			this.tx = model.tx;
			this.yx = model.yx;
			this.yy = model.yy;
			this.ty = model.ty;
			this.OnChanged ();
		}
		
		public void MultiplyBy(Transform t)
		{
			if (t != null)
			{
				Transform c = Transform.Multiply (t, this);
				
				this.xx = c.xx;
				this.xy = c.xy;
				this.tx = c.tx;
				this.yx = c.yx;
				this.yy = c.yy;
				this.ty = c.ty;
				
				this.OnChanged ();
			}
		}
		
		public void MultiplyByPostfix(Transform t)
		{
			if (t != null)
			{
				Transform c = Transform.Multiply (this, t);
				
				this.xx = c.xx;
				this.xy = c.xy;
				this.tx = c.tx;
				this.yx = c.yx;
				this.yy = c.yy;
				this.ty = c.ty;
				
				this.OnChanged ();
			}
		}
		
		public void Round()
		{
			Round (ref this.xx);
			Round (ref this.xy);
			Round (ref this.yx);
			Round (ref this.yy);
			Round (ref this.tx);
			Round (ref this.ty);
			
			this.OnChanged ();
		}
		
		public void Translate(double tx, double ty)
		{
			this.tx += tx;
			this.ty += ty;
			
			this.OnChanged ();
		}
		
		public void Translate(Point offset)
		{
			this.tx += offset.X;
			this.ty += offset.Y;
			
			this.OnChanged ();
		}
		
		public void RotateDeg(double angle)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotationDeg (angle));
			}
		}
		
		public void RotateDeg(double angle, Point center)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotationDeg (angle, center.X, center.Y));
			}
		}
		
		public void RotateDeg(double angle, double x, double y)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotationDeg (angle, x, y));
			}
		}
		
		public void RotateRad(double angle)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotationRad (angle));
			}
		}
		
		public void RotateRad(double angle, Point center)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotationRad (angle, center.X, center.Y));
			}
		}
		
		public void RotateRad(double angle, double x, double y)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotationRad (angle, x, y));
			}
		}
		
		public void Scale(double s)
		{
			if (s != 1.0f)
			{
				this.Scale (s, s);
			}
		}
		
		public void Scale(double sx, double sy)
		{
			this.xx *= sx;
			this.xy *= sy;
			this.yx *= sx;
			this.yy *= sy;
			this.tx *= sx;
			this.ty *= sy;
			
			this.OnChanged ();
		}
		
		public void Scale(double sx, double sy, double cx, double cy)
		{
			this.Translate (-cx, -cy);
			this.Scale (sx, sy);
			this.Translate (cx, cy);
			
			this.OnChanged ();
		}
		
		
		public static Transform FromScale(double sx, double sy)
		{
			return new Transform (sx, 0, 0, sy, 0, 0);
		}
		
		public static Transform FromScale(double sx, double sy, double cx, double cy)
		{
			return new Transform (sx, 0, 0, sy, cx-sx*cx, cy-sy*cy);
		}
		
		public static Transform FromTranslation(double tx, double ty)
		{
			return new Transform (1, 0, 0, 1, tx, ty);
		}
		
		public static Transform FromTranslation(Point offset)
		{
			return new Transform (1, 0, 0, 1, offset.X, offset.Y);
		}
		
		public static Transform FromRotationDeg(double angle)
		{
			double alpha = Math.DegToRad (angle);
			double sin   = System.Math.Sin (alpha);
			double cos   = System.Math.Cos (alpha);
			
			return new Transform (cos, -sin, sin, cos, 0, 0);
		}
		
		public static Transform FromRotationDeg(double angle, Point center)
		{
			return Transform.FromRotationDeg (angle, center.X, center.Y);
		}
		
		public static Transform FromRotationDeg(double angle, double cx, double cy)
		{
			Transform m = FromRotationDeg (angle);
			
			m.tx = cx - m.xx * cx - m.xy * cy;
			m.ty = cy - m.yx * cx - m.yy * cy;
			
			return m;
		}
		
		public static Transform FromRotationRad(double angle)
		{
			double sin   = System.Math.Sin (angle);
			double cos   = System.Math.Cos (angle);
			
			return new Transform (cos, -sin, sin, cos, 0, 0);
		}
		
		public static Transform FromRotationRad(double angle, Point center)
		{
			return Transform.FromRotationRad (angle, center.X, center.Y);
		}
		
		public static Transform FromRotationRad(double angle, double cx, double cy)
		{
			Transform m = FromRotationRad (angle);
			
			m.tx = cx - m.xx * cx - m.xy * cy;
			m.ty = cy - m.yx * cx - m.yy * cy;
			
			return m;
		}
		
		
		public static Transform Inverse(Transform m)
		{
			double det   = m.xx * m.yy - m.xy * m.yx;
			Transform c = new Transform ();
			
			if (det == 0)
			{
				throw new System.DivideByZeroException ("Transform matrix cannot be inverted.");
			}
			
			System.Diagnostics.Debug.Assert (det != 0.0);
			
			double det_1 = 1.0f / det;
			
			c.xx =   m.yy * det_1;
			c.xy = - m.xy * det_1;
			c.yx = - m.yx * det_1;
			c.yy =   m.xx * det_1;
			
			c.tx = - c.xx * m.tx - c.xy * m.ty;
			c.ty = - c.yx * m.tx - c.yy * m.ty;
			
			return c;
		}
		
		public static Transform Multiply(Transform a, Transform b)
		{
			Transform c = new Transform ();
			
			c.xx = a.xx * b.xx + a.xy * b.yx;
			c.xy = a.xx * b.xy + a.xy * b.yy;
			c.tx = a.xx * b.tx + a.xy * b.ty + a.tx;
			c.yx = a.yx * b.xx + a.yy * b.yx;
			c.yy = a.yx * b.xy + a.yy * b.yy;
			c.ty = a.yx * b.tx + a.yy * b.ty + a.ty;
			
			return c;
		}
		
		public static Point Multiply(Transform a, Point b)
		{
			Point c = new Point ();
			
			c.X = a.xx * b.X + a.xy * b.Y + a.tx;
			c.Y = a.yx * b.X + a.yy * b.Y + a.ty;
			
			return c;
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

			b.X = a.X*System.Math.Cos(angle) - a.Y*System.Math.Sin(angle);
			b.Y = a.X*System.Math.Sin(angle) + a.Y*System.Math.Cos(angle);

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

			a.X = p.X*System.Math.Cos(angle) - p.Y*System.Math.Sin(angle);
			a.Y = p.X*System.Math.Sin(angle) + p.Y*System.Math.Cos(angle);

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


		
		
		protected static readonly double epsilon = 0.00001;
		
		public static bool Equal(double a, double b)
		{
			double delta = a - b;
			return (delta < epsilon) && (delta > -epsilon);
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
			return (a < epsilon) && (a > -epsilon);
		}
		
		public static bool IsOne(double a)
		{
			return (a < 1+epsilon) && (a > 1-epsilon);
		}
		
		public static void Round(ref double a)
		{
			if (IsZero (a))
				a = 0;
			else if (IsOne (a))
				a = 1;
		}
		
		
		#region IComparable Members
		
		public int CompareTo(object obj)
		{
			if (obj is Transform)
			{
				Transform t = obj as Transform;
				
				if (t == null)
				{
					return 1;
				}
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
				return 1;
			}
			
			throw new System.ArgumentException ("object is not a Transform");
		}
		
		#endregion
		
		public static bool operator ==(Transform a, Transform b)
		{
			object oa = a;
			object ob = b;
			
			if (oa == ob)
			{
				return true;
			}
			
			if ((oa != null) && (ob != null))
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
			}
			
			return false;
		}
		
		public static bool operator !=(Transform a, Transform b)
		{
			object oa = a;
			object ob = b;
			
			if (oa == ob)
			{
				return false;
			}
			
			if ((oa != null) && (ob != null))
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
			}
			
			return true;
		}
		
		
		public override bool Equals(object obj)
		{
			if (obj is Transform)
			{
				Transform t = obj as Transform;
				
				if (t == null)
				{
					return false;
				}
				if (Transform.Equal (this.XX, t.XX) &&
					Transform.Equal (this.XY, t.XY) &&
					Transform.Equal (this.YX, t.YX) &&
					Transform.Equal (this.YY, t.YY) &&
					Transform.Equal (this.TX, t.TX) &&
					Transform.Equal (this.TY, t.TY))
				{
					return true;
				}
			}
			
			return false;
		}
		
		public bool EqualsStrictly(object obj)
		{
			if (obj is Transform)
			{
				Transform t = obj as Transform;
				
				if (t == null)
				{
					return false;
				}
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
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			buffer.Append ("[ ");
			buffer.Append (this.XX.ToString ());
			buffer.Append (" ");
			buffer.Append (this.XY.ToString ());
			buffer.Append (" ");
			buffer.Append (this.YX.ToString ());
			buffer.Append (" ");
			buffer.Append (this.YY.ToString ());
			buffer.Append (" ");
			buffer.Append (this.TX.ToString ());
			buffer.Append (" ");
			buffer.Append (this.TY.ToString ());
			buffer.Append (" ]");
			
			return buffer.ToString ();
		}
					
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}

		
		public event Support.EventHandler	Changed;
		
		private double						xx, xy, yx, yy;
		private double						tx, ty;
	}
}
