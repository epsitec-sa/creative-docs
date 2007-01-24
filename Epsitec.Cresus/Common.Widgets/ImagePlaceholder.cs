//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ImagePlaceholder</c> class displays a bitmap image as a plain
	/// widget.
	/// </summary>
	public class ImagePlaceholder : Widget
	{
		public ImagePlaceholder()
		{
		}
		
		public ImagePlaceholder(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public Drawing.Image DrawingImage
		{
			//	Image bitmap � afficher.
			get
			{
				return this.image;
			}

			set
			{
				this.image = value;
			}
		}

		public string FixIcon
		{
			//	Ic�ne fixe �ventuelle, � la place d'une image.
			get
			{
				return this.fixIcon;
			}

			set
			{
				if (this.fixIcon != value)
				{
					this.fixIcon = value;

					if (this.fixIcon == null)
					{
						this.textLayout = null;
					}
					else
					{
						this.textLayout = new TextLayout();
						this.textLayout.Alignment = ContentAlignment.MiddleCenter;
						this.textLayout.Text = string.Format(@"<img src=""{0}""/>", this.fixIcon);
					}
				}
			}
		}

		public bool StretchImage
		{
			//	Stretch les images en fonction de la place disponible.
			get
			{
				return this.stretchImage;
			}

			set
			{
				this.stretchImage = value;
			}
		}

		public bool PaintFrame
		{
			//	Cadre �ventuel.
			get
			{
				return this.paintFrame;
			}

			set
			{
				this.paintFrame = value;
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine l'image.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if (this.image == null)  // ic�ne fixe ?
			{
				if (this.textLayout != null)
				{
					this.textLayout.LayoutSize = rect.Size;
					this.textLayout.Paint(rect.BottomLeft, graphics);
				}
			}
			else  // image ?
			{
				double w = this.image.Width;
				double h = this.image.Height;

				if (this.stretchImage)
				{
					if (rect.Width/rect.Height < w/h)
					{
						double hh = rect.Height - rect.Width*h/w;
						rect.Bottom += hh/2;
						rect.Top    -= hh/2;
					}
					else
					{
						double ww = rect.Width - rect.Height*w/h;
						rect.Left  += ww/2;
						rect.Right -= ww/2;
					}
				}
				else
				{
					rect = new Rectangle(rect.Center.X-w/2, rect.Center.Y-h/2, w, h);
				}

				graphics.Align(ref rect);
				graphics.PaintImage(this.image, rect);
			}

			if (this.paintFrame)
			{
				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}

		protected Drawing.Image						image;
		protected string							fixIcon;
		protected TextLayout						textLayout;
		protected bool								paintFrame = false;
		protected bool								stretchImage = false;
	}
}

