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


		public static void Attach()
		{
			Window.MessageFilter += new MessageHandler(Balloon.MessageFilter);
		}

		public static void Detach()
		{
			Window.MessageFilter -= new MessageHandler(Balloon.MessageFilter);
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

		// Position horizontale du point chaud, habituellement au milieu de la largeur.
		public double Hot
		{
			get
			{
				return this.hot;
			}
			set
			{
				this.hot = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);

			double m = this.Client.Bounds.Left+this.hot;
			double h = this.distance;
			double w = this.distance*0.3;

			Path path = new Path();
			path.MoveTo(m, rect.Bottom);
			path.LineTo(m-w, rect.Bottom+h);
			path.LineTo(rect.Left, rect.Bottom+h);
			path.LineTo(rect.Left, rect.Top);
			path.LineTo(rect.Right, rect.Top);
			path.LineTo(rect.Right, rect.Bottom+h);
			path.LineTo(m+w, rect.Bottom+h);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.backgroundColor);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.frameColor);
		}


		private static void MessageFilter(object sender, Message message)
		{
			if ( message.Type == MessageType.MouseMove )
			{
				Window window = sender as Window;
				Drawing.Rectangle rect = new Drawing.Rectangle(window.Root.Location, window.Root.Size);
				Point p1 = window.MapWindowToScreen(rect.BottomLeft);
				Point p2 = window.MapWindowToScreen(rect.TopRight);
				rect = new Rectangle(p1, p2);
				System.Diagnostics.Debug.WriteLine(string.Format("Filter: x={0} l={1} r={2}", message.X, rect.Left, rect.Right));
			}
		}


		protected double					distance;
		protected double					margin;
		protected double					hot;
		protected Color						backgroundColor;
		protected Color						frameColor;
	}
}
