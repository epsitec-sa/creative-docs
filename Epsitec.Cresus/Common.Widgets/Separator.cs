
[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Separator))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Separator permet de dessiner des séparations.
	/// </summary>
	public class Separator : Widget
	{
		public Separator()
		{
		}
		
		public Separator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public Drawing.Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.Invalidate();
				}
			}
		}

		public double Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				if (this.alpha != value)
				{
					this.alpha = value;
					this.Invalidate();
				}
			}
		}

		public bool IsHorizontalLine
		{
			//	Force un trait horizontal.
			//	Si IsHorizontalLine=false et IsVerticalLine=false, tout le rectangle est dessiné.
			get
			{
				return this.isHorizontalLine;
			}

			set
			{
				if (this.isHorizontalLine != value)
				{
					this.isHorizontalLine = value;
				}
			}
		}

		public bool IsVerticalLine
		{
			//	Force un trait vertical.
			//	Si IsHorizontalLine=false et IsVerticalLine=false, tout le rectangle est dessiné.
			get
			{
				return this.isVerticalLine;
			}

			set
			{
				if (this.isVerticalLine != value)
				{
					this.isVerticalLine = value;
				}
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (!this.IsEnabled || this.alpha == 0)  return;

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			double w = this.DrawFrameWidth;

			if (this.isHorizontalLine)
			{
				rect.Bottom = System.Math.Floor(rect.Center.Y)-(w-1)/2;
				rect.Height = w;
			}

			if (this.isVerticalLine)
			{
				rect.Left = System.Math.Floor(rect.Center.X)-(w-1)/2;
				rect.Width = w;
			}

			graphics.AddFilledRectangle(rect);

			Drawing.Color color = adorner.ColorBorder;
			if ( !this.color.IsEmpty )
			{
				color = this.color;
			}

			graphics.RenderSolid(Drawing.Color.FromAlphaColor(this.alpha, color));
		}


		protected Drawing.Color			color = Drawing.Color.Empty;
		protected double				alpha = 1.0;
		protected bool					isHorizontalLine = false;
		protected bool					isVerticalLine = false;
	}
}
