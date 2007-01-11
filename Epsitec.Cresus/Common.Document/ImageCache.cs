using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.IO;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe <c>ImageCache</c> gère un cache par document des images.
	/// </summary>
	public class ImageCache
	{
		public ImageCache()
		{
			this.items = new Dictionary<string, Item>();
			this.preferLowres = true;
		}


		public bool PreferLowresImages
		{
			//	Indique le type des images auxquelles on s'intéresse.
			get
			{
				return this.preferLowres;
			}

			set
			{
				if (this.preferLowres != value)
				{
					this.preferLowres = value;

					//	Informe tout le cache.
					foreach (Item item in this.items.Values)
					{
						item.IsLowres = this.preferLowres;
					}
				}
			}
		}

		internal static string GetImageKeyName(string fileName, System.DateTime fileDate)
		{
			if (fileDate == System.DateTime.MinValue)
			{
				throw new System.ArgumentException ("Invalid file date");
			}
			else
			{
				return string.Concat (fileName.ToLowerInvariant (), "|", fileDate.Ticks.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}
		}
		
		public System.DateTime Load(string fileName)
		{
			//	Essaie de charger une image dans le cache.
			//	Celle méthode est appelée chaque fois que le nom édité de l'image est changé.
			if (string.IsNullOrEmpty (fileName))
			{
				return System.DateTime.MinValue;
			}
			else
			{
				Item item;
				System.DateTime fileDate = GlobalImageCache.GetFileDate (fileName);
				
				if (fileDate == System.DateTime.MinValue)
				{
					//	Aucun fichier n'existe sur le disque, mais peut-être que le nom de
					//	ce fichier est connu quand-même par le cache, parce que le fichier
					//	fait partie du document...
					
					item = this.Find (fileName);
				}
				else
				{
					//	Le fichier a été trouvé sur le disque; peut-être qu'il existe déjà
					//	une entrée dans le cache pour ce fichier ? On vérifie en se basant
					//	aussi sur la date du fichier, car on peut avoir un document avec
					//	plusieurs versions du même fichier :

					item = this.Find (fileName, fileDate);

					if (item == null)
					{
						item = this.Add (fileName, fileDate);
					}
				}

				if (item != null)
				{
					return item.GlobalItem.Date;
				}
				else
				{
					return System.DateTime.MinValue;
				}
			}
		}

		public Item Find(string fileName, System.DateTime fileDate)
		{
			if (string.IsNullOrEmpty (fileName))
			{
				return null;
			}
			if (fileDate == System.DateTime.MinValue)
			{
				return null;
			}

			return this.FindItem (ImageCache.GetImageKeyName (fileName, fileDate));
		}

		private Item Find(string fileName)
		{
			string key = string.Concat (fileName.ToLowerInvariant (), "|");

			foreach (KeyValuePair<string, Item> pair in this.items)
			{
				if (pair.Key.StartsWith (key))
				{
					return pair.Value;
				}
			}

			GlobalImageCache.Item global = GlobalImageCache.FindStartingWith (key);

			if (global != null)
			{
				Item item = new Item (this, global, this.preferLowres);
				this.items.Add (global.KeyName, item);
				return item;
			}
			else
			{
				return null;
			}
		}

		private Item FindItem(string key)
		{
			//	Retourne les données d'une image, pour autant que celle-ci soit déjà connue.
			Item item = null;
			
			if (this.items.TryGetValue(key, out item))
			{
				//	Image déjà dans le cache !
			}
			else
			{
				GlobalImageCache.Item gItem = GlobalImageCache.Find(key);
				if (gItem != null)  // image dans le cache global ?
				{
					item = new Item(this, gItem, this.preferLowres);
					this.items.Add(key, item);  // ajoute l'image dans le cache local
				}
			}

			if (item != null)  // image trouvée ?
			{
				item.GlobalItem.SetRecentTimeStamp();  // le plus récent
				GlobalImageCache.FreeOldest();  // libère éventuellement des antiquités
			}

			return item;
		}

		public bool Contains(string filename)
		{
			//	TODO: Valider avec la date
			//	Vérifie si une image est en cache.
			return this.items.ContainsKey(filename.ToLowerInvariant ());
		}

		protected Item Add(string filename, System.DateTime dateTime)
		{
			return this.Add (filename, null, null, null, dateTime);
		}

		protected Item Add(string filename, string zipFilename, string zipShortName, byte[] data, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.
			//	Si les données 'data' n'existent pas, l'image est lue sur disque.
			//	Si les données existent, l'image est lue à partir des données en mémoire.
			
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (filename) == false);
			string key = ImageCache.GetImageKeyName (filename, date);

			System.Diagnostics.Debug.Assert(date != System.DateTime.MinValue);

			Item item;

			if (this.items.TryGetValue (key, out item))
			{
				//	OK, l'élément est déjà dans notre cache local.
			}
			else
			{
				GlobalImageCache.Item gItem = GlobalImageCache.Add(key, filename, zipFilename, zipShortName, data, date);
				if (gItem == null)
				{
					item = null;
				}
				else
				{
					item = new Item(this, gItem, this.preferLowres);
					this.items.Add(key, item);
				}
			}
			
			return item;
		}

		public void Clear()
		{
			//	Supprime toutes les images du cache.

			Item[] items = new Item[this.items.Count];
			this.items.Values.CopyTo (items, 0);

			foreach (Item item in items)
			{
				GlobalImageCache.Remove(item.GlobalItem);
				item.Dispose();
			}

			this.items.Clear();
		}

		public void ResetUsedFlags()
		{
			foreach (Item item in this.items.Values)
			{
				item.UseFlag = false;
			}
		}

		public void SetUsedFlag(string fileName, System.DateTime fileDate)
		{
			if (string.IsNullOrEmpty (fileName))
			{
				return;
			}
			
			string key = ImageCache.GetImageKeyName (fileName, fileDate);
			this.items[key].UseFlag = true;
		}

		public void FlushUnused()
		{
			//	Supprime toutes les images inutilisées du cache des images.
			List<string> keysToDelete = new List<string> ();

			foreach (KeyValuePair<string, Item> pair in this.items)
			{
				if (pair.Value.UseFlag == false)
				{
					keysToDelete.Add (pair.Key);
				}
			}

			foreach (string key in keysToDelete)
			{
				Item item = this.items[key];
				this.items.Remove (key);
				item.Dispose ();
			}
		}

		public void ClearInsideDoc()
		{
			//	Enlève tous les modes InsideDoc.
			foreach (Item item in this.items.Values)
			{
				item.InsideDoc = false;
			}
		}

		public void GenerateShortNames()
		{
			//	Génère les noms courts pour toutes les images du document.
			foreach (Item item in this.items.Values)
			{
				item.ShortName = null;
			}

			foreach (Item item in this.items.Values)
			{
				string shortName = System.IO.Path.GetFileName(item.Filename);

				if (this.IsExistingShortName(shortName))  // nom déjà utilisé ?
				{
					string name = System.IO.Path.GetFileNameWithoutExtension(item.Filename);
					string ext  = System.IO.Path.GetExtension(item.Filename);  // extension avec le "." !

					int i=2;  // commence avec 'nom (2).ext'
					do
					{
						//	Génère un mom du style 'nom (n).ext'.
						shortName = string.Format("{0} ({1}){2}", name, i.ToString(System.Globalization.CultureInfo.InvariantCulture), ext);
						i++;
						System.Diagnostics.Debug.Assert(i < 10000);  // faut pas pousser...
					}
					while (this.IsExistingShortName(shortName));
				}

				item.ShortName = shortName;
			}
		}

		protected bool IsExistingShortName(string shortName)
		{
			//	Vérifie si un nom court existe.
			foreach (Item item in this.items.Values)
			{
				if (item.ShortName == shortName)
				{
					return true;
				}
			}

			return false;
		}


		public void WriteData(ZipFile zip, Document.ImageIncludeMode imageIncludeMode)
		{
			//	Ecrit toutes les données des images.
			if (imageIncludeMode == Document.ImageIncludeMode.None)
			{
				return;
			}

			foreach (Item item in this.items.Values)
			{
				if (item.InsideDoc || imageIncludeMode == Document.ImageIncludeMode.All)
				{
					string name = string.Format("images/{0}", item.ShortName);
					zip.AddEntry(name, item.GlobalItem.Data);
				}
			}
		}

		public void ReadData(string zipFilename, ZipFile zip, Document.ImageIncludeMode imageIncludeMode, Properties.Image propImage)
		{
			//	Lit les données d'une image.
			if (!this.Contains(propImage.FileName))  // pas encore dans le cache ?
			{
				bool isZip = false;

				if (imageIncludeMode == Document.ImageIncludeMode.All)
				{
					isZip = true;
				}

				if (imageIncludeMode == Document.ImageIncludeMode.Defined)
				{
					isZip = propImage.InsideDoc;
				}

				if (isZip)
				{
					string name = string.Format("images/{0}", propImage.ShortName);
					string path = ImageCache.CreateZipPath (zipFilename, name);
					byte[] data = zip[name].Data;  // lit les données dans le fichier zip

					if (data != null)
					{
						this.Add(propImage.FileName, zipFilename, propImage.ShortName, data, propImage.FileDate);
						return;
					}
				}

				string fileName = propImage.FileName;
				System.DateTime fileDate = GlobalImageCache.GetFileDate (propImage.FileName);
				Item item = this.Add(fileName, fileDate);  // lit le fichier image sur disque
				
				if (item != null)
				{
					propImage.FileDate = fileDate;
				}
			}
		}

		private static string CreateZipPath(string zipFileName, string zipEntryName)
		{
			System.Diagnostics.Debug.Assert (zipEntryName.Contains (":") == false);
			return string.Concat ("zip:", zipFileName, ":", zipEntryName);
		}

		private static bool ExtractZipPathNames(string zipPath, out string zipFileName, out string zipEntryName)
		{
			if (zipPath.StartsWith ("zip:"))
			{
				zipPath = zipPath.Substring (4);
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


		public static void Lock(string fileName, System.DateTime fileDate)
		{
			if (string.IsNullOrEmpty (fileName))
			{
				return;
			}
			
			GlobalImageCache.Lock (ImageCache.GetImageKeyName (fileName, fileDate));
		}

		public static void UnlockAll()
		{
			GlobalImageCache.UnlockAll ();
		}

		#region Class Item
		public class Item : System.IDisposable
		{
			internal Item(ImageCache cache, GlobalImageCache.Item gItem, bool isLowres)
			{
				this.cache = cache;
				this.gItem = gItem;
				this.isLowres = isLowres;
			}

			public string Filename
			{
				get
				{
					return this.gItem.Filename;
				}
			}

			public Size Size
			{
				get
				{
					return this.gItem.Size;
				}
			}

			public Drawing.Image Image
			{
				get
				{
					return this.gItem.Image(this.isLowres);
				}
			}

			public double Scale
			{
				get
				{
					return this.isLowres ? this.gItem.LowresScale : 1.0;
				}
			}

			public GlobalImageCache.Item GlobalItem
			{
				//	Item du cache statique global.
				get
				{
					return this.gItem;
				}
			}

			public bool IsLowres
			{
				//	Indique le type de l'image à laquelle on s'intéresse.
				get
				{
					return this.isLowres;
				}
				set
				{
					this.isLowres = value;
				}
			}

			public string ShortName
			{
				//	Nom court utilisé pour la sérialisation Zip.
				get
				{
					return this.shortName;
				}
				set
				{
					this.shortName = value;
				}
			}

			public bool InsideDoc
			{
				//	Image incorporée au fichier Zip ?
				get
				{
					return this.insideDoc;
				}
				set
				{
					this.insideDoc = value;
				}
			}

			public bool UseFlag
			{
				get
				{
					return this.useFlag;
				}
				set
				{
					this.useFlag = value;
				}
			}

			#region IDisposable Members
			public void Dispose()
			{
				if (this.gItem != null)
				{
					this.gItem = null;
				}
			}
			#endregion

			private ImageCache cache;
			private GlobalImageCache.Item		gItem;
			private string shortName;
			private bool insideDoc;
			private bool isLowres;
			private bool useFlag;
		}
		#endregion


		protected static Drawing.Image ResizeImage(Drawing.Image image, int dx, int dy)
		{
			//	Retourne une image redimensionnée.
			Graphics gfx = new Graphics();
			gfx.SetPixmapSize(dx, dy);
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(1, -1, 0, 0);

			gfx.ImageFilter = new ImageFilter(ImageFilteringMode.Bilinear);  // le plus rapide
			gfx.PaintImage(image, new Rectangle(0, 0, dx, dy));

			return Bitmap.FromPixmap(gfx.Pixmap) as Bitmap;
		}


		protected Dictionary<string, Item>		items;
		protected bool							preferLowres;
	}
}
