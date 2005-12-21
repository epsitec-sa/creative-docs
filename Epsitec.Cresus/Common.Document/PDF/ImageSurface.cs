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
		public ImageSurface(string filename, Size size, bool filter, int id)
		{
			this.filename = filename;
			this.size = size;
			this.filter = filter;
			this.id = id;

			//	Crée une fois pour toutes le Drawing.Image associé à cette image.
			try
			{
				this.drawingImage = Bitmap.FromFile(this.filename);
			}
			catch
			{
				this.drawingImage = null;
			}
		}

		public void Dispose()
		{
			//	Libère la surface.
			this.drawingImage = null;
		}

		public string Filename
		{
			//	Nom de l'image.
			get { return this.filename; }
		}

		public Size Size
		{
			//	Dimensions de l'image.
			get { return this.size; }
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
			get { return this.drawingImage; }
		}


		public static ImageSurface Search(System.Collections.ArrayList list, string filename, Size size, bool filter)
		{
			//	Cherche une image d'après son nom dans une liste.
			foreach ( ImageSurface image in list )
			{
				if ( image.filename == filename &&
					 image.size     == size     &&
					 image.filter   == filter   )
				{
					return image;
				}
			}
			return null;
		}

		public static ImageSurface Search(System.Collections.ArrayList list, Drawing.Image drim)
		{
			//	Cherche une image d'après son Drawing.Image.
			foreach ( ImageSurface image in list )
			{
				if ( image.drawingImage == null )  continue;

				if ( image.drawingImage.UniqueId == drim.UniqueId )
				{
					return image;
				}
			}
			return null;
		}


		protected string					filename;
		protected Size						size;
		protected bool						filter;
		protected int						id;
		protected Drawing.Image				drawingImage;
	}
}
