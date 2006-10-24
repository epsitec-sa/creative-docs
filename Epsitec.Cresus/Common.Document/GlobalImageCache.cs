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

		public static Item Add(string filename, string zipFilename, string zipShortName, byte[] data, System.DateTime date)
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
				Item item = new Item(filename, zipFilename, zipShortName, data, date);
				if (item.IsData)
				{
					GlobalImageCache.dico.Add(filename, item);
					return item;
				}
				else
				{
					return null;
				}
			}
		}

		public static void Remove(string filename)
		{
			//	Supprime une image dans le cache.
			if (GlobalImageCache.dico.ContainsKey(filename))
			{
				Item item = GlobalImageCache.dico[filename];
				GlobalImageCache.dico.Remove(filename);
				item.Dispose();
			}
		}

		public static void Lock(List<string> filenames)
		{
			//	Bloque toutes les images utilis�es dans la page.
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				if (filenames == null)
				{
					item.Locked = false;
				}
				else
				{
					item.Locked = filenames.Contains(item.Filename);
				}
			}
		}


		public static void FreeOldest()
		{
			//	Lib�re les images les plus vieilles, pour que le total du cache
			//	ne d�passe pas 0.5 GB.
			long total = 0;
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				total += item.KBUsed();  // total <- taille totale utilis�e par le cache
			}

			GlobalImageCache.FreeOldest(ref total, ImagePart.LargeOriginal);
			GlobalImageCache.FreeOldest(ref total, ImagePart.SmallOriginal);
			GlobalImageCache.FreeOldest(ref total, ImagePart.Lowres);
			GlobalImageCache.FreeOldest(ref total, ImagePart.Data);
		}

		protected static void FreeOldest(ref long total, ImagePart part)
		{
			//	Lib�re une partie des images les plus vieilles, pour que le total
			//	du cache ne d�passe pas 0.5 GB.
			while (total > GlobalImageCache.globalLimit)  // d�passe la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(part);

				if (older == null)  // rien trouv� ?
				{
					break;  // on arr�te de lib�rer, m�me si la limite est d�pass�e
				}

				total -= older.Free(part);
				System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: total={0}", total.ToString()));
			}
		}

		public enum ImagePart
		{
			LargeOriginal,	// image originale, seulement si elle est grande
			SmallOriginal,	// image originale, dans tous les cas
			Lowres,			// image en basse r�solution
			Data,			// donn�es de base
		}

		protected static Item SearchOlder(ImagePart part)
		{
			//	Cherche la partie d'image lib�rable la plus vieille du cache.
			long min = long.MaxValue;
			Item older = null;

			foreach (Item item in GlobalImageCache.dico.Values)
			{
				if (!item.Locked && item.IsFreeable(part) && min > item.TimeStamp)
				{
					min = item.TimeStamp;
					older = item;
				}
			}

			return older;
		}


		#region Class Item
		public class Item : System.IDisposable
		{
			public Item(string filename, string zipFilename, string zipShortName, byte[] data, System.DateTime date)
			{
				//	Constructeur qui met en cache les donn�es de l'image.
				//	Si les donn�es 'data' n'existent pas, l'image est lue sur disque.
				//	Si les donn�es existent, l'image est lue � partir des donn�es en m�moire.
				if (data == null)
				{
					System.Diagnostics.Debug.Assert(date == System.DateTime.MinValue);
				}
				else
				{
					System.Diagnostics.Debug.Assert(date != System.DateTime.MinValue);
				}

				this.filename = filename;
				this.zipFilename= zipFilename;
				this.zipShortName = zipShortName;
				this.date = date;

				try
				{
					if (data == null)  // lecture sur disque ?
					{
						this.data = System.IO.File.ReadAllBytes(this.filename);
						this.date = GlobalImageCache.FilenameDate(this.filename);
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

				this.SetRecentTimeStamp();
			}

			public bool Reload()
			{
				//	Relit l'image sur disque. Utile lorsque le fichier a chang�.
				//	Retourne false en cas d'erreur (fichier n'existe pas).
				byte[] initialData = this.data;

				try
				{
					this.data = System.IO.File.ReadAllBytes(this.filename);
					this.date = GlobalImageCache.FilenameDate(this.filename);
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

			public long Free(ImagePart part)
			{
				//	Lib�re si possible une partie de l'image.
				//	Retourne la taille lib�r�e en KB.
				long total = 0;

				if (this.IsFreeable(part))
				{
					System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: Free {0} {1}", this.filename, part.ToString()));
					total = this.KBUsed(part);

					if (part == ImagePart.LargeOriginal || part == ImagePart.SmallOriginal)
					{
						this.originalImage.Dispose();
						this.originalImage = null;
					}

					if (part == ImagePart.Lowres)
					{
						this.lowresImage.Dispose();
						this.lowresImage = null;
					}

					if (part == ImagePart.Data)
					{
						this.data = null;
					}
				}

				return total;
			}

			public bool IsFreeable(ImagePart part)
			{
				//	Indique s'il est possible de lib�rer une partie de cette image.
				if (part == ImagePart.LargeOriginal)
				{
					return (this.originalImage != null && this.IsLargeOriginal);
				}

				if (part == ImagePart.SmallOriginal)
				{
					return (this.originalImage != null);
				}

				if (part == ImagePart.Lowres)
				{
					return (this.lowresImage != null);
				}

				if (part == ImagePart.Data)
				{
					return (this.data != null);
				}

				return false;
			}

			public long KBUsed()
			{
				//	Retourne la taille totale utilis�e par l'image (toutes les parties) en KB.
				//	Prend en compte l'image originale, l'image basse r�solution et les donn�es.
				long total = 0;

				total += this.KBUsed(ImagePart.LargeOriginal);  // ne pas compter LargeOriginal + SmallOriginal !
				total += this.KBUsed(ImagePart.Lowres);
				total += this.KBUsed(ImagePart.Data);

				return total;
			}

			public long KBUsed(ImagePart part)
			{
				//	Retourne la taille utilis�e par une partie de l'image.
				//	Si la partie n'est pas utilis�e, retourne z�ro.
				long total = 0;

				if (part == ImagePart.LargeOriginal || part == ImagePart.SmallOriginal)
				{
					if (this.originalImage != null)
					{
						total += this.KBOriginalWeight;
					}
				}

				if (part == ImagePart.Lowres)
				{
					if (this.lowresImage != null)
					{
						double w = this.originalSize.Width  / this.lowresScale;
						double h = this.originalSize.Height / this.lowresScale;
						total += ((long) w * (long) h) / (1024/4);
					}
				}

				if (part == ImagePart.Data)
				{
					if (this.data != null)
					{
						total += this.data.Length/1024;
					}
				}

				return total;
			}

			public long KBOriginalWeight
			{
				//	Retourne la taille de l'image originale en KB, qu'elle existe ou pas.
				get
				{
					return ((long) this.originalSize.Width * (long) this.originalSize.Height) / (1024/4);
				}
			}

			protected bool IsLargeOriginal
			{
				//	Retourne 'true' si l'image originale d�passe la limite.
				get
				{
					return (this.KBOriginalWeight > GlobalImageCache.imageLimit);
				}
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

			public bool Locked
			{
				get
				{
					return this.locked;
				}
				set
				{
					this.locked = value;
				}
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

			public System.DateTime Date
			{
				//	Retourne la date de derni�re modification.
				get
				{
					return this.date;
				}
			}

			public bool IsData
			{
				//	Indique si les donn�es brutes de l'image existent.
				get
				{
					return this.data != null;
				}
			}

			public byte[] Data
			{
				//	Donn�es brutes de l'image.
				get
				{
					this.ReadImageData();
					return this.data;
				}
			}

			public Drawing.Image Image(bool isLowres)
			{
				//	Retourne l'objet Drawing.Image.
				this.ReadImageData();

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

			protected void ReadLowresImage()
			{
				//	Si l'image originale est trop grosse, cr�e l'image basse r�solution
				//	pour l'affichage et lib�re l'image originale.
				if (this.data == null || this.lowresImage != null)
				{
					return;
				}

				//	Cherche d'abord l'image dans le cache persistant.
				string key = PersistentImageCache.Key(this.filename, this.zipFilename, this.zipShortName);
				Size size;
				PersistentImageCache.Get(key, this.date, out this.lowresImage, out size);
				if (this.lowresImage == null)  // rien dans le cache persistant ?
				{
					this.ReadOriginalImage();

					if (this.IsLargeOriginal)  // image d�passe la limite ?
					{
						//	G�n�re une image pour l'affichage (this.lowresImage) qui p�se
						//	environ la limite fix�e.
						System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: ReadLowresImage {0}", this.filename));
						this.lowresScale = System.Math.Sqrt(this.KBOriginalWeight/GlobalImageCache.imageLimit);
						int dx = (int) (this.originalSize.Width/this.lowresScale);
						int dy = (int) (this.originalSize.Height/this.lowresScale);
						this.lowresImage = GlobalImageCache.ResizeImage(this.originalImage, dx, dy);

						PersistentImageCache.Add(key, this.date, this.lowresImage, this.originalSize);

						this.originalImage.Dispose();  // oublie tout de suite l'image originale
						this.originalImage = null;
					}
					else
					{
						this.lowresScale = 1.0;
					}
				}
				else  // trouv� dans le cache persistant ?
				{
					this.originalSize = size;
					this.lowresScale = System.Math.Sqrt(this.KBOriginalWeight/GlobalImageCache.imageLimit);
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

				System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: ReadOriginalImage {0}", this.filename));
				this.originalImage = Bitmap.FromData(this.data);
				System.Diagnostics.Debug.Assert(this.originalImage != null);
				this.originalSize = this.originalImage.Size;
				this.SetRecentTimeStamp();
			}

			protected void ReadImageData()
			{
				//	Relit les donn�es de l'image, si n�cessaire.
				if (this.data != null)
				{
					return;
				}

				System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: ReadImageData {0}", this.filename));
				if (this.zipFilename == null)
				{
					this.data = System.IO.File.ReadAllBytes(this.filename);
					this.date = GlobalImageCache.FilenameDate(this.filename);
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

				this.SetRecentTimeStamp();
			}

			protected bool IsZipLoading(string entryName)
			{
				return (entryName == this.filenameToRead);
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
			protected System.DateTime		date;
			protected byte[]				data;
			protected Drawing.Image			originalImage;
			protected Drawing.Image			lowresImage;
			protected Size					originalSize;
			protected double				lowresScale;
			protected long					timeStamp;
			protected bool					locked;
		}
		#endregion


		protected static System.DateTime FilenameDate(string filename)
		{
			//	Retourne la date de derni�re modification d'un fichier.
			System.IO.FileInfo info = new System.IO.FileInfo(filename);
			if (info.Exists)
			{
				return info.LastWriteTime;
			}
			else
			{
				return System.DateTime.MinValue;
			}
		}

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
		//?protected static readonly long				globalLimit = 50000;  // limite globale de 50 MB
		protected static readonly long				imageLimit  =   1000;  // limite par image de 1 MB

		protected static Dictionary<string, Item>	dico = new Dictionary<string, Item>();
		protected static long						timeStamp = 0;
	}
}
