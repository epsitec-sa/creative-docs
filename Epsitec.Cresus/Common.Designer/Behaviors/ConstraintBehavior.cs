//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Behaviors
{
	using ArrayList = System.Collections.ArrayList;
	
	/// <summary>
	/// La classe ConstraintBehavior représente une contrainte (verticale ou horizontale)
	/// utilisée par le système de drag & drop pour positionner les widgets, par
	/// exemple.
	/// </summary>
	public class ConstraintBehavior : System.ICloneable
	{
		public ConstraintBehavior()
		{
			this.Clear ();
		}
		
		public ConstraintBehavior(double filter_above) : this()
		{
			this.filter_above = filter_above;
		}
		
		
		public void Clear()
		{
			this.segments = new Drawing.Segment[0];
			this.hints    = new ArrayList ();
			this.distance = ConstraintBehavior.Infinite;
			this.bounds   = Drawing.Rectangle.Empty;
			this.priority = Priority.Low;
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2)
		{
			this.Add (coord, model_coord, x1, y1, x2, y2, Priority.Low, null);
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2, Priority priority)
		{
			this.Add (coord, model_coord, x1, y1, x2, y2, priority, null);
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2, Priority priority, AnchorStyles anchor)
		{
			this.Add (coord, model_coord, x1, y1, x2, y2, priority, new Hint (anchor));
		}
		
		public void Add (double coord, double model_coord, double x1, double y1, double x2, double y2, Priority priority, Hint hint)
		{
			double delta  = coord - model_coord;
			double weight = priority == Priority.High ? System.Math.Abs (delta * 0.8) : System.Math.Abs (delta);
			
			if (weight > this.filter_above)
			{
				return;
			}
			
			Drawing.Segment seg = new Drawing.Segment (new Drawing.Point (x1, y1), new Drawing.Point (x2, y2));
			
			if (hint != null)
			{
				hint = hint.Clone ();
				hint.Weight = weight;
				this.hints.Add (hint);
			}
			
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
		
		
		public ConstraintBehavior Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as ConstraintBehavior;
		}
		
		
		public double							Distance
		{
			get { return this.distance; }
			set { this.distance = value; }
		}
		
		public Drawing.Rectangle				Bounds
		{
			get { return this.bounds; }
			set { this.bounds = value; }
		}
		
		public Drawing.Segment[]				Segments
		{
			get { return this.segments; }
		}
		
		public Hint[]							Hints
		{
			get
			{
				Hint[] hints = new Hint[this.hints.Count];
				this.hints.CopyTo (hints);
				System.Array.Sort (hints);
				return hints;
			}
		}
		public bool								IsValid
		{
			get { return this.distance < ConstraintBehavior.Infinite; }
		}
		
		
		public const double						Infinite = 1000000;
		
		public override bool Equals(object obj)
		{
			ConstraintBehavior c = obj as ConstraintBehavior;
			
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
			return new ConstraintBehavior ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			ConstraintBehavior that = o as ConstraintBehavior;
			
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
		
		public class Hint : System.IComparable
		{
			public Hint(Widget widget) : this(widget, AnchorStyles.None)
			{
			}
			
			public Hint(AnchorStyles anchor) : this(null, anchor)
			{
			}
			
			public Hint(Widget widget, AnchorStyles anchor)
			{
				this.widget = widget;
				this.anchor = anchor;
			}
			
			
			public Hint Clone()
			{
				Hint copy = new Hint (this.widget, this.anchor);
				
				copy.weight = this.weight;
				
				return copy;
			}
			
			
			public double						Weight
			{
				get { return this.weight; }
				set { this.weight = value; }
			}
			
			public Widget						Widget
			{
				get { return this.widget; }
			}
			
			public AnchorStyles					Anchor
			{
				get { return this.anchor; }
			}
			
			
			#region IComparable Members
			public int CompareTo(object obj)
			{
				Hint that = obj as Hint;
				
				if (that == null)
				{
					return 1;
				}
				
				return this.weight.CompareTo (that.weight);
			}
			#endregion
			
			public static AnchorStyles MergedAnchor(Hint[] hints)
			{
				AnchorStyles anchor = AnchorStyles.None;
				
				for (int i = 0; i < hints.Length; i++)
				{
					anchor |= hints[i].Anchor;
				}
				
				return anchor;
			}
			
			
			protected Widget					widget;
			protected AnchorStyles				anchor;
			protected double					weight;
		}
		
		
		
		protected Priority						priority;
		protected double						filter_above = 10;
		protected double						distance;
		protected Drawing.Rectangle				bounds;
		protected Drawing.Segment[]				segments;
		protected ArrayList						hints;
		protected AnchorStyles					anchor;
	}
}
