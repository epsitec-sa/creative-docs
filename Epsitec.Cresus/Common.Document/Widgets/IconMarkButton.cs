using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	public enum SiteMark
	{
		OnBottom,		// marque en bas
		OnTop,			// marque en haut
		OnLeft,			// marque � gauche
		OnRight,		// marque � droite
	}


	/// <summary>
	/// La classe IconMarkButton est un IconButton avec une marque triangulaire sur un c�t�.
	/// </summary>
	public class IconMarkButton : IconButton
	{
		public IconMarkButton()
		{
		}

		public IconMarkButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public SiteMark							SiteMark
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

		protected Rectangle						IconButtonBounds
		{
			//	Donne le rectangle � utiliser pour le bouton.
			get
			{
				Rectangle rect = this.Client.Bounds;

				switch ( this.siteMark )
				{
					case SiteMark.OnBottom:
						rect.Bottom += this.markDimension;
						break;

					case SiteMark.OnTop:
						rect.Top -= this.markDimension;
						break;

					case SiteMark.OnLeft:
						rect.Left += this.markDimension;
						break;

					case SiteMark.OnRight:
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
			WidgetState state = this.PaintState;

			bool enable = ((state & WidgetState.Enabled) != 0);
			if ( !enable )
			{
				state &= ~WidgetState.Focused;
				state &= ~WidgetState.Entered;
				state &= ~WidgetState.Engaged;
			}

			if ( this.ActiveState == ActiveState.Yes )  // dessine la marque triangulaire ?
			{
				Drawing.Path path = new Path();
				double middle;

				switch ( this.siteMark )
				{
					case SiteMark.OnBottom:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Bottom);
						path.LineTo(middle-this.markDimension*0.75, rect.Bottom+this.markDimension);
						path.LineTo(middle+this.markDimension*0.75, rect.Bottom+this.markDimension);
						break;

					case SiteMark.OnTop:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Top);
						path.LineTo(middle-this.markDimension*0.75, rect.Top-this.markDimension);
						path.LineTo(middle+this.markDimension*0.75, rect.Top-this.markDimension);
						break;

					case SiteMark.OnLeft:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Left, middle);
						path.LineTo(rect.Left+this.markDimension, middle-this.markDimension*0.75);
						path.LineTo(rect.Left+this.markDimension, middle+this.markDimension*0.75);
						break;

					case SiteMark.OnRight:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Right, middle);
						path.LineTo(rect.Right-this.markDimension, middle-this.markDimension*0.75);
						path.LineTo(rect.Right-this.markDimension, middle+this.markDimension*0.75);
						break;
				}
				path.Close();

				graphics.Color = adorner.ColorTextFieldBorder(enable);
				graphics.PaintSurface(path);
			}
			
			rect = this.IconButtonBounds;
			state &= ~WidgetState.Selected;
			adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);

			if ( this.innerZoom != 1.0 )
			{
				double zoom = (this.innerZoom-1)/2+1;
				this.TextLayout.LayoutSize = rect.Size/this.innerZoom;
				Drawing.Transform transform = graphics.Transform;
				graphics.ScaleTransform(zoom, zoom, 0, -this.Client.Height*zoom);
				adorner.PaintButtonTextLayout(graphics, rect.BottomLeft, this.TextLayout, state, this.buttonStyle);
				graphics.Transform = transform;
			}
			else
			{
				this.TextLayout.LayoutSize = rect.Size;
				adorner.PaintButtonTextLayout(graphics, rect.BottomLeft, this.TextLayout, state, this.buttonStyle);
			}
		}


		protected SiteMark				siteMark = SiteMark.OnBottom;
		protected double				markDimension = 8;
	}
}
