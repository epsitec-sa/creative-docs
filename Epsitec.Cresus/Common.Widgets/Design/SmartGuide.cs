namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe SmartGuide propose un service d'alignement au moyen de guides
	/// verticaux et horizontaux, basé sur la connaissance du widget en cours de
	/// déplacement et du widget cible (conteneur, futur parent).
	/// </summary>
	public class SmartGuide
	{
		public SmartGuide()
		{
		}
		
		public SmartGuide(Widget widget) : this()
		{
			this.widget  = widget;
		}
		
		public SmartGuide(Widget widget, Drawing.GripId id) : this(widget)
		{
			this.grip_id = id;
		}
		
		public SmartGuide(Widget widget, Drawing.GripId id, Widget target) : this(widget, id)
		{
			this.target = target;
		}
		
		
		public Drawing.GripId			GripId
		{
			get { return this.grip_id; }
			set { this.grip_id = value; }
		}
		
		public Widget					Widget
		{
			get { return this.widget; }
			set { this.widget = value; }
		}
		
		public Widget					Target
		{
			get { return this.target; }
			set { this.target = value; }
		}
		
		
		public void Constrain(Drawing.Rectangle bounds, Constraint cx, Constraint cy)
		{
			if ((this.widget != null) &&
				(this.target != null))
			{
				if (this.grip_id == Drawing.GripId.Body)
				{
					this.ConstrainVerticals (bounds, cx, Drawing.EdgeId.Left | Drawing.EdgeId.Right);
					this.ConstrainHorizontals (bounds, cy, Drawing.EdgeId.Top | Drawing.EdgeId.Bottom);
				}
				else
				{
					Drawing.EdgeId edges = Drawing.Rectangle.ConvertToEdges (this.grip_id);
					
					if ((edges & (Drawing.EdgeId.Left | Drawing.EdgeId.Right)) != 0)
					{
						this.ConstrainVerticals (bounds, cx, edges);
					}
					if ((edges & (Drawing.EdgeId.Top | Drawing.EdgeId.Bottom)) != 0)
					{
						this.ConstrainHorizontals (bounds, cy, edges);
					}
				}
			}
		}
		
		
		protected void ConstrainVerticals(Drawing.Rectangle bounds, Constraint constraint, Drawing.EdgeId edges)
		{
			double xl = bounds.Left;
			double xr = bounds.Right;
			
			Widget            widget = this.target;
			Drawing.Rectangle model  = widget.Client.Bounds;
			
			double mx;
			
			if ((edges & Drawing.EdgeId.Left) != 0)
			{
				mx = model.Left + widget.DockMargins.Left;
				constraint.Add (xl, mx, mx, model.Bottom, mx, model.Top);
			}
			
			if ((edges & Drawing.EdgeId.Right) != 0)
			{
				mx = model.Right - widget.DockMargins.Right;
				constraint.Add (xr, mx, mx, model.Bottom, mx, model.Top);
			}
			
			Widget[] children = widget.Children.Widgets;
			
			for (int i = 0; i < children.Length; i++)
			{
				widget = children[i];
				model  = widget.Bounds;
				
				double y1 = System.Math.Min (model.Bottom, bounds.Bottom);
				double y2 = System.Math.Max (model.Top, bounds.Top);
				
				if ((edges & Drawing.EdgeId.Left) != 0)
				{
					//	Analyse l'alignement du bord gauche avec le bord gauche d'autres widgets, et dans une
					//	moindre mesure leur bord droit.
					
					mx = model.Left;
					constraint.Add (xl, mx, mx, y1, mx, y2);
					
					if ((bounds.Bottom < model.Top) &&
						(bounds.Top > model.Bottom))
					{
						mx = model.Right;
						constraint.Add (xl, mx, mx, y1, mx, y2);
					}
				}
				
				if ((edges & Drawing.EdgeId.Right) != 0)
				{
					//	Analyse l'alignement du bord droit avec le bord droit d'autres widgets, et dans une
					//	moindre mesure leur bord gauche.
					
					mx = model.Right;
					constraint.Add (xr, mx, mx, y1, mx, y2);
					
					if ((bounds.Bottom < model.Top) &&
						(bounds.Top > model.Bottom))
					{
						mx = model.Left;
						constraint.Add (xr, mx, mx, y1, mx, y2);
					}
				}
			}
		}
		
		protected void ConstrainHorizontals(Drawing.Rectangle bounds, Constraint constraint, Drawing.EdgeId edges)
		{
		}
		
		
		protected Drawing.GripId		grip_id;
		protected Widget				widget;
		protected Widget				target;
	}
}
