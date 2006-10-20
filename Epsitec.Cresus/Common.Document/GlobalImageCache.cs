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
			//	Retourne les donn�es d'une image.
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
			//	V�rifie si une image est en cache.
			return GlobalImageCache.dico.ContainsKey(filename);
		}

		public static Item Add(string filename, string zipFilename, string zipShortName, byte[] data)
		{
			//	Ajoute une nouvelle image dans le cache.
			//	Si les donn�es 'data' n'existent pas, l'image est lue sur disque.
			//	Si les donn�es existent, l'image est lue � partir des donn�es en m�moire.
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
			//	Lib�re toutes les images, si n�cessaire.
#if false
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				item.FreeOriginal();
			}
#endif
		}


		public static void FreeOldest()
		{
			//	Lib�re les images les plus vieilles, pour que le total du cache
			//	ne d�passe pas 0.5 GB.
			long total = 0;
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				total += item.KBUsed;  // total <- taille totale utilis�e par le cache
			}

			//	Lib�re une partie des images originales.
			while (total > GlobalImageCache.globalLimit)  // d�passe la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(ImagePart.Original);
				if (older == null)
				{
					break;
				}

				total -= older.FreeOriginal();
			}

			//	Lib�re une partie des images basse r�solution.
			while (total > GlobalImageCache.globalLimit)  // d�passe la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(ImagePart.Lowres);
				if (older == null)
				{
					break;
				}

				total -= older.FreeLowres();
			}

			//	Lib�re une partie des donn�es de base.
			while (total > GlobalImageCache.globalLimit)  // d�passe la limite globale ?
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
			//	Cherche l'image lib�rable la plus vieille du cache.
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
				//	Constructeur qui met en cache les donn�es de l'image.
				//	Si les donn�es 'data' n'existent pas, l'image est lue sur disque.
				//	Si les donn�es existent, l'image est lue � partir des donn�es en m�moire.
				this.filename = filename;
				this.zipFilename= zipFilename;
				this.zipShortName = zipShortName;

				try
				{
					if (data == null)  // lecture sur disque ?
					{
						this.data = System.IO.File.ReadAllBytes(this.filename);
					}
					else  // image en m�moire ?
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
				//	Relit l'image sur disque. Utile lorsque le fichier a chang�.
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
				//	Indique s'il est possible de lib�rer quelque chose dans cette image.
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
				//	Lib�re si possible l'image originale.
				//	Retourne la taille lib�r�e en KB.
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
				//	Lib�re si possible l'image basse r�solution.
				//	Retourne la taille lib�r�e en KB.
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
				//	Lib�re si possible toutes les donn�es de l'image.
				//	Retourne la taille lib�r�e en KB.
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
				//	Met la marque de vieillesse la plus r�cente.
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
				//	Retourne l'�chelle de l'image pour l'affichage (>= 1).
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
				//	Donn�es brutes de l'image.
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
				//	Retourne la taille totale utilis�e par l'image en KB.
				//	Prend en compte les donn�es, l'image originale et l'image basse r�solution.
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
				//	Retourne la taille de l'image basse r�solution en KB.
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
				//	Retourne la taille utilis�e par les donn�es de l'image en KB.
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
				//	Retourne 'true' si l'image originale d�passe la limite.
				get
				{
					return (this.KBOriginalWeight > GlobalImageCache.imageLimit);
				}
			}

			protected void ReadOriginalImage()
			{
				//	Lit l'image originale, si n�cessaire.
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
				//	Relit les donn�es de l'image, si n�cessaire.
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
						this.data = zip[this.filenameToRead].Data;  // lit les donn�es dans le fichier zip
					}
				}
			}

			protected bool IsZipLoading(string entryName)
			{
				return (entryName == this.filenameToRead);
			}

			protected void ReadLowresImage()
			{
				//	Si l'image originale est trop grosse, cr�e l'image basse r�solution
				//	pour l'affichage et lib�re l'image originale.
				if (this.data == null || this.lowresImage != null)
				{
					return;
				}

				this.ReadOriginalImage();

				if (this.IsBigOriginal)  // image d�passe la limite ?
				{
					//	G�n�re une image pour l'affichage (this.lowresImage) qui p�se
					//	environ la limite fix�e.
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
			//	Retourne une image redimensionn�e.
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
