namespace Epsitec.Common.Grafix
{
	/// <summary>
	/// La classe Transform représente une transformation 2D (matrice 2x2 et vecteur de
	/// translation).
	/// </summary>
	public class Transform
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
		
		public static Transform FromTranslation(float tx, float ty)
		{
			return new Transform (1, 0, 0, 1, tx, ty);
		}
		
		public static Transform FromRotation(int angle)
		{
			double alpha = angle * System.Math.PI / 180;
			float  sin   = (float) System.Math.Sin (alpha);
			float  cos   = (float) System.Math.Cos (alpha);
			
			return new Transform (cos, -sin, sin, cos, 0, 0);
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
		
		
		private float				xx, xy, yx, yy;
		private float				tx, ty;
	}
}
