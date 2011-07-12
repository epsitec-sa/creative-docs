//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

[assembly: DependencyClass (typeof(FrameBox))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>FrameBox</c> class is a very simple group which knows how to
	/// paint a thin frame around itself.
	/// </summary>
	public class FrameBox : AbstractGroup
	{
		public FrameBox()
		{
		}

		public FrameBox(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public bool DrawFullFrame
		{
			get
			{
				return (bool) this.GetValue (FrameBox.DrawFullFrameProperty);
			}
			set
			{
				this.SetValue (FrameBox.DrawFullFrameProperty, value);
			}
		}

		public Drawing.Size BestFitSize
		{
			get
			{
				return (Drawing.Size) this.GetValue (FrameBox.BestFitSizeProperty);
			}
			set
			{
				if (value == Drawing.Size.Empty)
				{
					this.ClearValue (FrameBox.BestFitSizeProperty);
				}
				else
				{
					this.SetValue (FrameBox.BestFitSizeProperty, value);
				}
			}
		}


		public override Drawing.Margins GetShapeMargins()
		{
			//	Si le cadre est dessiné avec une épaisseur qui dépasse un pixel, il faut augmenter
			//	la surface à redessiner pour ce widget.
			double width = this.DrawFrameWidth;

			if (width <= 1 || this.DrawFrameEdges == FrameEdges.None)
			{
				return base.GetShapeMargins ();
			}
			else
			{
				double m = (width-1)/2;
				return new Margins (m, m, m, m);
			}
		}

		protected virtual Rectangle GetFrameRectangle()
		{
			return Rectangle.Deflate (this.Client.Bounds, 0.5, 0.5);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.BackColor);
			}
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Rectangle rect = this.GetFrameRectangle ();

			if (this.DrawFullFrame)
			{
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (this.DrawDesignerFrame)
			{
				using (Path path = new Path (rect))
				{
					graphics.PaintDashedOutline (path, 1, 4, 4, CapStyle.Square, adorner.ColorBorder);
				}
			}

			if (this.DrawFrameEdges != FrameEdges.None)
			{
				using (Path path = new Path ())
				{
					if ((this.DrawFrameEdges & FrameEdges.Left) != 0)
					{
						path.MoveTo(rect.BottomLeft);
						path.LineTo(rect.TopLeft);
					}

					if ((this.DrawFrameEdges & FrameEdges.Right) != 0)
					{
						path.MoveTo(rect.BottomRight);
						path.LineTo(rect.TopRight);
					}

					if ((this.DrawFrameEdges & FrameEdges.Top) != 0)
					{
						path.MoveTo(rect.TopLeft);
						path.LineTo(rect.TopRight);
					}

					if ((this.DrawFrameEdges & FrameEdges.Bottom) != 0)
					{
						path.MoveTo(rect.BottomLeft);
						path.LineTo(rect.BottomRight);
					}

					graphics.Rasterizer.AddOutline (path, this.DrawFrameWidth, CapStyle.Square, JoinStyle.Round, 5.0);
					graphics.RenderSolid (adorner.ColorBorder);
				}
			}
		}

		public override Size GetBestFitSize()
		{
			if (this.ContainsValue (FrameBox.BestFitSizeProperty))
			{
				return this.BestFitSize;
			}
			else
			{
				return base.GetBestFitSize ();
			}
		}

		public static readonly DependencyProperty DrawFullFrameProperty = DependencyProperty.Register ("DrawFullFrame", typeof (bool), typeof (FrameBox), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty BestFitSizeProperty = DependencyProperty.Register ("BestFitSize", typeof (Drawing.Size), typeof (FrameBox), new VisualPropertyMetadata (Drawing.Size.Empty));
	}
}
