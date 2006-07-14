using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// La classe StackedPanel est la classe de base pour tous les panels empilés.
	/// </summary>
	public class StackedPanel : Widget
	{
		public StackedPanel()
		{
			this.Padding = new Margins(10, 10, 5, 10);

			this.label = new StaticText(this);
			this.label.Dock = DockStyle.Top;
			this.label.Margins = new Margins(0, 0, 0, 5);

			this.container = new Widget(this);
			this.container.Dock = DockStyle.Fill;

			this.Entered += new MessageEventHandler(this.HandleMouseEntered);
			this.Exited += new MessageEventHandler(this.HandleMouseExited);
		}
		
		public StackedPanel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Entered -= new MessageEventHandler(this.HandleMouseEntered);
				this.Exited -= new MessageEventHandler(this.HandleMouseExited);
			}
			
			base.Dispose(disposing);
		}


		public bool IsLeftPart
		{
			//	true  -> panneau de gauche pour la culture primaire
			//	false -> panneau de droite pour la culture secondaire
			get
			{
				return this.isLeftPart;
			}
			set
			{
				this.isLeftPart = value;
			}
		}

		public string Title
		{
			//	Texte du titre affiché en haut à gauche du panneau.
			get
			{
				return this.label.Text;
			}
			set
			{
				this.label.Text = value;
			}
		}

		public Color BackgroundColor
		{
			//	Couleur du fond du panneau.
			get
			{
				return this.backgroundColor;
			}
			set
			{
				if (this.backgroundColor != value)
				{
					this.backgroundColor = value;
					this.Invalidate();
				}
			}
		}

		public Widget Container
		{
			//	Donne le container à utiliser comme parent pour tous les enfants.
			get
			{
				return this.container;
			}
		}

		
		private void HandleMouseEntered(object sender, MessageEventArgs e)
		{
			//	La souris est entrée dans le panneau.
		}

		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			//	La souris est sortie du panneau.
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			if (!this.backgroundColor.IsEmpty)
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.backgroundColor);
			}

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);

			if (this.isLeftPart)
			{
				graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top+0.5);
			}

			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected Color						backgroundColor = Color.Empty;
		protected bool						isLeftPart = true;
		protected StaticText				label;
		protected Widget					container;
	}
}
