using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Slider implémente un curseur de réglage.
	/// </summary>
	public sealed class Slider : AbstractSlider, Support.Data.INumValue
	{
		public Slider()
			: base (false, false)
		{
			this.range = new Types.DecimalRange (0, 100, 1);
		}

		public Slider(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public bool								HasFrame
		{
			get
			{
				return this.hasFrame;
			}
			set
			{
				this.hasFrame = value;
			}
		}

		public Drawing.Color					Color
		{
			get
			{
				return (Drawing.Color) this.GetValue (Slider.ColorProperty);
			}
			set
			{
				this.SetValue (Slider.ColorProperty, value);
			}
		}


		protected override Epsitec.Common.Widgets.Behaviors.DragBehavior CreateDragBehavior()
		{
			return new Behaviors.DragBehavior (this, true, true);
		}

		protected override Drawing.Point DragLocation
		{
			get
			{
				return new Drawing.Point (0, 0);
			}
		}

		protected override bool OnDragBegin(Drawing.Point cursor)
		{
			return true;
		}

		protected override void OnDragging(DragEventArgs e)
		{
			this.LogarithmicValue = this.Detect (e.ToPoint);
		}

		protected override void OnDragEnd()
		{
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState state = this.GetPaintState ();

			double width = rect.Width;
			rect.Left   += adorner.GeometrySliderLeftMargin;
			rect.Right  += adorner.GeometrySliderRightMargin;
			rect.Bottom += adorner.GeometrySliderBottomMargin;

			if (this.hasFrame)
			{
				adorner.PaintTextFieldBackground (graphics, rect, state, TextFieldStyle.Multiline, TextFieldDisplayMode.Default, false, false);
			}
			else
			{
				graphics.AddLine (1, rect.Top-0.5, width-1, rect.Top-0.5);
				graphics.RenderSolid (adorner.ColorTextSliderBorder (this.IsEnabled));
			}

			if (this.IsEnabled)
			{
				rect.Deflate (Slider.FrameMargin, Slider.FrameMargin);

				Drawing.Color front = this.Color.IsEmpty     ? adorner.ColorCaption : this.Color;
				Drawing.Color back  = this.BackColor.IsEmpty ? adorner.ColorWindow  : this.BackColor;

				if ((!this.hasFrame) &&
					(rect.Left > 1))
				{
					Drawing.Path path = new Drawing.Path ();
					path.MoveTo (1, rect.Top);
					path.LineTo (rect.Left, rect.Top);
					path.LineTo (rect.Left, rect.Bottom);
					path.CurveTo (1, rect.Bottom, 1, rect.Top);
					graphics.Rasterizer.AddSurface (path);
					graphics.RenderSolid (front);
				}

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (back);

				double range = (double) (this.Range);
				double delta = (double) (this.LogarithmicValue - this.MinValue);

				if ((delta > 0) &&
					(range > 0))
				{
					rect.Width *= delta / range;
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (front);
				}
			}
		}

		protected override Zone DetectZone(Epsitec.Common.Drawing.Point pos)
		{
			return Zone.Thumb;
		}

		private decimal Detect(Drawing.Point pos)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			double width = this.Client.Size.Width;
			double left = 0;

			width -= adorner.GeometrySliderLeftMargin;
			width -= adorner.GeometrySliderRightMargin;
			width -= Slider.FrameMargin;

			left  += adorner.GeometrySliderLeftMargin;
			left  += Slider.FrameMargin;

			double offset  = (double) (pos.X - left);
			double range   = (double) (this.Range);
			double minimum = (double) (this.MinValue);

			if (width > 0)
			{
				return this.range.Constrain (minimum + offset * range / width);
			}

			return this.MinValue;
		}

		private static readonly double			FrameMargin = 1;

		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register ("Color", typeof (Drawing.Color), typeof (Slider), new Helpers.VisualPropertyMetadata (Drawing.Color.Empty, Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		
		private bool							hasFrame = true;
	}
}
