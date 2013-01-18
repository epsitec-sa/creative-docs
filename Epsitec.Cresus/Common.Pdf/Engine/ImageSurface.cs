//	Copyright © 2004-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using System.Collections.Generic;
using Epsitec.Common.Drawing.Platform;
using System.Xml.Linq;

namespace Epsitec.Common.Pdf.Engine
{
	/// <summary>
	/// La classe ImageSurface enregistre les informations sur une image bitmap.
	/// Il existe une seule instance de ImageSurface par nom de fichier, si
	/// plusieurs Objects.Image utilisent la même image.
	/// </summary>
	public sealed class ImageSurface : System.IDisposable
	{
		public ImageSurface(long uniqueId, int id, Image image)
		{
			this.uniqueId = uniqueId;
			this.id       = id;
			this.image    = image;

			this.nativeBitmap = NativeBitmap.Create (this.image.BitmapImage.NativeBitmap);
		}

		public Image Image
		{
			get
			{
				return this.image;
			}
		}

		public NativeBitmap NativeBitmap
		{
			get
			{
				return this.nativeBitmap;
			}
		}

		public long UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public Size BitmapSize
		{
			get
			{
				return this.bitmapSize;
			}
		}

		public Size ImageSize
		{
			//	Dimensions de l'image.
			get
			{
				return this.imageSize;
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

#if false
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
#endif

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

		public void Dispose()
		{
			if (this.ImageStream != null)
			{
				this.ImageStream.Dispose ();
			}
		}


		public static long GetUniqueId(Image image)
		{
			//	Retourne un identificateur unique basé sur les données de l'image.
			//	Deux objets Image différents ayant les mêmes contenus doivent rendre
			//	le même identificateur.
			//	TODO: C'est une implémentation bricolée, il faudra faire mieux !!!
			var bytes = image.BitmapImage.GetRawBitmapBytes ();
			long sum = 0;
			int i = 0;

			foreach (var b in bytes)
			{
				sum += b;
				sum ^= ImageSurface.randomXor[(i++) % ImageSurface.randomXor.Length];
			}

			return sum;
		}

		private static long[] randomXor =
		{
			0x12345678,
			0xa340bbcf,
			0x029d4588,
			0x33333333,
			0x51860178,
			0xff505620,
			0x55dac972,
			0x224411dd,
			0x5079ee40,
			0x10030f00,
			0x9445b4c2,
			0x30405588,
			0x09aaffc3,
		};


		public static ImageSurface Search(IEnumerable<ImageSurface> list, long uniqueId)
		{
			//	Cherche une image dans une liste.
			foreach (ImageSurface image in list)
			{
				if (image.uniqueId == uniqueId)
				{
					return image;
				}
			}

			return null;
		}


		private readonly long			uniqueId;
		private readonly Image			image;
		private readonly NativeBitmap	nativeBitmap;
		private readonly Size			imageSize;
		private readonly Size			bitmapSize;
		private readonly Margins		crop;
		private readonly ImageFilter	filter;
		private readonly int			id;
	}
}
