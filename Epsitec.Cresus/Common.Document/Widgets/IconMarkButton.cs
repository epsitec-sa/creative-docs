using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	public enum SiteMark
	{
		OnBottom,		// marque en bas
		OnTop,			// marque en haut
		OnLeft,			// marque à gauche
		OnRight,		// marque à droite
	}


	/// <summary>
	/// La classe IconMarkButton est un IconButton avec une marque 'v' en bas.
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
				this.siteMark = value;
			}
		}

		public double							MarkSpace
		{
			//	Espacement de la marque.
			get
			{
				return this.markSpace;
			}

			set
			{
				this.markSpace = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point(0, 0);
			Drawing.Path      path  = new Path();
			double            middle;

			switch ( this.siteMark )
			{
				case SiteMark.OnBottom:
					middle = (rect.Left+rect.Right)/2;
					path.MoveTo(middle, rect.Bottom);
					path.LineTo(middle-this.markSpace*0.75, rect.Bottom+this.markSpace);
					path.LineTo(middle+this.markSpace*0.75, rect.Bottom+this.markSpace);
					path.Close();

					rect.Bottom += this.markSpace;
					pos.Y += this.markSpace/2;
					break;

				case SiteMark.OnTop:
					middle = (rect.Left+rect.Right)/2;
					path.MoveTo(middle, rect.Top);
					path.LineTo(middle-this.markSpace*0.75, rect.Top-this.markSpace);
					path.LineTo(middle+this.markSpace*0.75, rect.Top-this.markSpace);
					path.Close();

					rect.Top -= this.markSpace;
					break;

				case SiteMark.OnLeft:
					middle = (rect.Bottom+rect.Top)/2;
					path.MoveTo(rect.Left, middle);
					path.LineTo(rect.Left+this.markSpace, middle-this.markSpace*0.75);
					path.LineTo(rect.Left+this.markSpace, middle+this.markSpace*0.75);
					path.Close();

					rect.Left += this.markSpace;
					pos.X += this.markSpace/2;
					break;

				case SiteMark.OnRight:
					middle = (rect.Bottom+rect.Top)/2;
					path.MoveTo(rect.Right, middle);
					path.LineTo(rect.Right-this.markSpace, middle-this.markSpace*0.75);
					path.LineTo(rect.Right-this.markSpace, middle+this.markSpace*0.75);
					path.Close();

					rect.Right -= this.markSpace;
					break;
			}

			bool enable = ((state & WidgetState.Enabled) != 0);
			if ( !enable )
			{
				state &= ~WidgetState.Focused;
				state &= ~WidgetState.Entered;
				state &= ~WidgetState.Engaged;
			}

			if ( this.ActiveState == ActiveState.Yes )  // dessine la marque 'v' ?
			{
				graphics.Color = adorner.ColorTextFieldBorder(enable);
				graphics.PaintSurface(path);
			}
			
			state &= ~WidgetState.Selected;
			adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);

			if ( this.innerZoom != 1.0 )
			{
				double zoom = (this.innerZoom-1)/2+1;
				this.TextLayout.LayoutSize = this.Client.Size/this.innerZoom;
				Drawing.Transform transform = graphics.Transform;
				graphics.ScaleTransform(zoom, zoom, 0, -this.Client.Height*zoom);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
				graphics.Transform = transform;
			}
			else
			{
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
			}
		}


		protected SiteMark				siteMark = SiteMark.OnBottom;
		protected double				markSpace = 8;
	}
}
