using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.IO;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Cache statique global des images de l'application.
	/// </summary>
	public class GlobalImageCache
	{
		public static Item Get(string filename)
		{
			//	Retourne les données d'une image.
			if (string.IsNullOrEmpty(filename))
			{
				return null;
			}

			if (GlobalImageCache.dico.ContainsKey(filename))
			{
				return GlobalImageCache.dico[filename];
			}
			else
			{
				return null;
			}
		}

		public static bool Contains(string filename)
		{
			//	Vérifie si une image est en cache.
			return GlobalImageCache.dico.ContainsKey(filename);
		}

		public static Item Add(string filename, string zipFilename, string zipShortName, byte[] data)
		{
			//	Ajoute une nouvelle image dans le cache.
			//	Si les données 'data' n'existent pas, l'image est lue sur disque.
			//	Si les données existent, l'image est lue à partir des données en mémoire.
			if (GlobalImageCache.dico.ContainsKey(filename))
			{
				return GlobalImageCache.dico[filename];
			}
			else
			{
				Item item = new Item(filename, zipFilename, zipShortName, data);
				if (item.Data == null)
				{
					return null;
				}
				else
				{
					GlobalImageCache.dico.Add(filename, item);
					return item;
				}
			}
		}

		public static void Remove(string filename)
		{
			//	Supprime une image dans le cache.
			Item item = GlobalImageCache.dico[filename];
			if (item != null)
			{
				GlobalImageCache.dico.Remove(filename);
				item.Dispose();
			}
		}

		public static void Free()
		{
			//	Libère toutes les images, si nécessaire.
#if false
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				item.FreeOriginal();
			}
#endif
		}


		public static void FreeOldest()
		{
			//	Libère les images les plus vieilles, pour que le total du cache
			//	ne dépasse pas 0.5 GB.
			long total = 0;
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				total += item.KBUsed;  // total <- taille totale utilisée par le cache
			}

			//	Libère une partie des images originales.
			while (total > GlobalImageCache.globalLimit)  // dépasse la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(ImagePart.Original);
				if (older == null)
				{
					break;
				}

				total -= older.FreeOriginal();
			}

			//	Libère une partie des images basse résolution.
			while (total > GlobalImageCache.globalLimit)  // dépasse la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(ImagePart.Lowres);
				if (older == null)
				{
					break;
				}

				total -= older.FreeLowres();
			}

			//	Libère une partie des données de base.
			while (total > GlobalImageCache.globalLimit)  // dépasse la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(ImagePart.Data);
				if (older == null)
				{
					break;
				}

				total -= older.FreeData();
			}
		}

		public enum ImagePart
		{
			Data,
			Original,
			Lowres,
		}

		protected static Item SearchOlder(ImagePart part)
		{
			//	Cherche l'image libèrable la plus vieille du cache.
			long max = long.MaxValue;
			Item older = null;

			foreach (Item item in GlobalImageCache.dico.Values)
			{
				if (item.IsFreeable(part) && max > item.TimeStamp)
				{
					max = item.TimeStamp;
					older = item;
				}
			}

			return older;
		}


		#region Class Item
		public class Item : System.IDisposable
		{
			public Item(string filename, string zipFilename, string zipShortName, byte[] data)
			{
				//	Constructeur qui met en cache les données de l'image.
				//	Si les données 'data' n'existent pas, l'image est lue sur disque.
				//	Si les données existent, l'image est lue à partir des données en mémoire.
				this.filename = filename;
				this.zipFilename= zipFilename;
				this.zipShortName = zipShortName;

				try
				{
					if (data == null)  // lecture sur disque ?
					{
						this.data = System.IO.File.ReadAllBytes(this.filename);
					}
					else  // image en mémoire ?
					{
						this.data = data;
					}
				}
				catch
				{
					this.data = null;
				}
			}

			public bool Reload()
			{
				//	Relit l'image sur disque. Utile lorsque le fichier a changé.
				//	Retourne false en cas d'erreur (fichier n'existe pas).
				byte[] initialData = this.data;

				try
				{
					this.data = System.IO.File.ReadAllBytes(this.filename);
				}
				catch
				{
					this.data = initialData;
					return false;
				}

				this.originalImage = null;
				this.lowresImage = null;
				return true;
			}

			public bool IsFreeable(ImagePart part)
			{
				//	Indique s'il est possible de libèrer quelque chose dans cette image.
				if (part == ImagePart.Data)
				{
					return (this.data != null);
				}

				if (part == ImagePart.Original)
				{
					return (this.originalImage != null && this.IsBigOriginal);
				}

				if (part == ImagePart.Lowres)
				{
					return (this.lowresImage != null);
				}

				return false;
			}

			public long FreeOriginal()
			{
				//	Libère si possible l'image originale.
				//	Retourne la taille libérée en KB.
				long total = 0;

				if (this.originalImage != null)
				{
					total = this.KBOriginalWeight;

					this.originalImage.Dispose();
					this.originalImage = null;
				}

				return total;
			}

			public long FreeLowres()
			{
				//	Libère si possible l'image basse résolution.
				//	Retourne la taille libérée en KB.
				long total = 0;

				if (this.lowresImage != null)
				{
					total = this.KBLowresUsed;

					this.lowresImage.Dispose();
					this.lowresImage = null;
				}

				return total;
			}

			public long FreeData()
			{
				//	Libère si possible toutes les données de l'image.
				//	Retourne la taille libérée en KB.
				long total = 0;

				if (this.data != null)
				{
					total = this.KBDataUsed;
					this.data = null;
				}

				return total;
			}

			public long TimeStamp
			{
				//	Retourne la marque de vieillesse.
				get
				{
					return this.timeStamp;
				}
			}

			public void SetRecentTimeStamp()
			{
				//	Met la marque de vieillesse la plus récente.
				this.timeStamp = GlobalImageCache.timeStamp++;
			}

			public void Write(string otherFilename)
			{
				//	Exporte l'image originale dans un fichier quelconque.
				System.IO.File.WriteAllBytes(otherFilename, this.data);
			}

			public Size Size
			{
				//	Retourne la taille de l'image originale.
				get
				{
					return this.originalSize;
				}
			}

			public double LowresScale
			{
				//	Retourne l'échelle de l'image pour l'affichage (>= 1).
				get
				{
					return this.lowresScale;
				}
			}

			public string Filename
			{
				//	Retourne le nom de fichier avec le chemin complet.
				get
				{
					return this.filename;
				}
			}
			
			public string ZipFilename
			{
				//	Nom du fichier zip contenant l'image.
				get
				{
					return this.zipFilename;
				}
			}

			public string ZipShortName
			{
				//	Nom court de l'image dans le fichier zip.
				get
				{
					return this.zipShortName;
				}
			}

			public byte[] Data
			{
				//	Données brutes de l'image.
				get
				{
					return this.data;
				}
			}

			public Drawing.Image Image(bool isLowres)
			{
				//	Retourne l'objet Drawing.Image.
				if (this.data == null)
				{
					return null;
				}

				if (isLowres)
				{
					this.ReadLowresImage();

					if (this.lowresImage != null)
					{
						return this.lowresImage;
					}
					else
					{
						return this.originalImage;
					}
				}
				else
				{
					this.ReadOriginalImage();
					return this.originalImage;
				}
			}

			public long KBUsed
			{
				//	Retourne la taille totale utilisée par l'image en KB.
				//	Prend en compte les données, l'image originale et l'image basse résolution.
				get
				{
					long total = 0;

					if (this.originalImage != null)
					{
						total += this.KBOriginalWeight;
					}

					total += this.KBDataUsed;
					total += this.KBLowresUsed;

					return total;
				}
			}

			public long KBOriginalWeight
			{
				//	Retourne la taille de l'image originale en KB.
				get
				{
					return ((long) this.originalSize.Width * (long) this.originalSize.Height) / (1024/4);
				}
			}

			public long KBLowresUsed
			{
				//	Retourne la taille de l'image basse résolution en KB.
				get
				{
					if (this.lowresImage == null)
					{
						return 0;
					}
					else
					{
						double w = this.originalSize.Width  / this.lowresScale;
						double h = this.originalSize.Height / this.lowresScale;
						return ((long) w * (long) h) / (1024/4);
					}
				}
			}

			public long KBDataUsed
			{
				//	Retourne la taille utilisée par les données de l'image en KB.
				get
				{
					if (this.data == null)
					{
						return 0;
					}
					else
					{
						return this.data.Length/1024;
					}
				}
			}

			protected bool IsBigOriginal
			{
				//	Retourne 'true' si l'image originale dépasse la limite.
				get
				{
					return (this.KBOriginalWeight > GlobalImageCache.imageLimit);
				}
			}

			protected void ReadOriginalImage()
			{
				//	Lit l'image originale, si nécessaire.
				this.ReadImageData();

				if (this.data == null || this.originalImage != null)
				{
					return;
				}

				this.originalImage = Bitmap.FromData(this.data);
				System.Diagnostics.Debug.Assert(this.originalImage != null);
				this.originalSize = this.originalImage.Size;
			}

			protected void ReadImageData()
			{
				//	Relit les données de l'image, si nécessaire.
				if (this.data != null)
				{
					return;
				}

				if (this.zipFilename == null)
				{
					this.data = System.IO.File.ReadAllBytes(this.filename);
				}
				else
				{
					ZipFile zip = new ZipFile();

					this.filenameToRead = string.Format("images/{0}", this.zipShortName);
					if (zip.TryLoadFile(this.zipFilename, this.IsZipLoading))
					{
						this.data = zip[this.filenameToRead].Data;  // lit les données dans le fichier zip
					}
				}
			}

			protected bool IsZipLoading(string entryName)
			{
				return (entryName == this.filenameToRead);
			}

			protected void ReadLowresImage()
			{
				//	Si l'image originale est trop grosse, crée l'image basse résolution
				//	pour l'affichage et libère l'image originale.
				if (this.data == null || this.lowresImage != null)
				{
					return;
				}

				this.ReadOriginalImage();

				if (this.IsBigOriginal)  // image dépasse la limite ?
				{
					//	Génère une image pour l'affichage (this.lowresImage) qui pèse
					//	environ la limite fixée.
					this.lowresScale = System.Math.Sqrt(this.KBOriginalWeight/GlobalImageCache.imageLimit);
					int dx = (int) (this.originalSize.Width/this.lowresScale);
					int dy = (int) (this.originalSize.Height/this.lowresScale);
					this.lowresImage = GlobalImageCache.ResizeImage(this.originalImage, dx, dy);

					this.originalImage.Dispose();  // oublie tout de suite l'image originale
					this.originalImage = null;
				}
				else
				{
					this.lowresScale = 1.0;
				}
			}

			#region IDisposable Members
			public void Dispose()
			{
				this.data = null;

				if (this.originalImage != null)
				{
					this.originalImage.Dispose();
					this.originalImage = null;
				}

				if (this.lowresImage != null)
				{
					this.lowresImage.Dispose();
					this.lowresImage = null;
				}
			}
			#endregion
						
			protected string				filename;
			protected string				zipFilename;
			protected string				zipShortName;
			protected string				filenameToRead;
			protected byte[]				data;
			protected Drawing.Image			originalImage;
			protected Drawing.Image			lowresImage;
			protected Size					originalSize;
			protected double				lowresScale;
			protected long					timeStamp;
		}
		#endregion


		protected static Drawing.Image ResizeImage(Drawing.Image image, int dx, int dy)
		{
			//	Retourne une image redimensionnée.
			Graphics gfx = new Graphics();
			gfx.SetPixmapSize(dx, dy);
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(1, -1, 0, 0);

			gfx.ImageFilter = new ImageFilter(ImageFilteringMode.Bilinear);  // moche mais rapide
			gfx.PaintImage(image, new Rectangle(0, 0, dx, dy));

			return Bitmap.FromPixmap(gfx.Pixmap) as Bitmap;
		}


		protected static readonly long				globalLimit = 500000;  // limite globale de 0.5 GB
		protected static readonly long				imageLimit  =   1000;  // limite par image de 1 MB

		protected static Dictionary<string, Item>	dico = new Dictionary<string, Item>();
		protected static long						timeStamp = 0;
	}
}
