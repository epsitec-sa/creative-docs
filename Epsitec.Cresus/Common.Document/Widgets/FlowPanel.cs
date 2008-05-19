using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// Conteneur qui place les widgets contenus comme des caractères dans un texte.
	/// </summary>
	public class FlowPanel : Widget
	{
		public FlowPanel() : base()
		{
		}

		public FlowPanel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public override Margins GetInternalPadding()
		{
			return new Drawing.Margins(3, 3, 3, 3);
		}

		protected override void MeasureMinMax(ref Size min, ref Size max)
		{
			Common.Widgets.Layouts.LayoutMeasure measureHeight = Common.Widgets.Layouts.LayoutMeasure.GetHeight(this);

			if (!measureHeight.SamePassIdAsLayoutContext(this))
			{
				//this.ResetColumnLineCount();
			}

			base.MeasureMinMax(ref min, ref max);

			double width = 0;
			double height = 0;
			double dy = 0;
			int column = 0;

			foreach (Widget child in this.Children)
			{
				if (column >= this.columns)
				{
					min.Width = System.Math.Max(min.Width, width);
					column = 0;
					width = 0;
					height += dy;
					dy = 0;
				}

				width += child.PreferredWidth + child.Margins.Left + child.Margins.Right;
				dy = System.Math.Max(dy, child.PreferredHeight + child.Margins.Top + child.Margins.Bottom);
				column++;
			}

			height += dy;

			min.Height = System.Math.Max(min.Height, height + this.Padding.Height + this.GetInternalPadding().Height);
		}

		protected override void ManualArrange()
		{
			base.ManualArrange();

			Drawing.Rectangle rect = this.Client.Bounds;

			rect.Deflate(this.Padding);
			rect.Deflate(this.GetInternalPadding());

			double x = 0;
			double y = 0;
			double dy = 0;

			int column = 0;

			foreach (Widget child in this.Children)
			{
				if (column >= this.columns)
				{
					column = 0;
					x = 0;
				}

				x += child.PreferredWidth + child.Margins.Left + child.Margins.Right;

				if (x > rect.Width && column > 0)
				{
					if (column < this.columns)
					{
						this.columns = column;
						this.lines = (this.Children.Count + this.columns - 1) / this.columns;

						Common.Widgets.Layouts.LayoutContext.AddToMeasureQueue(this);
						return;
					}
				}

				column++;
			}

			if (this.ActualWidth > this.lastWidth)
			{
				this.ResetColumnLineCount();

				this.lastWidth = this.ActualWidth;

				Common.Widgets.Layouts.LayoutContext.AddToMeasureQueue(this);
				return;
			}

			x = 0;
			y = 0;
			dy = 0;

			column = 0;

			foreach (Widget child in this.Children)
			{
				if (column >= this.columns)
				{
					column = 0;
					x = 0;
					y += dy;
					dy = 0;
				}

				child.SetManualBounds(new Drawing.Rectangle(rect.Left + x, rect.Top - y - child.PreferredHeight, child.PreferredWidth, child.PreferredHeight));

				dy = System.Math.Max(dy, child.PreferredHeight + child.Margins.Top + child.Margins.Bottom);
				x += child.PreferredWidth + child.Margins.Left + child.Margins.Right;
				column++;
			}

			this.lastWidth = this.ActualWidth;
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle rect  = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}

		protected override void OnChildrenChanged()
		{
			base.OnChildrenChanged();
			this.ResetColumnLineCount();
		}

		private void ResetColumnLineCount()
		{
			this.columns = this.Children.Count;
			this.lines = 1;
		}

		private double lastWidth;
		private int lines;
		private int columns;
	}
}
