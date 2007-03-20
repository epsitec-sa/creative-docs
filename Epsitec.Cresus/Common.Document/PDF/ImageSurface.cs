using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe ImageSurface enregistre les informations sur une image bitmap.
	/// Il existe une seule instance de ImageSurface par nom de fichier, si
	/// plusieurs Objects.Image utilisent la m�me image.
	/// </summary>
	public class ImageSurface
	{
		public ImageSurface(ImageCache.Item cache, Size size, Margins crop, ImageFilter filter, int id)
		{
			this.cache = cache;
			this.size = size;
			this.crop = crop;
			this.filter = filter;
			this.id = id;
		}

		public void Dispose()
		{
			//	Lib�re la surface.
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

		public ImageFilter Filter
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
			//	Donne le Drawing.Image associ�.
			get
			{
				Drawing.Image image = this.cache.Image;
				
				if (image != null)
				{
					this.exists = true;
				}

				return image;
			}
		}

		public bool Exists
		{
			//	Retourne true si l'image a pu �tre charg�e au moins une fois avec succ�s.
			get
			{
				return this.cache.Size != Size.Zero;
			}
		}


		public static ImageSurface Search(System.Collections.ArrayList list, string filename, Size size, Margins crop, ImageFilter filter)
		{
			//	Cherche une image d'apr�s son nom dans une liste.
			foreach ( ImageSurface image in list )
			{
				if ( image.cache.FileName == filename &&
					 image.size           == size     &&
					 image.crop           == crop     &&
					 image.filter         == filter   )
				{
					return image;
				}
			}
			return null;
		}

		public static ImageSurface Search(System.Collections.ArrayList list, Drawing.Image drim, Size size, Margins crop, ImageFilter filter)
		{
			//	Cherche une image d'apr�s son Drawing.Image.
			foreach ( ImageSurface image in list )
			{
				Drawing.Image di = image.cache.CachedImage;
				if ( di == null )  continue;

				if ( di.UniqueId  == drim.UniqueId &&
					 image.size   == size          &&
					 image.crop   == crop          &&
					 image.filter == filter        )
				{
					return image;
				}
			}
			return null;
		}


		protected ImageCache.Item			cache;
		protected Size						size;
		protected Margins					crop;
		protected ImageFilter				filter;
		protected int						id;
		protected bool						exists;
	}
}
