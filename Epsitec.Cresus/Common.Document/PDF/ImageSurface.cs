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
		public ImageSurface(string filename, double width, double height, bool filter, int id)
		{
			this.filename = filename;
			this.width = width;
			this.height = height;
			this.filter = filter;
			this.id = id;

			// Crée une fois pour toutes le Drawing.Image associé à cette image.
			try
			{
				this.drawingImage = Bitmap.FromFile(this.filename);
			}
			catch
			{
				this.drawingImage = null;
			}
		}

		// Libère la surface.
		public void Dispose()
		{
			this.drawingImage = null;
		}

		// Nom de l'image.
		public string Filename
		{
			get { return this.filename; }
		}

		// Largeur de l'image.
		public double Width
		{
			get { return this.width; }
		}

		// Hauteur de l'image.
		public double Height
		{
			get { return this.height; }
		}

		// Filtre de l'image.
		public bool Filter
		{
			get { return this.filter; }
		}

		// Identificateur unique.
		public int Id
		{
			get { return this.id; }
		}

		// Donne le Drawing.Image associé.
		public Drawing.Image DrawingImage
		{
			get { return this.drawingImage; }
		}


		// Cherche une image d'après son nom dans une liste.
		public static ImageSurface Search(System.Collections.ArrayList list, string filename, double width, double height, bool filter)
		{
			foreach ( ImageSurface image in list )
			{
				if ( image.filename == filename &&
					 image.width    == width    &&
					 image.height   == height   &&
					 image.filter   == filter   )
				{
					return image;
				}
			}
			return null;
		}

		// Cherche une image d'après son Drawing.Image.
		public static ImageSurface Search(System.Collections.ArrayList list, Drawing.Image drim)
		{
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
		protected double					width;
		protected double					height;
		protected bool						filter;
		protected int						id;
		protected Drawing.Image				drawingImage;
	}
}
