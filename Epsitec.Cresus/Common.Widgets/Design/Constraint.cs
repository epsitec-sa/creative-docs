namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe Constraint représente une contrainte (verticale ou horizontale)
	/// utilisée par le système de drag & drop pour positionner les widgets, par
	/// exemple.
	/// </summary>
	public class Constraint : System.ICloneable
	{
		public Constraint()
		{
			this.Clear ();
		}
		
		public Constraint(double filter_above) : this()
		{
			this.filter_above = filter_above;
		}
		
		
		public void Clear()
		{
			this.segments = new Segment[0];
			this.distance = Constraint.Infinite;
			this.bounds   = Drawing.Rectangle.Empty;
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2)
		{
			double delta = coord - model_coord;
			
			if (System.Math.Abs (delta) > this.filter_above)
			{
				return;
			}
			
			Segment seg = new Segment (new Drawing.Point (x1, y1), new Drawing.Point (x2, y2));
			
			if (delta == this.distance)
			{
				System.Diagnostics.Debug.Assert (this.segments.Length > 0);
				System.Diagnostics.Debug.Assert (this.segments[0].Orientation == seg.Orientation);
				
				for (int i = 0; i < this.segments.Length; i++)
				{
					if (Segment.CompareAlignment (this.segments[i], seg))
					{
						this.segments[i] = Segment.Merge (this.segments[i], seg);
						return;
					}
				}
				
				Segment[] copy = new Segment[this.segments.Length + 1];
				
				this.segments.CopyTo (copy, 1);
				this.segments = copy;
				this.segments[0] = seg;
			}
			else if (System.Math.Abs (delta) < System.Math.Abs (this.distance))
			{
				this.distance = delta;
				this.segments = new Segment[1];
				this.segments[0] = seg;
			}
		}
		
		public Constraint Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as Constraint;
		}
		
		
		public double				Distance
		{
			get { return this.distance; }
			set { this.distance = value; }
		}
		
		public Drawing.Rectangle	Bounds
		{
			get { return this.bounds; }
			set { this.bounds = value; }
		}
		
		public Segment[]			Segments
		{
			get { return this.segments; }
		}
		
		
		public const double			Infinite = 1000000;
		
		public override bool Equals(object obj)
		{
			Constraint c = obj as Constraint;
			
			if (c != null)
			{
				if ((c.Distance == this.Distance) &&
					(c.Bounds == this.Bounds) &&
					(c.segments.Length == this.segments.Length))

				{
					for (int i = 0; i < this.segments.Length; i++)
					{
						if (c.segments[i] != this.segments[i])
						{
							return false;
						}
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			return this.bounds.GetHashCode ()
				 ^ this.distance.GetHashCode ();
		}
		
		
		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.Clone ();
		}
		#endregion
		
		protected virtual object CloneNewObject()
		{
			return new Constraint ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			Constraint that = o as Constraint;
			
			that.distance = this.distance;
			that.bounds   = this.bounds;
			that.segments = new Segment[this.segments.Length];
			
			this.segments.CopyTo (that.segments, 0);
			
			return that;
		}
		
		public enum Orientation
		{
			Any,
			Vertical,
			Horizontal
		}
		
		
		public struct Segment
		{
			public Segment(Drawing.Point p1, Drawing.Point p2)
			{
				this.orientation = Segment.GetOrientation (p1, p2);
				
				if (this.orientation == Orientation.Horizontal)
				{
					this.location = new Drawing.Point ((p1.X < p2.X) ? p1.X : p2.X, p1.Y);
					this.length   = System.Math.Abs (p1.X - p2.X);
				}
				else if (this.orientation == Orientation.Vertical)
				{
					this.location = new Drawing.Point (p1.X, (p1.Y < p2.Y) ? p1.Y : p2.Y);
					this.length   = System.Math.Abs (p1.Y - p2.Y);
				}
				else
				{
					throw new System.ArgumentException (string.Format ("Points {0} and {1} must be aligned.", p1, p2));
				}
			}
			
			
			public Orientation		Orientation
			{
				get { return this.orientation; }
				set { this.orientation = value; }
			}
			
			public Drawing.Point	Location
			{
				get { return this.location; }
				set { this.location = value; }
			}
			
			public double			Length
			{
				get { return this.length; }
				set { this.length = value; }
			}
			
			public Drawing.Point	P1
			{
				get { return this.location; }
			}
			
			public Drawing.Point	P2
			{
				get
				{
					if (this.orientation == Orientation.Horizontal)
					{
						return new Drawing.Point (this.location.X + this.length, this.location.Y);
					}
					if (this.orientation == Orientation.Vertical)
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
				if (obj is Segment)
				{
					Segment s = (Segment) obj;
					return (this == s);
				}
				
				return false;
			}
			
			public override int GetHashCode()
			{
				return this.orientation.GetHashCode () ^ this.location.GetHashCode () ^ this.length.GetHashCode ();
			}
			
			
			public static Segment Merge(Segment a, Segment b)
			{
				if (a.Orientation == b.Orientation)
				{
					if (a.Orientation == Orientation.Vertical)
					{
						Segment seg = new Segment ();
						seg.location.X  = a.P1.X;
						seg.location.Y  = System.Math.Min (a.P1.Y, b.P1.Y);
						seg.length      = System.Math.Max (a.P2.Y, b.P2.Y) - seg.location.Y;
						seg.orientation = a.orientation;
						return seg;
					}
					else if (a.Orientation == Orientation.Horizontal)
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
			
			public static Orientation GetOrientation(Drawing.Point p1, Drawing.Point p2)
			{
				if (p1 == p2)
				{
					return Orientation.Any;
				}
				if (p1.X == p2.X)
				{
					return Orientation.Vertical;
				}
				if (p1.Y == p2.Y)
				{
					return Orientation.Horizontal;
				}
			
				return Orientation.Any;
			}
			
			public static bool CompareAlignment(Segment a, Segment b)
			{
				if (a.Orientation == b.Orientation)
				{
					if (a.Orientation == Orientation.Vertical)
					{
						return a.location.X == b.location.X;
					}
					else if (a.Orientation == Orientation.Horizontal)
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
			
			
			private Orientation		orientation;
			private Drawing.Point	location;
			private double			length;
		}
		
		
		protected double			filter_above = 10;
		protected double			distance;
		protected Drawing.Rectangle	bounds;
		protected Segment[]			segments;
	}
}
