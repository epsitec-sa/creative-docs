using Epsitec.Common.Drawing;
using System.Collections.Generic;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe ImageSurface enregistre les informations sur une image bitmap.
	/// Il existe une seule instance de ImageSurface par nom de fichier, si
	/// plusieurs Objects.Image utilisent la même image.
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
			//	Donne le Drawing.Image associé.
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
			//	Retourne true si l'image a pu être chargée au moins une fois avec succès.
			get
			{
				return this.cache.Size != Size.Zero;
			}
		}


		public static ImageSurface Search(IEnumerable<ImageSurface> list, string filename, Size size, Margins crop, ImageFilter filter)
		{
			//	Cherche une image d'après son nom dans une liste.
			foreach (ImageSurface image in list)
			{
				if ((image.cache.FileName == filename) &&
					(image.size           == size)     &&
					(image.crop           == crop)     &&
					(image.filter         == filter))
				{
					return image;
				}
			}

			return null;
		}

#if false
		public static ImageSurface Search(IEnumerable<ImageSurface> list, long uniqueId, Size size, Margins crop, ImageFilter filter)
		{
			//	Cherche une image d'après son Drawing.Image.
			foreach (ImageSurface image in list)
			{
				Drawing.Image di = image.cache.CachedImage;

				if (di == null)
				{
					continue;
				}

				if (di.UniqueId  == uniqueId &&
					 image.size   == size          &&
					 image.crop   == crop          &&
					 image.filter == filter)
				{
					return image;
				}
			}

			return null;
		}
#endif

		protected ImageCache.Item			cache;
		protected Size						size;
		protected Margins					crop;
		protected ImageFilter				filter;
		protected int						id;
		protected bool						exists;
	}
}
