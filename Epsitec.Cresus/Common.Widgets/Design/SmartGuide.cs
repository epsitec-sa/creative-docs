namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe SmartGuide propose un service d'alignement au moyen de guides
	/// verticaux et horizontaux, basé sur la connaissance du widget en cours de
	/// déplacement et du widget cible (conteneur actuel ou futur parent).
	/// </summary>
	public class SmartGuide
	{
		public SmartGuide()
		{
			this.align = Panels.WidgetPalette.Guide;
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
		
		public IGuideAlign				GuideAlign
		{
			get
			{
				if (this.align == SmartGuide.default_align)
				{
					return null;
				}
				
				return this.align;
			}
			set
			{
				if (value == null)
				{
					this.align = SmartGuide.default_align;
				}
				else
				{
					this.align = value;
				}
			}
		}
		
		
		public void Constrain(Drawing.Rectangle bounds, Constraint cx, Constraint cy)
		{
			this.Constrain (bounds, -1, cx, cy);
		}
		
		public void Constrain(Drawing.Rectangle bounds, double base_line_offset, Constraint cx, Constraint cy)
		{
			if ((this.widget != null) &&
				(this.target != null))
			{
				if (this.grip_id == Drawing.GripId.Body)
				{
					//	Si on déplace un widget par sa poignée centrale, alors on accepte des alignements
					//	sur tous les bords du widget, y compris sur la ligne de base.
					
					this.ConstrainVerticals (bounds, cx, Drawing.EdgeId.Left | Drawing.EdgeId.Right);
					this.ConstrainHorizontals (bounds, base_line_offset, cy, Drawing.EdgeId.Top | Drawing.EdgeId.Bottom);
				}
				else
				{
					Drawing.EdgeId edges = Drawing.Rectangle.ConvertToEdges (this.grip_id);
					
					//	Pour les poignées périphériques, on utilise uniquement le ou les bord(s) correspondant(s)
					//	pour déterminer l'alignement.
					
					if ((edges & (Drawing.EdgeId.Left | Drawing.EdgeId.Right)) != 0)
					{
						this.ConstrainVerticals (bounds, cx, edges);
					}
					if ((edges & (Drawing.EdgeId.Top | Drawing.EdgeId.Bottom)) != 0)
					{
						this.ConstrainHorizontals (bounds, -1, cy, edges);
					}
				}
			}
		}
		
		
		protected void ConstrainVerticals(Drawing.Rectangle bounds, Constraint constraint, Drawing.EdgeId edges)
		{
			double xl = bounds.Left;
			double xr = bounds.Right;
			
			Widget            widget  = this.target;
			Drawing.Rectangle model   = widget.InnerBounds;
			Drawing.Margins   margins = this.align.GetInnerMargins (widget);
			
			double mx;
			
			//	Considère tout d'abord l'alignement avec les marges intérieures du widget parent :
			
			if ((edges & Drawing.EdgeId.Left) != 0)
			{
				mx = model.Left + widget.DockMargins.Left + margins.Left;
				constraint.Add (xl, mx, mx, model.Bottom, mx, model.Top, Constraint.Priority.Low, AnchorStyles.Left);
			}
			
			if ((edges & Drawing.EdgeId.Right) != 0)
			{
				mx = model.Right - widget.DockMargins.Right - margins.Right;
				constraint.Add (xr, mx, mx, model.Bottom, mx, model.Top, Constraint.Priority.Low, AnchorStyles.Right);
			}
			
			//	Passe en revue tous les frères et détermine les alignements respectifs :
			
			Widget[] children = widget.Children.Widgets;
			
			for (int i = 0; i < children.Length; i++)
			{
				widget  = children[i];
				model   = widget.Bounds;
				margins = this.align.GetAlignMargins (widget, this.widget);
				
				if (widget == this.widget)
				{
					continue;
				}
				
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
						mx = model.Right + margins.Right;
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
						mx = model.Left - margins.Left;
						constraint.Add (xr, mx, mx, y1, mx, y2);
					}
				}
			}
		}
		
		protected void ConstrainHorizontals(Drawing.Rectangle bounds, double base_line_offset, Constraint constraint, Drawing.EdgeId edges)
		{
			double y1 = bounds.Bottom;
			double y2 = bounds.Bottom + base_line_offset;
			double y3 = bounds.Top;
			
			Widget            widget  = this.target;
			Drawing.Rectangle model   = widget.InnerBounds;
			Drawing.Point     basel   = widget.BaseLine;
			Drawing.Margins   margins = this.align.GetInnerMargins (widget);
			
			double my;
			
			//	Considère tout d'abord l'alignement avec les marges intérieures du widget parent :
			
			if ((edges & Drawing.EdgeId.Bottom) != 0)
			{
				my = model.Bottom + widget.DockMargins.Bottom + margins.Bottom;
				constraint.Add (y1, my, model.Left, my, model.Right, my, Constraint.Priority.Low, AnchorStyles.Bottom);
			}
			
			if ((edges & Drawing.EdgeId.Top) != 0)
			{
				my = model.Top - widget.DockMargins.Top - margins.Top;
				constraint.Add (y3, my, model.Left, my, model.Right, my, Constraint.Priority.Low, AnchorStyles.Top);
			}
			
			//	Passe en revue tous les frères et détermine les alignements respectifs :
			
			Widget[] children = widget.Children.Widgets;
			
			for (int i = 0; i < children.Length; i++)
			{
				widget  = children[i];
				model   = widget.Bounds;
				basel   = widget.BaseLine;
				margins = this.align.GetAlignMargins (widget, this.widget);
				
				if (widget == this.widget)
				{
					continue;
				}
				
				double x1 = System.Math.Min (model.Left, bounds.Left);
				double x2 = System.Math.Max (model.Right, bounds.Right);
				
				if ((base_line_offset >= 0) &&
					(! basel.IsEmpty))
				{
					//	Analyse l'alignement de la ligne de base avec celle d'autres widgets.
					
					my = model.Bottom + basel.Y;
					constraint.Add (y2, my, x1, my, x2, my, Constraint.Priority.High);
				}
				
				if ((edges & Drawing.EdgeId.Bottom) != 0)
				{
					//	Analyse l'alignement du bord inférieur avec le bord inférieur d'autres widgets, et dans une
					//	moindre mesure leur bord supérieur.
					
					my = model.Bottom;
					constraint.Add (y1, my, x1, my, x2, my);
					
					if ((bounds.Left < model.Right) &&
						(bounds.Right > model.Left))
					{
						my = model.Top + margins.Top;
						constraint.Add (y1, my, x1, my, x2, my);
					}
				}
				
				if ((edges & Drawing.EdgeId.Top) != 0)
				{
					//	Analyse l'alignement du bord supérieur avec le bord supérieur d'autres widgets, et dans une
					//	moindre mesure leur bord inférieur.
					
					my = model.Top;
					constraint.Add (y3, my, x1, my, x2, my);
					
					if ((bounds.Left < model.Right) &&
						(bounds.Right > model.Left))
					{
						my = model.Bottom - margins.Bottom;
						constraint.Add (y3, my, x1, my, x2, my);
					}
				}
			}
		}
		
		
		#region DefaultGuideAlign class
		protected class DefaultGuideAlign : IGuideAlign
		{
			public Drawing.Margins GetInnerMargins(Widget widget)
			{
				return SmartGuide.root_margins;
			}

			public Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b)
			{
				return SmartGuide.align_margins;
			}
		}
		#endregion
		
		protected Drawing.GripId			grip_id;
		protected Widget					widget;
		protected Widget					target;
		protected IGuideAlign				align;
		
		protected static IGuideAlign		default_align = new DefaultGuideAlign();
		protected static Drawing.Margins	root_margins  = new Drawing.Margins (12, 12, 20, 20);
		protected static Drawing.Margins	align_margins = new Drawing.Margins (8, 8, 6, 6);
	}
}
