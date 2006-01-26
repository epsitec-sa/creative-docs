using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextSample est un widget affichant un échantillon d'une propriété de texte.
	/// </summary>
	public class TextSample : AbstractSample
	{
		public TextSample() : base()
		{
		}

		public TextSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public Text.TextStyle TextStyle
		{
			//	Style représenté.
			get
			{
				return this.textStyle;
			}

			set
			{
				if ( this.textStyle != value )
				{
					this.textStyle = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine l'échantillon.
			if ( this.document == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if ( this.textStyle == null )  // dessine une croix ?
			{
				rect.Deflate(0.5);
				Color color = adorner.ColorBorder;
				color.A = 0.3;

				graphics.AddLine(rect.BottomLeft, rect.TopRight);
				graphics.RenderSolid(color);

				graphics.AddLine(rect.TopLeft, rect.BottomRight);
				graphics.RenderSolid(color);
			}
			else
			{
				this.PaintSample(graphics, rect);
			}
		}

		protected void PaintSample(Graphics graphics, Drawing.Rectangle rect)
		{
			if ( rect.IsEmpty )  return;

			Drawing.Rectangle iClip = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(this.MapClientToRoot(rect));

			rect.Deflate(rect.Height*0.05);
			rect.Bottom -= rect.Height*10;  // hauteur presque infinie

			double scale = 1.0/10.0;
			Transform initial = graphics.Transform;
			graphics.ScaleTransform(scale, scale, 0.0, 0.0);
			rect.Scale(1.0/scale);

			Document document = this.document.DocumentForSamples;
			document.Modifier.OpletQueueEnable = false;

			string latin = "";

			if ( this.textStyle.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
			{
				latin = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
			}

			if ( this.textStyle.TextStyleClass == Common.Text.TextStyleClass.Text )
			{
				latin = "Lorem ipsum dolor";
			}

			Objects.TextBox2 obj = new Objects.TextBox2(document, null);
			obj.CreateFromRectangle(rect);
			obj.EditInsertText(latin, this.textStyle);

			Shape[] shapes = obj.ShapesBuild(graphics, null, false);

			Drawer drawer = new Drawer(document);
			drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.All, shapes);

			obj.Dispose();
			graphics.Transform = initial;
			graphics.RestoreClippingRectangle(iClip);
		}


		protected Text.TextStyle			textStyle;
	}
}
