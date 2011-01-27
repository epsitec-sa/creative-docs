using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Properties
{
	public class CropLogic
	{
		public CropLogic(Document document, Objects.Image image)
		{
			this.document = document;
			this.image = image;
		}

		public void UpdateImge()
		{
			this.imageSize = new Size (1000, 1000);

			var pi = this.image.PropertyImage;
			if (pi != null)
			{
				ImageCache.Item item = this.document.ImageCache.Find (pi.FileName, pi.FileDate);
				if (item != null)
				{
					this.imageSize = item.Size;
				}
			}

			this.objectSize = this.image.ImageBitmapMaxFill;
		}


		public double RelativeZoom
		{
			//	Zoom relatif compris entre 0 et 1.
			get
			{
				var imageSize = this.imageSize;

				double dx = imageSize.Width  - this.CropMargins.Width;
				double dy = imageSize.Height - this.CropMargins.Height;

				double zoomx = dx > 0 ? imageSize.Width/dx : 1;
				double zoomy = dy > 0 ? imageSize.Height/dy : 1;

				double zoom = System.Math.Min (zoomx, zoomy);

				zoom = (zoom-1)/3;  // [1..4] -> [0..1]
				zoom = System.Math.Max (0.0, zoom);
				zoom = System.Math.Min (1.0, zoom);

				return zoom;
			}
			set
			{
				//?this.SetCropMargins (value, this.RelativePosition, keepRatio: false);
				this.SetCropZoom (value);
			}
		}

		public Point RelativePosition
		{
			//	Position relative comprise entre -1 et 1.
			get
			{
				var rect = new Rectangle (Point.Zero, this.imageSize);
				var c = rect.Center;

				var used = rect;
				used.Deflate (this.CropMargins);
				var u = used.Center;

				return new Point ((c.X-u.X)/(rect.Width/2), (c.Y-u.Y)/(rect.Height/2));
			}
			set
			{
				this.SetCropMargins (this.RelativeZoom, value, keepRatio: true);
			}
		}

		private void SetCropZoom(double zoom)
		{
			zoom = 1+(zoom*3);  // [0..1] -> [1..4]

			Size size = this.imageSize;  // taille de l'image sélectionnée
			Margins crop = this.CropMargins;  // crop avant de commencer à zoomer

			Rectangle rect = new Rectangle (Point.Zero, size);
			rect.Deflate (crop);  // rectangle effectif actuel

			double zoomx = size.Width/rect.Width;  // zoom horizontal actuel
			double zoomy = size.Height/rect.Height;  // zoom vertical actuel

			double w, h;

			if (zoomx < zoomy)
			{
				w = size.Width/zoom;  // nouvelle largeur
				h = w*rect.Height/rect.Width;  // garde les mêmes proportions
			}
			else
			{
				h = size.Height/zoom;  // nouvelle hauteur
				w = h*rect.Width/rect.Height;  // garde les mêmes proportions
			}

			Margins newCrop = Margins.Zero;
			newCrop.Left   = rect.Center.X-w/2;
			newCrop.Right  = size.Width-(rect.Center.X+w/2);
			newCrop.Bottom = rect.Center.Y-h/2;
			newCrop.Top    = size.Height-(rect.Center.Y+h/2);
			newCrop = CropLogic.CropAdjust (newCrop);

			this.CropMargins = newCrop;
		}

		private void SetCropMargins(double relativeZoom, Point relativePosition, bool keepRatio)
		{
			relativeZoom = System.Math.Max (0.0, relativeZoom);
			relativeZoom = System.Math.Min (1.0, relativeZoom);
			double zoom = 1+(relativeZoom*3);  // [0..1] -> [1..4]

			double dx = this.imageSize.Width  / zoom;
			double dy = this.imageSize.Height / zoom;

			if (keepRatio)
			{
				var initialCrop = this.CropMargins;
				double initialDx = this.imageSize.Width - initialCrop.Left - initialCrop.Right;
				double initialDy = this.imageSize.Height - initialCrop.Bottom - initialCrop.Top;

				dx = System.Math.Min (dx, initialDx);
				dy = System.Math.Min (dy, initialDy);
			}

			double cx = this.imageSize.Width  * (-relativePosition.X+1)/2;
			double cy = this.imageSize.Height * (-relativePosition.Y+1)/2;

			double ml = cx-dx/2;
			double mr = this.imageSize.Width - (cx+dx/2);
			double mb = cy-dy/2;
			double mt = this.imageSize.Height - (cy+dy/2);

			var crop = new Margins (ml, mr, mt, mb);
			this.CropMargins = CropLogic.CropAdjust (crop);
		}


		private Margins CropMargins
		{
			get
			{
				var pi = this.image.PropertyImage;
				return pi.CropMargins;
			}
			set
			{
				var pi = this.image.PropertyImage;
				pi.CropMargins = value;
			}
		}


		private static Margins CropAdjust(Margins crop)
		{
			//	Ajuste le recadrage pour n'avoir jamais de marges négatives.
			if (crop.Left < 0)
			{
				crop.Right += crop.Left;
				crop.Left = 0;
			}

			if (crop.Right < 0)
			{
				crop.Left += crop.Right;
				crop.Right = 0;
			}

			if (crop.Bottom < 0)
			{
				crop.Top += crop.Bottom;
				crop.Bottom = 0;
			}

			if (crop.Top < 0)
			{
				crop.Bottom += crop.Top;
				crop.Top = 0;
			}

			return crop;
		}


		private readonly Document			document;
		private readonly Objects.Image		image;

		private Size						imageSize;
		private Size						objectSize;
	}
}
