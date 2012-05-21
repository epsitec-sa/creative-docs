//	Copyright © 2006-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

/*
 *	Note: on ne doit pas faire de image.Dispose (cf. //!), car il se peut
 *	qu'un appelant soit encore en train de travailler avec l'instance en
 *	question. Un Dispose serait dès lors fatal (accès hors de la mémoire,
 *	par exemple)
 */

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Cache statique global des images de l'application.
	/// </summary>
	public static partial class GlobalImageCache
	{
		internal static void Register(ImageCache imageCache)
		{
			//	Enregistre un cache local; si c'est le premier, il y a un certain
			//	nombre d'initialisations à faire.
			if (GlobalImageCache.localCache.Count == 0)
			{
				GlobalImageCache.Initialize();
			}

			GlobalImageCache.localCache.Add(imageCache);
		}

		internal static void Unregister(ImageCache imageCache)
		{
			//	Supprime un cache local de notre liste; si c'est le dernier, il y
			//	a un certain nombre de nettoyages à faire.
			GlobalImageCache.localCache.Remove(imageCache);

			if (GlobalImageCache.localCache.Count == 0)
			{
				GlobalImageCache.ShutDown();
			}
		}

		private static void Initialize()
		{
			GlobalImageCache.imageManager = ImageManager.Instance;
			GlobalImageCache.imageManager.RegisterProtocol(GlobalImageCache.ZipProtocolPrefix, GlobalImageCache.ReadZippedDocument);

			Bitmap.OutOfMemoryEncountered += sender => GlobalImageCache.FreeEverything ();
		}

		private static void ShutDown()
		{
			GlobalImageCache.imageManager.UnregisterProtocol(GlobalImageCache.ZipProtocolPrefix);
			GlobalImageCache.imageManager = null;
		}
		
		public static Item Find(string key, string zipFileName)
		{
			//	Retourne les données d'une image.
			Item item;
			key = GlobalImageCache.GetGlobalKeyName(key, zipFileName);
			GlobalImageCache.items.TryGetValue(key, out item);
			return item;
		}

		public static ImageManager ImageManager
		{
			get
			{
				//	Retourne le cache du gestionnaire d'images (cache sur disque).
				return GlobalImageCache.imageManager;
			}
		}

		private static byte[] ReadZippedDocument(string zipPath)
		{
			//	Lit les données depuis un document ZIP.
			string zipFileName;
			string zipEntryName;

			if (GlobalImageCache.ExtractZipPathNames(string.Concat(GlobalImageCache.ZipProtocolPrefix, zipPath), out zipFileName, out zipEntryName))
			{
				DocumentManager manager = GlobalImageCache.FindDocumentManager(zipFileName);

				using (System.IO.Stream stream = manager.GetLocalFileStream(System.IO.FileAccess.Read))
				{
					if (stream != null)
					{
						IO.ZipFile zip = new IO.ZipFile();

						bool ok = zip.TryLoadFile(stream, name => (name == zipEntryName));

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
			string zipPath = zipFileName.ToLowerInvariant();

			foreach (ImageCache cache in GlobalImageCache.localCache)
			{
				Document document = cache.Document;
				DocumentManager manager = document.DocumentManager;

				if (manager != null)
				{
					string path = manager.GetLocalFilePath().ToLowerInvariant();

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
			System.Diagnostics.Debug.Assert(zipEntryName.Contains(":") == false);
			return string.Concat(GlobalImageCache.ZipProtocolPrefix, zipFileName, ":", zipEntryName);
		}

		public static bool ExtractZipPathNames(string zipPath, out string zipFileName, out string zipEntryName)
		{
			if (!string.IsNullOrEmpty(zipPath) && zipPath.StartsWith(GlobalImageCache.ZipProtocolPrefix))
			{
				zipPath = zipPath.Substring(GlobalImageCache.ZipProtocolPrefix.Length);
				int pos = zipPath.LastIndexOf(':');

				if (pos > 0)
				{
					zipFileName = zipPath.Substring(0, pos);
					zipEntryName = zipPath.Substring(pos+1);

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
				if (pair.Key.StartsWith(prefix))
				{
					if (string.IsNullOrEmpty(zipFileName) || pair.Value.ZipFileName == zipFileName)
					{
						return pair.Value;
					}
				}
			}
			
			return null;
		}

		public static void RemoveReferenceToZip(string zipFileName)
		{
			List<Item> list = new List<Item>();
			
			foreach (KeyValuePair<string, Item> pair in GlobalImageCache.items)
			{
				if (pair.Value.ZipFileName == zipFileName)
				{
					list.Add(pair.Value);
				}
			}

			foreach (Item item in list)
			{
				GlobalImageCache.Remove(item);
			}
		}

		public static Item Add(string key, string fileName, string zipPath, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.
			System.Diagnostics.Debug.Assert(date != System.DateTime.MinValue);

			Item item;

			string zipFileName;
			string zipEntryName;

			GlobalImageCache.ExtractZipPathNames(zipPath, out zipFileName, out zipEntryName);

			key = GlobalImageCache.GetGlobalKeyName(key, zipFileName);

			if (GlobalImageCache.items.TryGetValue(key, out item))
			{
				return item;
			}
			else
			{
				item = new Item(fileName, zipPath, date);

				if (!string.IsNullOrEmpty(zipPath) || System.IO.File.Exists(fileName))
				{
					GlobalImageCache.items.Add(key, item);
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

			if (GlobalImageCache.items.ContainsKey(key))
			{
				GlobalImageCache.items.Remove(key);
				item.Dispose();
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
			key = GlobalImageCache.GetGlobalKeyName(key, zipFileName);
			
			if (GlobalImageCache.items.TryGetValue(key, out item))
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

		internal static void FreeEverything()
		{
			GlobalImageCache.FreeEverything (ImagePart.LargeOriginal);
			GlobalImageCache.FreeEverything (ImagePart.SmallOriginal);
			GlobalImageCache.FreeEverything (ImagePart.Lowres);
			GlobalImageCache.FreeEverything (ImagePart.Data);
			
			System.GC.Collect ();
		}

		private static void FreeEverything(ImagePart part)
		{
			while (true)
			{
				Item older = GlobalImageCache.SearchOlder (part);
				
				if (older == null)
				{
					break;
				}

				older.Free (part);
			}
		}

		private static void FreeOldest(ref long total, ImagePart part)
		{
			//	Libère une partie des images les plus vieilles, pour que le total
			//	du cache ne dépasse pas 0.5 GB.
			while (total > GlobalImageCache.GlobalLimit)  // dépasse la limite globale ?
			{
				Item older = GlobalImageCache.SearchOlder(part);

				if (older == null)  // rien trouvé ?
				{
					break;  // on arrête de libérer, même si la limite est dépassée
				}

				total -= older.Free(part);
				//?System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: total={0}", total.ToString()));
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

				if (!item.Locked && item.IsFreeable(part))
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




		internal static System.DateTime GetFileDate(string path)
		{
			//	Retourne la date de dernière modification d'un fichier.
			try
			{
				if (!string.IsNullOrEmpty (path) && System.IO.File.Exists (path))
				{
					return System.IO.File.GetLastWriteTimeUtc (path);
				}
			}
			catch
			{
			}

			return System.DateTime.MinValue;
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


		private static string GetGlobalKeyName(string localKeyName, string zipFileName)
		{
			if (string.IsNullOrEmpty(zipFileName))
			{
				return localKeyName;
			}
			else
			{
				return string.Concat(localKeyName, "|", zipFileName);
			}
		}

		
		private static readonly long				GlobalLimit = 500*1024;  // limite globale de 0.5 GB
		//?private static readonly long				GlobalLimit =  20*1024;  // limite globale de 20 MB
		private static readonly long				ImageLimit  =   1*1024;  // limite par image de 1 MB
		private const string						ZipProtocolPrefix = "crdoc-ZIP:";

		private static Dictionary<string, Item>		items = new Dictionary<string, Item>();
		private static long							timeStamp = 0;
		private static ImageManager					imageManager;
		private static List<ImageCache>				localCache = new List<ImageCache>();
	}
}
