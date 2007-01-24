//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			//	Image bitmap à afficher.
			get
			{
				return this.image;
			}

			set
			{
				this.image = value;
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
			//	Cadre éventuel.
			get
			{
				return this.paintFrame;
			}

			set
			{
				this.paintFrame = value;
			}
		}

		protected override void OnIconNameChanged(string oldIconName, string newIconName)
		{
			base.OnIconNameChanged (oldIconName, newIconName);

			if (string.IsNullOrEmpty (oldIconName) &&
				string.IsNullOrEmpty (newIconName))
			{
				//	Nothing to do. Change is not significant : the text remains
				//	empty if we swap "" for null.
			}
			else
			{
				this.UpdateText (newIconName);
			}
		}

		private void UpdateText(string newIconName)
		{
			if (string.IsNullOrEmpty (newIconName))
			{
				this.Text = null;
			}
			else
			{
				this.Text = string.Format (@"<img src=""{0}""/>", newIconName);
				this.ContentAlignment = ContentAlignment.MiddleCenter;
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine l'image.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if (this.image == null)  // icône fixe ?
			{
				if (!string.IsNullOrEmpty (this.Text))
				{
//					this.textLayout.LayoutSize = rect.Size;
					this.TextLayout.Paint(rect.BottomLeft, graphics);
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
		protected bool								paintFrame = false;
		protected bool								stretchImage = false;
	}
}

