using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Slider implémente un curseur de réglage.
	/// </summary>
	public class Slider : AbstractSlider, Support.Data.INumValue, Behaviors.IDragBehaviorHost
	{
		public Slider() : base (false, false)
		{
			this.drag_behavior = new Behaviors.DragBehavior (this, true, true);
			this.range = new Types.DecimalRange (0, 100, 1);
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
				rect.Deflate(Slider.margin, Slider.margin);
				
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
			width -= Slider.margin;
			
			left  += adorner.GeometrySliderLeftMargin;
			left  += Slider.margin;
			
			double offset  = (double) (pos.X - left);
			double range   = (double) (this.Range);
			double minimum = (double) (this.MinValue);
			
			if (width > 0)
			{
				return this.range.Constrain (minimum + offset * range / width);
			}
			
			return this.MinValue;
		}
		
		protected static readonly double margin = 1;

		private Behaviors.DragBehavior		drag_behavior;
		private Drawing.Color				color = Drawing.Color.Empty;
		private bool						has_frame = true;
	}
}
