//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Drawing.Platform;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Pdf.Engine
{
	/// <summary>
	/// La classe ImageSurface enregistre les informations sur une image bitmap.
	/// Il existe une seule instance de ImageSurface par nom de fichier, si
	/// plusieurs Objects.Image utilisent la même image.
	/// </summary>
	public sealed class ImageSurface : System.IDisposable
	{
		public ImageSurface(string uniqueId, int id, Image image)
		{
			this.uniqueId = uniqueId;
			this.id       = id;
			this.image    = image;

			this.nativeBitmap = NativeBitmap.Create (this.image.BitmapImage.NativeBitmap);
		}

		public ImageSurface(string uniqueId, int id, Image image, ImageFilter filter)
			: this (uniqueId, id, image)
		{
			this.filter = filter;
		}

		
		public Image							Image
		{
			get
			{
				return this.image;
			}
		}

		public NativeBitmap						NativeBitmap
		{
			get
			{
				return this.nativeBitmap;
			}
		}

		public string							UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public ImageFilter						Filter
		{
			//	Filtre de l'image.
			get
			{
				return this.filter;
			}
		}

		public int								Id
		{
			//	Identificateur unique.
			get
			{
				return this.id;
			}
		}

		public PdfImageStream					ImageStream
		{
			get;
			set;
		}

		public ImageCompression					ImageCompression
		{
			get;
			set;
		}

		public PdfComplexSurfaceType			SurfaceType
		{
			get;
			set;
		}

		public ColorConversion					ColorConversion
		{
			get;
			set;
		}

		public double							JpegQuality
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
			
			if (this.nativeBitmap != null)
			{
				this.nativeBitmap.Dispose ();
			}
		}


		public static string GetUniqueId(Image image)
		{
			//	Retourne un identificateur unique basé sur les données de l'image.
			//	Deux objets Image différents ayant les mêmes contenus doivent rendre
			//	le même identificateur.

			var info = new UniqueIdInfo (image);
			var uid  = "";

			//	On maintient un cache pour éviter que si on passe plusieurs fois le
			//	même object image en entrée, on ne recalcule chaque fois son hash,
			//	vu que c'est une opération coûteuse.
			
			lock (ImageSurface.exclusion)
			{
				if (ImageSurface.infos.TryGetValue (info, out uid))
				{
					return uid;
				}
			}

			uid = Epsitec.Common.IO.Checksum.ComputeMd5Hash (image.BitmapImage.GetRawBitmapBytes ());

			lock (ImageSurface.exclusion)
			{
				var dead = ImageSurface.infos.Keys.Where (x => x.IsDead).ToList ();

				dead.ForEach (x => ImageSurface.infos.Remove (x));

				ImageSurface.infos[info] = uid;
			}

			return uid;
		}

		#region UniqueIdInfo Structure

		private struct UniqueIdInfo
		{
			public UniqueIdInfo(Image image)
			{
				this.image = new System.WeakReference (image);
				this.hash = image.GetHashCode ();
			}


			public bool							IsDead
			{
				get
				{
					return this.image.IsAlive == false;
				}
			}

			public Image						Image
			{
				get
				{
					return this.image.Target as Image;
				}
			}

			
			public override int GetHashCode()
			{
				return this.hash;
			}

			public override bool Equals(object obj)
			{
				return ((UniqueIdInfo) obj).image.Target == this.image.Target;
			}

			
			private readonly System.WeakReference image;
			private readonly int				hash;
		}

		#endregion

		private static readonly object			exclusion = new object ();
		private static readonly Dictionary<UniqueIdInfo, string> infos = new Dictionary<UniqueIdInfo, string> ();

		private readonly string					uniqueId;
		private readonly Image					image;
		private readonly NativeBitmap			nativeBitmap;
		private readonly ImageFilter			filter;
		private readonly int					id;
	}
}
