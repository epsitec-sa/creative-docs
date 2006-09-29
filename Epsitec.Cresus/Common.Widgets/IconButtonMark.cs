using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconButtonMark est un IconButton avec une marque triangulaire sur un côté.
	/// </summary>
	public class IconButtonMark : IconButton
	{
		public IconButtonMark()
		{
		}

		public IconButtonMark(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public ButtonMarkDisposition							SiteMark
		{
			//	Emplacement de la marque.
			get
			{
				return this.siteMark;
			}

			set
			{
				if ( this.siteMark != value )
				{
					this.siteMark = value;
					this.Invalidate();
				}
			}
		}

		public double							MarkDimension
		{
			//	Dimension de la marque.
			get
			{
				return this.markDimension;
			}

			set
			{
				if ( this.markDimension != value )
				{
					this.markDimension = value;
					this.Invalidate();
				}
			}
		}

		public Color							BulletColor
		{
			//	Couleur de la puce éventuelle (si différent de Color.Empty).
			get
			{
				return this.bulletColor;
			}

			set
			{
				if ( this.bulletColor != value )
				{
					this.bulletColor = value;
					this.Invalidate();
				}
			}
		}

		protected Rectangle						IconButtonBounds
		{
			//	Donne le rectangle à utiliser pour le bouton.
			get
			{
				Rectangle rect = this.Client.Bounds;

				switch ( this.siteMark )
				{
					case ButtonMarkDisposition.Below:
						rect.Bottom += this.markDimension;
						break;

					case ButtonMarkDisposition.Above:
						rect.Top -= this.markDimension;
						break;

					case ButtonMarkDisposition.Left:
						rect.Left += this.markDimension;
						break;

					case ButtonMarkDisposition.Right:
						rect.Right -= this.markDimension;
						break;
				}

				return rect;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle   rect  = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;

			bool enable = ((state & WidgetPaintState.Enabled) != 0);
			if ( !enable )
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}

			if ( this.ActiveState == ActiveState.Yes )  // dessine la marque triangulaire ?
			{
				Drawing.Path path = new Path();
				double middle;
				double factor = 1.0;

				switch ( this.siteMark )
				{
					case ButtonMarkDisposition.Below:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Bottom);
						path.LineTo(middle-this.markDimension*factor, rect.Bottom+this.markDimension);
						path.LineTo(middle+this.markDimension*factor, rect.Bottom+this.markDimension);
						break;

					case ButtonMarkDisposition.Above:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Top);
						path.LineTo(middle-this.markDimension*factor, rect.Top-this.markDimension);
						path.LineTo(middle+this.markDimension*factor, rect.Top-this.markDimension);
						break;

					case ButtonMarkDisposition.Left:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Left, middle);
						path.LineTo(rect.Left+this.markDimension, middle-this.markDimension*factor);
						path.LineTo(rect.Left+this.markDimension, middle+this.markDimension*factor);
						break;

					case ButtonMarkDisposition.Right:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Right, middle);
						path.LineTo(rect.Right-this.markDimension, middle-this.markDimension*factor);
						path.LineTo(rect.Right-this.markDimension, middle+this.markDimension*factor);
						break;
				}
				path.Close();

				graphics.Color = adorner.ColorTextFieldBorder(enable);
				graphics.PaintSurface(path);
			}
			
			rect = this.IconButtonBounds;
			state &= ~WidgetPaintState.Selected;
			adorner.PaintButtonBackground (graphics, rect, state, Direction.Down, this.ButtonStyle);

			if (!this.bulletColor.IsEmpty)
			{
				Rectangle r = rect;
				r.Deflate(3.5);
				r.Width = r.Height;

				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(this.bulletColor);

				graphics.AddRectangle(r);
				graphics.RenderSolid(adorner.ColorTextFieldBorder(enable));

				rect.Left += rect.Height;
			}

			if ( this.innerZoom != 1.0 )
			{
				double zoom = (this.innerZoom-1)/2+1;
				this.TextLayout.LayoutSize = rect.Size/this.innerZoom;
				Drawing.Transform transform = graphics.Transform;
				graphics.ScaleTransform(zoom, zoom, 0, -this.Client.Size.Height*zoom);
				adorner.PaintButtonTextLayout (graphics, rect.BottomLeft, this.TextLayout, state, this.ButtonStyle);
				graphics.Transform = transform;
			}
			else
			{
				this.TextLayout.LayoutSize = rect.Size;
				adorner.PaintButtonTextLayout (graphics, rect.BottomLeft, this.TextLayout, state, this.ButtonStyle);
			}
		}


		protected ButtonMarkDisposition				siteMark = ButtonMarkDisposition.Below;
		protected double				markDimension = 8;
		protected Color					bulletColor = Color.Empty;
	}
}
