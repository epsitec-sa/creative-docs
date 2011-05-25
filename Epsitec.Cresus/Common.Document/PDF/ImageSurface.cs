
using Epsitec.Common.Drawing;
using System.Collections.Generic;

using Epsitec.Common.Drawing.Platform;
using System.Xml.Linq;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe ImageSurface enregistre les informations sur une image bitmap.
	/// Il existe une seule instance de ImageSurface par nom de fichier, si
	/// plusieurs Objects.Image utilisent la même image.
	/// </summary>
	public sealed class ImageSurface : System.IDisposable
	{
		private ImageSurface()
		{
		}

		public ImageSurface(ImageCache.Item cache, Size imageSize, Margins crop, ImageFilter filter, int id)
		{
			System.Diagnostics.Debug.Assert (cache != null);

			var size = cache == null ? Size.Zero : cache.Size;
			var file = cache == null ? null      : cache.GetImageFileReference ();
			var name = cache == null ? null      : cache.FileName;

			System.Diagnostics.Debug.Assert (file != null);

			this.imageName = name;
			this.imageSize = imageSize;
			this.crop      = crop;
			this.filter    = filter;
			this.id        = id;
			
			this.BitmapSize    = size;
			this.fileReference = file;
		}

		public Size BitmapSize
		{
			get;
			set;
		}

		public Size ImageSize
		{
			//	Dimensions de l'image.
			get
			{
				return this.imageSize;
			}
		}

		public string FilePath
		{
			get
			{
				return this.fileReference == null ? null : this.fileReference.Path;
			}
		}

		public Margins Crop
		{
			//	Marges de recadrage de l'image.
			get
			{
				return this.crop;
			}
		}

		public ImageFilter Filter
		{
			//	Filtre de l'image.
			get
			{
				return this.filter;
			}
		}

		public int Id
		{
			//	Identificateur unique.
			get
			{
				return this.id;
			}
		}

		public bool Exists
		{
			//	Retourne true si l'image a pu être chargée au moins une fois avec succès.
			get
			{
				return this.BitmapSize != Size.Zero;
			}
		}

		public double MinDpi
		{
			get;
			set;
		}

		public double MaxDpi
		{
			get;
			set;
		}

		public int DX
		{
			get;
			set;
		}

		public int DY
		{
			get;
			set;
		}

		public PdfImageStream ImageStream
		{
			get;
			set;
		}

		public ImageCompression ImageCompression
		{
			get;
			set;
		}

		public PdfComplexSurfaceType SurfaceType
		{
			get;
			set;
		}

		public BitmapColorType ColorType
		{
			get;
			set;
		}

		public ColorConversion ColorConversion
		{
			get;
			set;
		}

		public double JpegQuality
		{
			get;
			set;
		}

		public bool IsTransparent
		{
			get;
			set;
		}

		public void Dispose()
		{
			if (this.fileReference != null)
			{
				this.fileReference.Dispose ();
			}
			if (this.ImageStream != null)
			{
				this.ImageStream.Dispose ();
			}
		}


		public void Update(string source)
		{
			var xml = XDocument.Parse (source);
			var surface = xml.Element ("surface");

			this.BitmapSize    = Size.Parse ((string) surface.Attribute ("bitmapSize"));
			this.ColorType     = Epsitec.Common.Types.InvariantConverter.ToEnum<BitmapColorType> ((string) surface.Attribute ("colorType"));
		}

		public static string Serialize(ImageSurface item)
		{
			var xml = new XDocument (
				new XElement ("surface",
					new XAttribute ("imageName", item.imageName),
					new XAttribute ("imageSize", item.imageSize.ToString ()),
					new XAttribute ("bitmapSize", item.BitmapSize.ToString ()),
					new XAttribute ("colorType", item.ColorType.ToString ()),
					new XAttribute ("colorConv", item.ColorConversion.ToString ()),
					new XAttribute ("jpegQuality", item.JpegQuality),
					new XAttribute ("surfaceType", item.SurfaceType.ToString ()),
					new XAttribute ("compression", item.ImageCompression.ToString ()),
					new XAttribute ("crop", item.crop.ToString ()),
					new XAttribute ("filter", item.filter.ToString ()),
					new XAttribute ("id", item.id),
					new XAttribute ("file", item.fileReference.Path),
					new XAttribute ("minDPI", item.MinDpi),
					new XAttribute ("maxDPI", item.MaxDpi),
					new XAttribute ("dx", item.DX),
					new XAttribute ("dy", item.DY),
					new XAttribute ("transparent", item.IsTransparent),
					PdfImageStream.ToXml (item.ImageStream)
					));

			return xml.ToString ();
		}

		public static ImageSurface Deserialize(string source)
		{
			var xml = XDocument.Parse (source);
			var surface = xml.Element ("surface");

			var item = new ImageSurface ();

			item.imageName        = (string) surface.Attribute ("imageName");
			item.imageSize        = Size.Parse ((string) surface.Attribute ("imageSize"));
			item.BitmapSize       = Size.Parse ((string) surface.Attribute ("bitmapSize"));
			item.ColorType        = Epsitec.Common.Types.InvariantConverter.ToEnum<BitmapColorType> ((string) surface.Attribute ("colorType"));
			item.ColorConversion  = Epsitec.Common.Types.InvariantConverter.ToEnum<ColorConversion> ((string) surface.Attribute ("colorConv"));
			item.JpegQuality      = (double) surface.Attribute ("jpegQuality");
			item.SurfaceType      = Epsitec.Common.Types.InvariantConverter.ToEnum<PdfComplexSurfaceType> ((string) surface.Attribute ("surfaceType"));
			item.ImageCompression = Epsitec.Common.Types.InvariantConverter.ToEnum<ImageCompression> ((string) surface.Attribute ("compression"));
			item.crop             = Margins.Parse ((string) surface.Attribute ("crop"));
			item.filter           = ImageFilter.Parse ((string) surface.Attribute ("filter"));
			item.id               = (int) surface.Attribute ("id");
			item.fileReference    = new ImageFileReference ((string) surface.Attribute ("file"));
			item.MinDpi           = (double) surface.Attribute ("minDPI");
			item.MaxDpi           = (double) surface.Attribute ("maxDPI");
			item.DX               = (int) surface.Attribute ("dx");
			item.DY               = (int) surface.Attribute ("dy");
			item.IsTransparent    = (bool) surface.Attribute ("transparent");
			item.ImageStream      = PdfImageStream.FromXml (surface);

			return item;
		}


		public static ImageSurface Search(IEnumerable<ImageSurface> list, string filename, Size size, Margins crop, ImageFilter filter)
		{
			//	Cherche une image d'après son nom dans une liste.
			foreach (ImageSurface image in list)
			{
				if ((image.imageName == filename) &&
					(image.filter   == filter) &&
					(Size.Equal (image.imageSize, size, 0.001)) &&
					(Margins.Equal (image.crop, crop, 0.001)))
				{
					return image;
				}
			}

			return null;
		}

		private string						imageName;
		private Size						imageSize;
		private Margins						crop;
		private ImageFilter					filter;
		private int							id;
		private ImageFileReference			fileReference;
	}
}
