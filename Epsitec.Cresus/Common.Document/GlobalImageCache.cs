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
	public static class GlobalImageCache
	{
		internal static void Register(ImageCache imageCache)
		{
			//	Enregistre un cache local; si c'est le premier, il y a un certain
			//	nombre d'initialisations à faire.

			if (GlobalImageCache.localCache.Count == 0)
			{
				GlobalImageCache.Initialize ();
			}

			GlobalImageCache.localCache.Add (imageCache);
		}

		internal static void Unregister(ImageCache imageCache)
		{
			//	Supprime un cache local de notre liste; si c'est le dernier, il y
			//	a un certain nombre de nettoyages à faire.

			GlobalImageCache.localCache.Remove (imageCache);

			if (GlobalImageCache.localCache.Count == 0)
			{
				GlobalImageCache.ShutDown ();
			}
		}

		private static void Initialize()
		{
			GlobalImageCache.cache = ImageManager.GetDefaultCache ();
			GlobalImageCache.cache.RegisterProtocol (GlobalImageCache.zipPrefix, GlobalImageCache.ZippedDocumentReader);
		}

		private static void ShutDown()
		{
			GlobalImageCache.cache.UnregisterProtocol (GlobalImageCache.zipPrefix);
			GlobalImageCache.cache = null;
		}
		
		public static Item Find(string key, string zipFileName)
		{
			//	Retourne les données d'une image.
			Item item;
			key = GlobalImageCache.GetGlobalKeyName (key, zipFileName);
			GlobalImageCache.items.TryGetValue (key, out item);
			return item;
		}

		public static ImageManager GetImageManager()
		{
			//	Retourne le cache du gestionnaire d'images (cache sur disque).
			return GlobalImageCache.cache;
		}

		private static byte[] ZippedDocumentReader(string zipPath)
		{
			//	Lit les données depuis un document ZIP.
			string zipFileName;
			string zipEntryName;

			if (GlobalImageCache.ExtractZipPathNames (string.Concat (GlobalImageCache.zipPrefix, zipPath), out zipFileName, out zipEntryName))
			{
				DocumentManager manager = GlobalImageCache.FindDocumentManager (zipFileName);

				using (System.IO.Stream stream = manager.GetLocalFileStream (System.IO.FileAccess.Read))
				{
					if (stream != null)
					{
						IO.ZipFile zip = new IO.ZipFile ();

						bool ok = zip.TryLoadFile (stream,
							delegate (string name)
							{
								return (name == zipEntryName);
							});

						if (ok)
						{
							return zip[zipEntryName].Data;  // lit les données dans le fichier zip
						}
					}
				}
			}

			return null;
		}

		private static DocumentManager FindDocumentManager(string zipFileName)
		{
			string zipPath = zipFileName.ToLowerInvariant ();

			foreach (ImageCache cache in GlobalImageCache.localCache)
			{
				Document document = cache.Document;
				DocumentManager manager = document.DocumentManager;

				if (manager != null)
				{
					string path = manager.GetLocalFilePath ().ToLowerInvariant ();

					if (path == zipPath)
					{
						return manager;
					}
				}
			}

			return null;
		}

		public static string CreateZipPath(string zipFileName, string zipEntryName)
		{
			System.Diagnostics.Debug.Assert (zipEntryName.Contains (":") == false);
			return string.Concat (GlobalImageCache.zipPrefix, zipFileName, ":", zipEntryName);
		}

		public static bool ExtractZipPathNames(string zipPath, out string zipFileName, out string zipEntryName)
		{
			if (!string.IsNullOrEmpty (zipPath) &&
				zipPath.StartsWith (GlobalImageCache.zipPrefix))
			{
				zipPath = zipPath.Substring (GlobalImageCache.zipPrefix.Length);
				int pos = zipPath.LastIndexOf (':');

				if (pos > 0)
				{
					zipFileName = zipPath.Substring (0, pos);
					zipEntryName = zipPath.Substring (pos+1);

					return true;
				}
			}

			zipFileName = null;
			zipEntryName = null;

			return false;
		}



		public static Item FindStartingWith(string prefix, string zipFileName)
		{
			foreach (KeyValuePair<string, Item> pair in GlobalImageCache.items)
			{
				if (pair.Key.StartsWith (prefix))
				{
					if (string.IsNullOrEmpty (zipFileName) ||
						pair.Value.ZipFileName == zipFileName)
					{
						return pair.Value;
					}
				}
			}
			
			return null;
		}

		public static void RemoveReferenceToZip(string zipFileName)
		{
			List<Item> list = new List<Item> ();
			
			foreach (KeyValuePair<string, Item> pair in GlobalImageCache.items)
			{
				if (pair.Value.ZipFileName == zipFileName)
				{
					list.Add (pair.Value);
				}
			}

			foreach (Item item in list)
			{
				GlobalImageCache.Remove (item);
			}
		}

		public static Item Add(string key, string fileName, string zipPath, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.

			System.Diagnostics.Debug.Assert (date != System.DateTime.MinValue);

			Item item;

			string zipFileName;
			string zipEntryName;

			GlobalImageCache.ExtractZipPathNames (zipPath, out zipFileName, out zipEntryName);

			key = GlobalImageCache.GetGlobalKeyName (key, zipFileName);

			if (GlobalImageCache.items.TryGetValue (key, out item))
			{
				return item;
			}
			else
			{
				item = new Item (fileName, zipPath, date);

				if (!string.IsNullOrEmpty (zipPath) ||
					System.IO.File.Exists (fileName))
				{
					GlobalImageCache.items.Add (key, item);
					return item;
				}
				else
				{
					return null;
				}
			}
		}

		public static void Remove(Item item)
		{
			//	Supprime une image dans le cache.

			string key = item.GlobalKeyName;

			if (GlobalImageCache.items.ContainsKey (key))
			{
				GlobalImageCache.items.Remove (key);
				item.Dispose ();
			}
		}

		public static void UnlockAll()
		{
			//	Débloque toutes les images utilisées.
			foreach (Item item in GlobalImageCache.items.Values)
			{
				item.Locked = false;
			}
		}

		public static void Lock(string key, string zipFileName)
		{
			//	Bloque l'image car elle est utilisée dans la page.
			Item item;

			key = GlobalImageCache.GetGlobalKeyName (key, zipFileName);
			
			if (GlobalImageCache.items.TryGetValue (key, out item))
			{
				item.Locked = true;
			}
		}


		public static void FreeOldest()
		{
			//	Libère les images les plus vieilles, pour que le total du cache
			//	ne dépasse pas 0.5 GB.
			long total = 0;
			foreach (Item item in GlobalImageCache.items.Values)
			{
				total += item.KBUsed();  // total <- taille totale utilisée par le cache
			}

			GlobalImageCache.FreeOldest(ref total, ImagePart.LargeOriginal);
			GlobalImageCache.FreeOldest(ref total, ImagePart.SmallOriginal);
			GlobalImageCache.FreeOldest(ref total, ImagePart.Lowres);
			GlobalImageCache.FreeOldest(ref total, ImagePart.Data);
		}

		private static void FreeOldest(ref long total, ImagePart part)
		{
			//	Libère une partie des images les plus vieilles, pour que le total
			//	du cache ne dépasse pas 0.5 GB.
			while (total > GlobalImageCache.globalLimit)  // dépasse la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(part);

				if (older == null)  // rien trouvé ?
				{
					break;  // on arrête de libérer, même si la limite est dépassée
				}

				total -= older.Free(part);
				System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: total={0}", total.ToString()));
			}
		}

		public enum ImagePart
		{
			LargeOriginal,	// image originale, seulement si elle est grande
			SmallOriginal,	// image originale, dans tous les cas
			Lowres,			// image en basse résolution
			Data,			// données de base
		}

		private static Item SearchOlder(ImagePart part)
		{
			//	Cherche la partie d'image libérable la plus vieille du cache.
			long min = long.MaxValue;
			long max = 0;
			Item older = null;

			foreach (Item item in GlobalImageCache.items.Values)
			{
				long time = item.TimeStamp;
				
				if (!item.Locked && item.IsFreeable (part))
				{
					if (min > time)
					{
						min = time;
						older = item;
					}
				}
				if (max < time)
				{
					max = time;
				}
			}

			if (min == max)
			{
				//	Evite de retourner le dernier élément ajouté dans le cache.
				return null;
			}

			return older;
		}


		#region Class Item
		public class Item : System.IDisposable
		{
			public Item(string filename, string zipPath, System.DateTime date)
			{
				//	Constructeur qui met en cache les données de l'image.
				
				System.Diagnostics.Debug.Assert(date != System.DateTime.MinValue);

				this.filename = filename;

				if (string.IsNullOrEmpty (zipPath))
				{
				}
				else
				{
					GlobalImageCache.ExtractZipPathNames (zipPath, out this.zipFilename, out this.zipEntryName);
				}

				this.date = date;
				this.SetRecentTimeStamp();
			}

			public bool Reload()
			{
				//	Relit l'image sur disque. Utile lorsque le fichier a changé.
				//	Retourne false en cas d'erreur (fichier n'existe pas).
				byte[] initialData = this.data;

				try
				{
					this.data = System.IO.File.ReadAllBytes(this.filename);
					this.date = GlobalImageCache.GetFileDate(this.filename);
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
				//	Libère si possible une partie de l'image.
				//	Retourne la taille libérée en KB.
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
				//	Indique s'il est possible de libérer une partie de cette image.
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
				//	Retourne la taille totale utilisée par l'image (toutes les parties) en KB.
				//	Prend en compte l'image originale, l'image basse résolution et les données.
				long total = 0;

				total += this.KBUsed(ImagePart.LargeOriginal);  // ne pas compter LargeOriginal + SmallOriginal !
				total += this.KBUsed(ImagePart.Lowres);
				total += this.KBUsed(ImagePart.Data);

				return total;
			}

			public long KBUsed(ImagePart part)
			{
				//	Retourne la taille utilisée par une partie de l'image.
				//	Si la partie n'est pas utilisée, retourne zéro.
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
						double w = this.originalSize.Width  / this.lowResScale;
						double h = this.originalSize.Height / this.lowResScale;
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
				//	Retourne 'true' si l'image originale dépasse la limite.
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
				//	Met la marque de vieillesse la plus récente.
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

			public Size Size
			{
				//	Retourne la taille de l'image originale.
				get
				{
					return this.originalSize;
				}
			}

			public double LowResScale
			{
				//	Retourne l'échelle de l'image pour l'affichage (>= 1).
				get
				{
					return this.lowResScale;
				}
			}

			public string LocalKeyName
			{
				get
				{
					return ImageManager.GetKey (this.filename, this.date);
				}
			}

			public string GlobalKeyName
			{
				get
				{
					return GlobalImageCache.GetGlobalKeyName (this.LocalKeyName, this.zipFilename);
				}
			}

			public string FileName
			{
				//	Retourne le nom de fichier avec le chemin complet.
				get
				{
					return this.filename;
				}
			}
			
			public string ZipFileName
			{
				//	Nom du fichier zip contenant l'image.
				get
				{
					return this.zipFilename;
				}
			}

			public string ZipEntryName
			{
				//	Nom de l'image dans le fichier zip.
				get
				{
					return this.zipEntryName;
				}
			}

			public System.DateTime FileDate
			{
				//	Retourne la date de dernière modification.
				get
				{
					return this.date;
				}
			}

			public bool HasData
			{
				//	Indique si les données brutes de l'image existent.
				get
				{
					return this.data != null;
				}
			}

			public byte[] GetImageData()
			{
				//	Données brutes de l'image.
				this.ReadImageData(true);
				return this.data;
			}

			public Drawing.Image GetImage(ImageCacheResolution resolution, bool read)
			{
				//	Retourne l'objet Drawing.Image.
				if (resolution == ImageCacheResolution.Low)
				{
					this.ReadLowresImage(read);

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
					this.ReadOriginalImage(read);

					return this.originalImage;
				}
			}

			protected void ReadLowresImage(bool read)
			{
				//	Si l'image originale est trop grosse, crée l'image basse résolution
				//	pour l'affichage et libère l'image originale.
				if ((this.lowresImage != null) ||
					(read == false))
				{
					return;
				}

				//	Cherche d'abord l'image dans le cache persistant.
				string path = this.zipFilename == null ? string.Concat ("file:", this.filename) : GlobalImageCache.CreateZipPath (this.zipFilename, this.zipEntryName);
				ImageData imageData = GlobalImageCache.GetImageManager ().GetImage (path, this.filename, this.date);
				Opac.FreeImage.Image sampleImage = imageData.GetSampleImage ();

				if (sampleImage == null)
				{
					this.originalSize = new Size (imageData.SourceWidth, imageData.SourceHeight);
					this.lowResScale = 1.0;
				}
				else
				{
					this.originalSize = new Size (imageData.SourceWidth, imageData.SourceHeight);
					this.lowresImage = Bitmap.FromImage (sampleImage);
					this.lowResScale = (double) this.originalSize.Width / this.lowresImage.Width;
				}
			}

			protected void ReadOriginalImage(bool read)
			{
				//	Lit l'image originale, si nécessaire.
				this.ReadImageData(read);

				if (this.data == null || this.originalImage != null || read == false)
				{
					return;
				}

				System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: ReadOriginalImage {0}", this.filename));
				this.originalImage = Bitmap.FromData(this.data);
				System.Diagnostics.Debug.Assert(this.originalImage != null);
				this.originalSize = this.originalImage.Size;
				this.SetRecentTimeStamp();
			}

			protected void ReadImageData(bool read)
			{
				//	Relit les données de l'image, si nécessaire.
				if ((this.data != null) ||
					(read == false))
				{
					return;
				}

				if (this.zipFilename == null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("GlobalImageCache: ReadImageData from file {0}", this.filename));
					this.data = System.IO.File.ReadAllBytes (this.filename);
					this.date = GlobalImageCache.GetFileDate(this.filename);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("GlobalImageCache: ReadImageData from zip {0} {1}", this.zipFilename, this.zipEntryName));
					ZipFile zip = new ZipFile ();

					bool ok = zip.TryLoadFile (this.zipFilename,
						delegate (string entryName)
						{
							return (entryName == this.zipEntryName);
						});

					if (ok)
					{
						this.data = zip[this.zipEntryName].Data;  // lit les données dans le fichier zip
					}
				}

				this.SetRecentTimeStamp();
			}

			protected bool LoadCondition(string entryName)
			{
				return (entryName == this.zipEntryName);
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
			protected string				zipEntryName;
			protected System.DateTime		date;
			protected byte[]				data;
			protected Drawing.Image			originalImage;
			protected Drawing.Image			lowresImage;
			protected Size					originalSize;
			protected double				lowResScale;
			protected long					timeStamp;
			protected bool					locked;
		}
		#endregion


		internal static System.DateTime GetFileDate(string path)
		{
			//	Retourne la date de dernière modification d'un fichier.
			if ((!string.IsNullOrEmpty (path)) &&
				(System.IO.File.Exists (path)))
			{
				return System.IO.File.GetLastWriteTimeUtc (path);
			}
			else
			{
				return System.DateTime.MinValue;
			}
		}

		private static Drawing.Image ResizeImage(Drawing.Image image, int dx, int dy)
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


		//?private static readonly long				globalLimit = 500000;  // limite globale de 0.5 GB
		private static readonly long				globalLimit =  20000;  // limite globale de 20 MB
		private static readonly long				imageLimit  =   1000;  // limite par image de 1 MB
		private const string						zipPrefix = "crdoc-ZIP:";

		private static Dictionary<string, Item>		items = new Dictionary<string, Item> ();
		private static long							timeStamp = 0;
		private static ImageManager					cache;
		private static List<ImageCache>				localCache = new List<ImageCache> ();

		private static string GetGlobalKeyName(string localKeyName, string zipFileName)
		{
			if (string.IsNullOrEmpty (zipFileName))
			{
				return localKeyName;
			}
			else
			{
				return string.Concat (localKeyName, "|", zipFileName);
			}

		}
	}
}
