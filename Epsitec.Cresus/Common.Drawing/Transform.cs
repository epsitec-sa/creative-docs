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
		
		public Transform(float xx, float xy, float yx, float yy, float tx, float ty)
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
		
		
		public float				XX
		{
			get { return this.xx; }
			set { this.xx = value; }
		}
		
		public float				XY
		{
			get { return this.xy; }
			set { this.xy = value; }
		}
		
		public float				YX
		{
			get { return this.yx; }
			set { this.yx = value; }
		}
		
		public float				YY
		{
			get { return this.yy; }
			set { this.yy = value; }
		}
		
		public float				TX
		{
			get { return this.tx; }
			set { this.tx = value; }
		}
		
		public float				TY
		{
			get { return this.ty; }
			set { this.ty = value; }
		}
		
		
		public System.Drawing.PointF TransformDirect(System.Drawing.PointF pt)
		{
			float x = pt.X;
			float y = pt.Y;
			
			pt.X = this.xx * x + this.xy * y + this.tx;
			pt.Y = this.yx * x + this.yy * y + this.ty;
			
			return pt;
		}
		
		public System.Drawing.PointF TransformInverse(System.Drawing.PointF pt)
		{
			float det = this.xx * this.yy - this.xy * this.yx;
			
			System.Diagnostics.Debug.Assert (det != 0.0f);
			
			float x = pt.X - this.tx;
			float y = pt.Y - this.ty;
			
			pt.X = (  this.yy * x - this.xy * y) / det;
			pt.Y = (- this.yx * x + this.xx * y) / det;
			
			return pt;
		}
		
		
		public void Reset()
		{
			this.xx = 1;
			this.xy = 0;
			this.tx = 0;
			this.yx = 0;
			this.yy = 1;
			this.ty = 0;
		}
		
		public void MultiplyBy(Transform t)
		{
			Transform c = Transform.Multiply (t, this);
			
			this.xx = c.xx;
			this.xy = c.xy;
			this.tx = c.tx;
			this.yx = c.yx;
			this.yy = c.yy;
			this.ty = c.ty;
		}
		
		public void MultiplyByPostfix(Transform t)
		{
			Transform c = Transform.Multiply (this, t);
			
			this.xx = c.xx;
			this.xy = c.xy;
			this.tx = c.tx;
			this.yx = c.yx;
			this.yy = c.yy;
			this.ty = c.ty;
		}
		
		public void Round()
		{
			Round (ref this.xx);
			Round (ref this.xy);
			Round (ref this.yx);
			Round (ref this.yy);
			Round (ref this.tx);
			Round (ref this.ty);
		}
		
		public void Translate(float tx, float ty)
		{
			this.tx += tx;
			this.ty += ty;
		}
		
		public void Rotate(int angle)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotation (angle));
			}
		}
		
		public void Rotate(int angle, System.Drawing.PointF center)
		{
			if (angle != 0)
			{
				this.MultiplyBy (Transform.FromRotation (angle, center));
			}
		}
		
		public void Scale(float s)
		{
			if (s != 1.0f)
			{
				this.Scale (s, s);
			}
		}
		
		public void Scale(float sx, float sy)
		{
			this.xx *= sx;
			this.xy *= sy;
			this.yx *= sx;
			this.yy *= sy;
			this.tx *= sx;
			this.ty *= sy;
		}
		
		
		public static Transform FromScale(float sx, float sy)
		{
			return new Transform (sx, 0, 0, sy, 0, 0);
		}
		
		public static Transform FromTranslation(float tx, float ty)
		{
			return new Transform (1, 0, 0, 1, tx, ty);
		}
		
		public static Transform FromRotation(int angle)
		{
			double alpha = angle * System.Math.PI / 180;
			float  sin   = (float) System.Math.Sin (alpha);
			float  cos   = (float) System.Math.Cos (alpha);
			
			return new Transform (cos, sin, -sin, cos, 0, 0);
		}
		
		public static Transform FromRotation(int angle, System.Drawing.PointF center)
		{
			Transform m = FromRotation (angle);
			
			float cx = center.X;
			float cy = center.Y;
			
			m.tx = - (m.xx * cx + m.xy * cy) + cx;
			m.ty = - (m.yx * cx + m.yy * cy) + cy;
			
			return m;
		}
		
		public static Transform Inverse(Transform m)
		{
			float det   = m.xx * m.yy - m.xy * m.yx;
			Transform c = new Transform ();
			
			System.Diagnostics.Debug.Assert (det != 0.0f);
			
			float det_1 = 1.0f / det;
			
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
		
		public static System.Drawing.PointF Multiply(Transform a, System.Drawing.PointF b)
		{
			System.Drawing.PointF c = new System.Drawing.PointF ();
			
			c.X = a.xx * b.X + a.xy * b.Y + a.tx;
			c.Y = a.yx * b.X + a.yy * b.Y + a.ty;
			
			return c;
		}
		
		
		protected static readonly float epsilon = 0.00001f;
		
		public static bool Equal(float a, float b)
		{
			float delta = a - b;
			return (delta < epsilon) && (delta > -epsilon);
		}
		
		public static bool Equal(System.Drawing.PointF a, System.Drawing.PointF b)
		{
			return Equal (a.X, b.X) && Equal (a.Y, b.Y);
		}
		
		public static bool Equal(System.Drawing.SizeF a, System.Drawing.SizeF b)
		{
			return Equal (a.Width, b.Width) && Equal (a.Height, b.Height);
		}
		
		public static bool Equal(System.Drawing.RectangleF a, System.Drawing.RectangleF b)
		{
			return Equal (a.Location, b.Location) && Equal (a.Size, b.Size);
		}
		
		public static bool IsZero(float a)
		{
			return (a < epsilon) && (a > -epsilon);
		}
		
		public static bool IsOne(float a)
		{
			return (a < 1+epsilon) && (a > 1-epsilon);
		}
		
		public static void Round(ref float a)
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
				if (Equal (this.XX, t.XX) &&
					Equal (this.XY, t.XY) &&
					Equal (this.YX, t.YX) &&
					Equal (this.YY, t.YY) &&
					Equal (this.TX, t.TX) &&
					Equal (this.TY, t.TY))
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
		
		public override bool Equals(object obj)
		{
			if (obj is Transform)
			{
				Transform t = obj as Transform;
				
				if (t == null)
				{
					return false;
				}
				if (Equal (this.XX, t.XX) &&
					Equal (this.XY, t.XY) &&
					Equal (this.YX, t.YX) &&
					Equal (this.YY, t.YY) &&
					Equal (this.TX, t.TX) &&
					Equal (this.TY, t.TY))
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
		
		
		private float				xx, xy, yx, yy;
		private float				tx, ty;
	}
}
