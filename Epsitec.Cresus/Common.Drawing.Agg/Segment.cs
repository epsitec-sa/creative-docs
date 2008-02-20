//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public enum SegmentOrientation
	{
		Invalid,
		Vertical,
		Horizontal
	}
	
	/// <summary>
	/// La classe Segment représente un segment horizontal ou vertical.
	/// </summary>
	
	[System.Serializable]
	
	public struct Segment
	{
		public Segment(Drawing.Point p1, Drawing.Point p2)
		{
			this.orientation = Segment.GetOrientation (p1, p2);
			
			if (this.orientation == SegmentOrientation.Horizontal)
			{
				this.location = new Drawing.Point ((p1.X < p2.X) ? p1.X : p2.X, p1.Y);
				this.length   = System.Math.Abs (p1.X - p2.X);
			}
			else if (this.orientation == SegmentOrientation.Vertical)
			{
				this.location = new Drawing.Point (p1.X, (p1.Y < p2.Y) ? p1.Y : p2.Y);
				this.length   = System.Math.Abs (p1.Y - p2.Y);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Points {0} and {1} must be aligned.", p1, p2));
			}
		}
		
		
		public SegmentOrientation				Orientation
		{
			get { return this.orientation; }
			set { this.orientation = value; }
		}
		
		public Drawing.Point					Location
		{
			get { return this.location; }
			set { this.location = value; }
		}
		
		public double							Length
		{
			get { return this.length; }
			set { this.length = value; }
		}
		
		public Drawing.Point					P1
		{
			get { return this.location; }
		}
		
		public Drawing.Point					P2
		{
			get
			{
				if (this.orientation == SegmentOrientation.Horizontal)
				{
					return new Drawing.Point (this.location.X + this.length, this.location.Y);
				}
				if (this.orientation == SegmentOrientation.Vertical)
				{
					return new Drawing.Point (this.location.X, this.location.Y + this.length);
				}
				
				return this.location;
			}
		}
		
		
		public override string ToString()
		{
			return string.Format ("[{0}:{1}:{2}:{3}]", this.location.X, this.location.Y, this.length, this.orientation);
		}

		
		public override bool Equals(object obj)
		{
			return (obj is Segment) && (this == (Segment)obj);
		}
		
		public override int GetHashCode()
		{
			return this.orientation.GetHashCode () ^ this.location.GetHashCode () ^ this.length.GetHashCode ();
		}
		
		
		public static Segment Merge(Segment a, Segment b)
		{
			if (a.Orientation == b.Orientation)
			{
				if (a.Orientation == SegmentOrientation.Vertical)
				{
					Segment seg = new Segment ();
					seg.location.X  = a.P1.X;
					seg.location.Y  = System.Math.Min (a.P1.Y, b.P1.Y);
					seg.length      = System.Math.Max (a.P2.Y, b.P2.Y) - seg.location.Y;
					seg.orientation = a.orientation;
					return seg;
				}
				else if (a.Orientation == SegmentOrientation.Horizontal)
				{
					Segment seg = new Segment ();
					seg.location.X  = System.Math.Min (a.P1.X, b.P1.X);
					seg.location.Y  = a.P1.Y;
					seg.length      = System.Math.Max (a.P2.X, b.P2.X) - seg.location.X;
					seg.orientation = a.orientation;
					return seg;
				}
			}
			
			throw new System.ArgumentException (string.Format ("Segments {0} and {1} must be aligned.", a, b));
		}
		
		public static SegmentOrientation GetOrientation(Drawing.Point p1, Drawing.Point p2)
		{
			if (p1 == p2)
			{
				return SegmentOrientation.Invalid;
			}
			if (p1.X == p2.X)
			{
				return SegmentOrientation.Vertical;
			}
			if (p1.Y == p2.Y)
			{
				return SegmentOrientation.Horizontal;
			}
		
			return SegmentOrientation.Invalid;
		}
		
		
		public static bool CompareAlignment(Segment a, Segment b)
		{
			if (a.Orientation == b.Orientation)
			{
				if (a.Orientation == SegmentOrientation.Vertical)
				{
					return a.location.X == b.location.X;
				}
				if (a.Orientation == SegmentOrientation.Horizontal)
				{
					return a.location.Y == b.location.Y;
				}
			}
			
			return false;
		}
		
		
		public static bool operator ==(Segment a, Segment b)
		{
			return (a.orientation == b.orientation) && (a.location == b.location) && (a.length == b.length);
		}
		
		public static bool operator !=(Segment a, Segment b)
		{
			return (a.orientation != b.orientation) || (a.location != b.location) || (a.length != b.length);
		}
		
		
		private SegmentOrientation				orientation;
		private Drawing.Point					location;
		private double							length;
	}
}
