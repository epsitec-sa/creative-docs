using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
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
			this.hot = 0;
			this.awayMargin = 5;

			this.Padding = new Drawing.Margins(this.margin, this.margin, this.margin, this.distance+this.margin);

			this.backgroundColor = Color.FromName("Info");
			this.frameColor = Color.FromBrightness(0);
		}

		public Balloon(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void Attach()
		{
			Window.MessageFilter += new MessageHandler(this.MessageFilter);
		}

		public void Detach()
		{
			Window.MessageFilter -= new MessageHandler(this.MessageFilter);
		}

		
		public double Distance
		{
			//	Hauteur de la queue.
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

		public double Margin
		{
			//	Marge autour des boutons contenus.
			get
			{
				return this.margin;
			}

			set
			{
				if ( this.margin != value )
				{
					this.margin = value;
					this.Padding = new Drawing.Margins (this.margin, this.margin, this.margin, this.distance+this.margin);
					this.Invalidate();
				}
			}
		}

		public double Hot
		{
			//	Position horizontale du point chaud (queue), habituellement au milieu de la largeur.
			get
			{
				return this.hot;
			}

			set
			{
				this.hot = value;
			}
		}

		public double AwayMargin
		{
			//	Distance d'extinction lorsque la souris s'éloigne.
			get
			{
				return this.awayMargin;
			}

			set
			{
				this.awayMargin = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la "bulle" de style bd.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);

			Path path;

			if ( double.IsNaN(this.hot) )
			{
				rect.Bottom += this.distance;
				path = Path.FromRectangle(rect);
			}
			else
			{
				double m = this.Client.Bounds.Left+this.hot;
				double h = this.distance;
				double w = this.distance*0.3;

				path = new Path();
				path.MoveTo(m, rect.Bottom);
				path.LineTo(m-w, rect.Bottom+h);
				path.LineTo(rect.Left, rect.Bottom+h);
				path.LineTo(rect.Left, rect.Top);
				path.LineTo(rect.Right, rect.Top);
				path.LineTo(rect.Right, rect.Bottom+h);
				path.LineTo(m+w, rect.Bottom+h);
				path.Close();
			}

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.backgroundColor);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.frameColor);
		}


		public bool IsAway(Point mouse)
		{
			//	Indique si la souris est trop loin de la mini-palette.
			Point p1 = this.MapClientToScreen(new Point(0, 0));
			Point p2 = this.MapClientToScreen(new Point(this.ActualWidth, this.ActualHeight));
			Rectangle rect = new Rectangle(p1, p2);

			double dx = System.Math.Abs(mouse.X-rect.Center.X);
			double dy = System.Math.Abs(mouse.Y-rect.Center.Y);

			if ( dx > dy*(rect.Width/rect.Height) )
			{
				return (dx-rect.Width/2 > this.awayMargin);
			}
			else
			{
				return (dy-rect.Height/2 > this.awayMargin);
			}
		}

		private void MessageFilter(object sender, Message message)
		{
			//	Appelé même lorsque la souris n'est plus sur le widget.
			if ( message.MessageType == MessageType.MouseMove )
			{
				Window window = sender as Window;
				
				Point mouse = window.MapWindowToScreen(message.Cursor);
				if ( this.IsAway(mouse) )
				{
					this.OnCloseNeeded();
				}
			}
		}


		protected virtual void OnCloseNeeded()
		{
			//	Génère un événement pour dire que la fermeture est nécessaire.
			var handler = this.GetUserEventHandler("CloseNeeded");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler CloseNeeded
		{
			add
			{
				this.AddUserEventHandler("CloseNeeded", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CloseNeeded", value);
			}
		}

		
		protected double					distance;
		protected double					margin;
		protected double					hot;
		protected double					awayMargin;
		protected Color						backgroundColor;
		protected Color						frameColor;
	}
}
