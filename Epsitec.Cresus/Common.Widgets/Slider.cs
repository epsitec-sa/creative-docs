namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Slider implémente un curseur de réglage.
	/// </summary>
	public class Slider : Widget, Support.Data.INumValue, Behaviors.IDragBehaviorHost
	{
		public Slider()
		{
			this.drag_behavior = new Behaviors.DragBehavior (this, true, true);
		}
		
		public Slider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public bool							HasFrame
		{
			get { return this.has_frame; }
			set { this.has_frame = value; }
		}
		
		public Drawing.Color				Color
		{
			get
			{
				return this.color;
			}

			set
			{
				if ( this.color != value )
				{
					this.color = value;
					this.Invalidate();
				}
			}
		}
		
		
		#region INumValue Members
		public decimal						Value
		{
			get
			{
				return this.value;
			}
			set
			{
				value = this.range.Constrain (value);
				
				if (this.value != value)
				{
					this.value = value;
					this.OnValueChanged ();
					this.Invalidate ();
				}
			}
		}

		public decimal						MinValue
		{
			get
			{
				return this.range.Minimum;
			}
			set
			{
				this.range.Minimum = value;
			}
		}

		public decimal						MaxValue
		{
			get
			{
				return this.range.Maximum;
			}
			set
			{
				this.range.Maximum = value;
			}
		}

		public decimal						Resolution
		{
			get
			{
				return this.range.Resolution;
			}
			set
			{
				this.range.Resolution = value;
			}
		}
		
		public decimal						Range
		{
			get
			{
				return this.MaxValue - this.MinValue;
			}
		}
		
		
		public event Support.EventHandler	ValueChanged;
		#endregion

		public decimal						Logarithmic
		{
			get
			{
				return this.logarithmic;
			}
			set
			{
				this.logarithmic = value;
			}
		}
		
		public decimal						LogarithmicValue
		{
			get
			{
				if ( this.MaxValue == this.MinValue )
				{
					return this.Value;
				}
				else
				{
					decimal norm = (this.Value-this.MinValue)/(this.MaxValue-this.MinValue);
					norm = (decimal)System.Math.Pow((double)norm, (double)(1.0M/this.logarithmic));
					return norm*(this.MaxValue-this.MinValue)+this.MinValue;
				}
			}
			set
			{
				if ( this.MaxValue != this.MinValue )
				{
					decimal norm = (value-this.MinValue)/(this.MaxValue-this.MinValue);
					norm = (decimal)System.Math.Pow((double)norm, (double)this.logarithmic);
					value = norm*(this.MaxValue-this.MinValue)+this.MinValue;
				}
				this.Value = value;
			}
		}
		
		#region IDragBehaviorHost Members
		Drawing.Point						Behaviors.IDragBehaviorHost.DragLocation
		{
			get
			{
				return new Drawing.Point (0, 0);
			}
		}

		
		bool Behaviors.IDragBehaviorHost.OnDragBegin(Drawing.Point cursor)
		{
			return true;
		}

		void Behaviors.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.LogarithmicValue = this.Detect (e.ToPoint);
		}

		void Behaviors.IDragBehaviorHost.OnDragEnd()
		{
		}
		#endregion
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;

			double width = rect.Width;
			rect.Left   += adorner.GeometrySliderLeftMargin;
			rect.Right  += adorner.GeometrySliderRightMargin;
			rect.Bottom += adorner.GeometrySliderBottomMargin;
			
			if (this.has_frame)
			{
				adorner.PaintTextFieldBackground(graphics, rect, state, TextFieldStyle.Multi, TextDisplayMode.Default, false);
			}
			else
			{
				graphics.AddLine(1, rect.Top-0.5, width-1, rect.Top-0.5);
				graphics.RenderSolid(adorner.ColorTextSliderBorder(this.IsEnabled));
			}

			if (this.IsEnabled)
			{
				rect.Deflate(this.margin, this.margin);
				
				Drawing.Color front = this.color.IsEmpty     ? adorner.ColorCaption : this.color;
				Drawing.Color back  = this.BackColor.IsEmpty ? adorner.ColorWindow  : this.BackColor;
				
				if ((!this.has_frame) &&
					(rect.Left > 1))
				{
					Drawing.Path path = new Drawing.Path();
					path.MoveTo(1, rect.Top);
					path.LineTo(rect.Left, rect.Top);
					path.LineTo(rect.Left, rect.Bottom);
					path.CurveTo(1, rect.Bottom, 1, rect.Top);
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(front);
				}
				
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(back);
				
				double range = (double) (this.Range);
				double delta = (double) (this.LogarithmicValue - this.MinValue);
				
				if ((delta > 0) &&
					(range > 0))
				{
					rect.Width *= delta / range;
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(front);
				}
			}
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (! this.drag_behavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}
		
		
		protected virtual decimal Detect(Drawing.Point pos)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			double width = this.Client.Size.Width;
			double left = 0;
			
			width -= adorner.GeometrySliderLeftMargin;
			width -= adorner.GeometrySliderRightMargin;
			width -= this.margin;
			
			left  += adorner.GeometrySliderLeftMargin;
			left  += this.margin;
			
			double offset  = (double) (pos.X - left);
			double range   = (double) (this.Range);
			double minimum = (double) (this.MinValue);
			
			if (width > 0)
			{
				return this.range.Constrain (minimum + offset * range / width);
			}
			
			return this.MinValue;
		}

		protected virtual void OnValueChanged()
		{
			if ( this.ValueChanged != null )  // qq'un écoute ?
			{
				this.ValueChanged(this);
			}
		}

		
		private Behaviors.DragBehavior		drag_behavior;
		private decimal						value = 0;
		private Types.DecimalRange			range = new Types.DecimalRange (0, 100, 1);
		private decimal						logarithmic = 1;
		private Drawing.Color				color = Drawing.Color.Empty;
		protected readonly double			margin = 1;
		private bool						has_frame = true;
	}
}
