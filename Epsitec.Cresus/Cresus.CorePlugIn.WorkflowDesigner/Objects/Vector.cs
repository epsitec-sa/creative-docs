//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public struct Vector
	{
		public Vector(Point origin, Point end)
		{
			this.origin = origin;
			this.end    = end;
		}

		public Vector(Point start, Size direction)
		{
			this.origin = start;
			this.end    = start + direction;
		}

		public Vector(Point start, double angle)
		{
			this.origin = start;
			this.end    = Transform.RotatePointDeg (start, angle, new Point (start.X+1, start.Y));
		}

		public Vector(Vector vector, Size offset)
		{
			this.origin = new Point (vector.origin.X+offset.Width, vector.origin.Y+offset.Height);
			this.end    = new Point (vector.end   .X+offset.Width, vector.end   .Y+offset.Height);
		}


		public Point Origin
		{
			get
			{
				return this.origin;
			}
		}

		public Point End
		{
			get
			{
				return this.end;
			}
		}

		public Size Direction
		{
			get
			{
				return new Size (this.end.X-this.origin.X, this.end.Y-this.origin.Y);
			}
		}

		public double Angle
		{
			get
			{
				return Point.ComputeAngleDeg (this.origin, this.end);
			}
		}

		public bool HasDirection
		{
			get
			{
				return this.end.X != this.origin.X || this.end.Y != this.origin.Y;
			}
		}


		public Point GetPoint(double distance)
		{
			return Point.Move (this.origin, this.end, distance);
		}


		public bool IsValid
		{
			get
			{
				return !this.IsZero;
			}
		}

		public bool IsZero
		{
			get
			{
				return origin.IsZero || end.IsZero;
			}
		}

		public static readonly Vector Zero;

		private readonly Point origin;
		private readonly Point end;
	}
}
