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
			this.segments = new Drawing.Segment[0];
			this.distance = Constraint.Infinite;
			this.bounds   = Drawing.Rectangle.Empty;
			this.priority = Priority.Low;
			this.anchor   = AnchorStyles.None;
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2)
		{
			this.Add (coord, model_coord, x1, y1, x2, y2, Priority.Low, AnchorStyles.None);
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2, Priority priority)
		{
			this.Add (coord, model_coord, x1, y1, x2, y2, priority, AnchorStyles.None);
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2, Priority priority, AnchorStyles anchor_hint)
		{
			double delta  = coord - model_coord;
			double weight = priority == Priority.High ? System.Math.Abs (delta * 0.8) : System.Math.Abs (delta);
			
			if (weight > this.filter_above)
			{
				return;
			}
			
			Drawing.Segment seg = new Drawing.Segment (new Drawing.Point (x1, y1), new Drawing.Point (x2, y2));
			
			this.anchor |= anchor_hint;
			
			if ((delta == this.distance) &&
				(priority == this.priority))
			{
				System.Diagnostics.Debug.Assert (this.segments.Length > 0);
				System.Diagnostics.Debug.Assert (this.segments[0].Orientation == seg.Orientation);
				
				for (int i = 0; i < this.segments.Length; i++)
				{
					if (Drawing.Segment.CompareAlignment (this.segments[i], seg))
					{
						this.segments[i] = Drawing.Segment.Merge (this.segments[i], seg);
						return;
					}
				}
				
				Drawing.Segment[] copy = new Drawing.Segment[this.segments.Length + 1];
				
				this.segments.CopyTo (copy, 1);
				this.segments = copy;
				this.segments[0] = seg;
			}
			else if ((weight < System.Math.Abs (this.distance)) &&
				/**/ (priority >= this.priority))
			{
				this.priority = priority;
				this.distance = delta;
				this.segments = new Drawing.Segment[1];
				this.segments[0] = seg;
			}
		}
		
		public Constraint Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as Constraint;
		}
		
		
		public double					Distance
		{
			get { return this.distance; }
			set { this.distance = value; }
		}
		
		public Drawing.Rectangle		Bounds
		{
			get { return this.bounds; }
			set { this.bounds = value; }
		}
		
		public Drawing.Segment[]		Segments
		{
			get { return this.segments; }
		}
		
		public AnchorStyles				Anchor
		{
			get { return this.anchor; }
		}
		public bool						IsValid
		{
			get { return this.distance < Constraint.Infinite; }
		}
		
		
		public const double				Infinite = 1000000;
		
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
			that.anchor   = this.anchor;
			that.segments = new Drawing.Segment[this.segments.Length];
			
			this.segments.CopyTo (that.segments, 0);
			
			return that;
		}
		
		
		public enum Priority
		{
			Low,
			High
		}
		
		
		
		protected Priority				priority;
		protected double				filter_above = 10;
		protected double				distance;
		protected Drawing.Rectangle		bounds;
		protected Drawing.Segment[]		segments;
		protected AnchorStyles			anchor;
	}
}
