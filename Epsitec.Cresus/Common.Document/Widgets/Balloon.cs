using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe Balloon est un widget affichant une "bulle" style bd, avec la queue au milieu en bas.
	/// </summary>
	public class Balloon : Widget
	{
		public Balloon()
		{
			this.distance = 10;
			this.margin = 3;
			this.DockPadding = new Drawing.Margins(this.margin, this.margin, this.margin, this.distance+this.margin);

			this.backgroundColor = Color.FromName("Info");
			this.frameColor = Color.FromBrightness(0);
		}

		public Balloon(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		// Hauteur de la queue.
		public double Distance
		{
			get
			{
				return this.distance;
			}
			set
			{
				if ( this.distance != value )
				{
					this.distance = value;
					this.Invalidate();
				}
			}
		}

		// Marge autour des boutons contenus.
		public double Margin
		{
			get
			{
				return this.margin;
			}
			set
			{
				if ( this.margin != value )
				{
					this.margin = value;
					this.DockPadding = new Drawing.Margins(this.margin, this.margin, this.margin, this.distance+this.margin);
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);

			double h = this.distance;
			double w = this.distance*0.3;

			Path path = new Path();
			path.MoveTo(rect.Center.X, rect.Bottom);
			path.LineTo(rect.Center.X-w, rect.Bottom+h);
			path.LineTo(rect.Left, rect.Bottom+h);
			path.LineTo(rect.Left, rect.Top);
			path.LineTo(rect.Right, rect.Top);
			path.LineTo(rect.Right, rect.Bottom+h);
			path.LineTo(rect.Center.X+w, rect.Bottom+h);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.backgroundColor);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.frameColor);
		}


		protected double					distance;
		protected double					margin;
		protected Color						backgroundColor;
		protected Color						frameColor;
	}
}
