//	Copyright © 2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Drawing
{
	public struct PointDistance : System.IEquatable<PointDistance>, System.IComparable<PointDistance>
	{
		public PointDistance(Point point, double distance)
		{
			this.point = point;
			this.distance = distance;
		}

		
		public Point Point
		{
			get
			{
				return this.point;
			}
		}

		public double Distance
		{
			get
			{
				return this.distance;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return double.IsPositiveInfinity (this.distance) && this.point.IsZero;
			}
		}

		public static readonly PointDistance Empty = new PointDistance (Point.Zero, double.PositiveInfinity);


		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "pt={0} d={1}", this.point, this.distance);
		}

		public override bool  Equals(object obj)
		{
 			 if (obj is PointDistance)
			 {
				 return this.Equals ((PointDistance) obj);
			 }

			return false;
		}

		public override int  GetHashCode()
		{
 			 return this.point.GetHashCode () ^ this.distance.GetHashCode ();
		}


		#region IEquatable<PointDistance> Members

		public bool Equals(PointDistance other)
		{
			return this.point == other.point
				&& this.distance == other.distance;
		}

		#endregion

		#region IComparable<PointDistance> Members

		public int  CompareTo(PointDistance other)
		{
 			double delta = other.distance - this.distance;
			
			if (delta < 0)
			{
				return -1;
			}
			
			if (delta > 0)
			{
				return 1;
			}

			return 0;
		}

		#endregion
		
		
		private readonly Point					point;
		private readonly double					distance;
	}
}
