using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe ImageSurface enregistre les informations sur une image bitmap.
	/// Il existe une seule instance de ImageSurface par nom de fichier, si
	/// plusieurs Objects.Image utilisent la même image.
	/// </summary>
	public class ImageSurface
	{
		public ImageSurface(ImageCache.Item cache, Size size, Margins crop, bool filter, int id)
		{
			this.cache = cache;
			this.size = size;
			this.crop = crop;
			this.filter = filter;
			this.id = id;
		}

		public void Dispose()
		{
			//	Libère la surface.
			this.cache = null;
		}

		public ImageCache.Item Cache
		{
			//	Cache de l'image.
			get { return this.cache; }
		}

		public Size Size
		{
			//	Dimensions de l'image.
			get { return this.size; }
		}

		public Margins Crop
		{
			//	Marges de recadrage de l'image.
			get { return this.crop; }
		}

		public bool Filter
		{
			//	Filtre de l'image.
			get { return this.filter; }
		}

		public int Id
		{
			//	Identificateur unique.
			get { return this.id; }
		}

		public Drawing.Image DrawingImage
		{
			//	Donne le Drawing.Image associé.
			get { return this.cache.Image; }
		}


		public static ImageSurface Search(System.Collections.ArrayList list, string filename, Size size, Margins crop, bool filter)
		{
			//	Cherche une image d'après son nom dans une liste.
			foreach ( ImageSurface image in list )
			{
				if ( image.cache.Filename == filename &&
					 image.size           == size     &&
					 image.crop           == crop     &&
					 image.filter         == filter   )
				{
					return image;
				}
			}
			return null;
		}

		public static ImageSurface Search(System.Collections.ArrayList list, Drawing.Image drim, Size size, Margins crop)
		{
			//	Cherche une image d'après son Drawing.Image.
			foreach ( ImageSurface image in list )
			{
				if ( image.DrawingImage == null )  continue;

				if ( image.DrawingImage.UniqueId == drim.UniqueId &&
					 image.size                  == size          &&
					 image.crop                  == crop          )
				{
					return image;
				}
			}
			return null;
		}


		protected ImageCache.Item			cache;
		protected Size						size;
		protected Margins					crop;
		protected bool						filter;
		protected int						id;
	}
}
