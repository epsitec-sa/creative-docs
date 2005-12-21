using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Stretcher permet de déformer un point.
	/// </summary>
	public class Stretcher
	{
		public Stretcher()
		{
		}

		public Rectangle InitialRectangle
		{
			get { return this.initialRectangle; }
			set { this.initialRectangle = value; }
		}

		public Point FinalBottomLeft
		{
			get { return this.finalBottomLeft; }
			set { this.finalBottomLeft = value; }
		}

		public Point FinalBottomRight
		{
			get { return this.finalBottomRight; }
			set { this.finalBottomRight = value; }
		}

		public Point FinalTopLeft
		{
			get { return this.finalTopLeft; }
			set { this.finalTopLeft = value; }
		}

		public Point FinalTopRight
		{
			get { return this.finalTopRight; }
			set { this.finalTopRight = value; }
		}

		public Point Transform(Point pos)
		{
			//	Transforme un point du système "initial" vers le système "final".
			double sx=0.5, sy=0.5;

			if ( this.initialRectangle.Width != 0.0 )
			{
				sx = (pos.X-this.initialRectangle.Left) / this.initialRectangle.Width;
			}

			if ( this.initialRectangle.Height != 0.0 )
			{
				sy = (pos.Y-this.initialRectangle.Bottom) / this.initialRectangle.Height;
			}

			Point bottom = Point.Scale(this.finalBottomLeft, this.finalBottomRight, sx);
			Point top    = Point.Scale(this.finalTopLeft,    this.finalTopRight,    sx);
			return Point.Scale(bottom, top, sy);
		}

		public Point Reverse(Point pos)
		{
			//	Transformation inverse d'un point du système "final" vers le système "initial".
			//	La forme finale ne doit pas contenir d'angle aigu.
			Point[] inter;

			//	Intersection entre les 2 horizontales.
			Point h;
			inter = Geometry.Intersect(this.finalBottomLeft, this.finalBottomRight, this.finalTopLeft, this.finalTopRight);
			if ( inter == null )  // droites parallèles ?
			{
				h = pos+(this.finalBottomRight-this.finalBottomLeft);
			}
			else
			{
				h = inter[0];
			}

			inter = Geometry.Intersect(pos, h, this.finalBottomLeft, this.finalTopLeft);
			if ( inter == null )  return this.initialRectangle.Center;
			Point left = inter[0];

			inter = Geometry.Intersect(pos, h, this.finalBottomRight, this.finalTopRight);
			if ( inter == null )  return this.initialRectangle.Center;
			Point right = inter[0];

			//	Intersection entre les 2 verticales.
			Point v;
			inter = Geometry.Intersect(this.finalBottomLeft, this.finalTopLeft, this.finalBottomRight, this.finalTopRight);
			if ( inter == null )  // droites parallèles ?
			{
				v = pos+(this.finalTopLeft-this.finalBottomLeft);
			}
			else
			{
				v = inter[0];
			}

			inter = Geometry.Intersect(pos, v, this.finalBottomLeft, this.finalBottomRight);
			if ( inter == null )  return this.initialRectangle.Center;
			Point bottom = inter[0];

			inter = Geometry.Intersect(pos, v, this.finalTopLeft, this.finalTopRight);
			if ( inter == null )  return this.initialRectangle.Center;
			Point top = inter[0];

			double cx = Point.Distance(left,   pos) / Point.Distance(left, right);
			double cy = Point.Distance(bottom, pos) / Point.Distance(bottom, top);

			pos.X = this.initialRectangle.Left   + this.initialRectangle.Width*cx;
			pos.Y = this.initialRectangle.Bottom + this.initialRectangle.Height*cy;
			return pos;
		}

		protected Rectangle			initialRectangle;
		protected Point				finalBottomLeft;
		protected Point				finalBottomRight;
		protected Point				finalTopLeft;
		protected Point				finalTopRight;
	}
}
