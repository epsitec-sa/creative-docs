//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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


		public Drawing.Image Image
		{
			//	Image bitmap à afficher.
			get
			{
				return this.image;
			}

			set
			{
				this.image = value;
				this.Invalidate ();
			}
		}

		public ImageDisplayMode DisplayMode
		{
			//	Stretch les images en fonction de la place disponible.
			get
			{
				return this.displayMode;
			}

			set
			{
				if (this.displayMode != value)
				{
					this.displayMode = value;
					this.Invalidate ();
				}
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
				if (this.paintFrame != value)
				{
					this.paintFrame = value;
					this.Invalidate ();
				}
			}
		}

		public Drawing.Rectangle GetImageClientBounds()
		{
			Drawing.Rectangle rect = this.Client.Bounds;

			if (this.image != null)
			{
				double w = this.image.Width;
				double h = this.image.Height;

				if (this.displayMode == ImageDisplayMode.Stretch)
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
					rect = new Rectangle (rect.Center.X-w/2, rect.Center.Y-h/2, w, h);
				}
			}

			return rect;
		}

		protected override void OnIconUriChanged(string oldIconUri, string newIconUri)
		{
			base.OnIconUriChanged (oldIconUri, newIconUri);

			if (string.IsNullOrEmpty (oldIconUri) &&
				string.IsNullOrEmpty (newIconUri))
			{
				//	Nothing to do. Change is not significant : the text remains
				//	empty if we swap "" for null.
			}
			else
			{
				this.UpdateText (newIconUri);
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
					this.TextLayout.Paint (rect.BottomLeft, graphics);
				}
			}
			else  // image ?
			{
				double w = this.image.Width;
				double h = this.image.Height;

				ImageFilter oldFilter = graphics.ImageFilter;

				if (this.displayMode == ImageDisplayMode.Stretch)
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

					graphics.ImageFilter = new ImageFilter (ImageFilteringMode.ResamplingBicubic);
				}
				else
				{
					rect = new Rectangle (rect.Center.X-w/2, rect.Center.Y-h/2, w, h);
					graphics.ImageFilter = new ImageFilter (ImageFilteringMode.None);
				}

				rect = graphics.Align (rect);
				graphics.PaintImage (this.image, rect);
				graphics.ImageFilter = oldFilter;
			}

			if (this.paintFrame)
			{
				Drawing.Rectangle frame = rect;
				
				frame.Deflate (0.5);
				
				graphics.AddRectangle (frame);
				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (this.IsSelected)
			{
				Drawing.Rectangle frame = rect;

				frame.Deflate (0.5);
				graphics.AddRectangle (frame);
				frame.Deflate (1.0);
				graphics.AddRectangle (frame);
				
				graphics.RenderSolid (adorner.ColorCaption);

			}
		}

		private void UpdateText(string newIconUri)
		{
			if (string.IsNullOrEmpty (newIconUri))
			{
				this.Text = null;
			}
			else
			{
				this.Text = string.Format (@"<img src=""{0}""/>", newIconUri);
				this.ContentAlignment = ContentAlignment.MiddleCenter;
			}
		}

		private Drawing.Image						image;
		private bool								paintFrame;
		private ImageDisplayMode					displayMode;
	}
}

