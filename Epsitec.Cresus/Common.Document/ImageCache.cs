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
	public sealed class ImageCache
	{
		public ImageCache()
		{
			this.items = new Dictionary<string, Item> ();
			this.preferLowRes = true;
		}


		public bool PreferLowResImages
		{
			//	Indique le type des images auxquelles on s'intéresse.
			get
			{
				return this.preferLowRes;
			}

			set
			{
				if (this.preferLowRes != value)
				{
					this.preferLowRes = value;

					//	Informe tout le cache.
					foreach (Item item in this.items.Values)
					{
						item.UseLowRes = this.preferLowRes;
					}
				}
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
					return item.GlobalItem.FileDate;
				}
				else
				{
					return System.DateTime.MinValue;
				}
			}
		}

		public Item Find(string fileName, System.DateTime fileDate)
		{
			//	Cherche l'élément correspondant exactement au nom et à la date
			//	spécifiée. Cherche au besoin aussi dans le cache global.

			if (string.IsNullOrEmpty (fileName))
			{
				return null;
			}
			if (fileDate == System.DateTime.MinValue)
			{
				return null;
			}

			return this.FindItem (ImageCache.CreateKeyName (fileName, fileDate));
		}

		public void Clear()
		{
			//	Supprime toutes les images du cache.
			this.items.Clear ();
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

			string key = ImageCache.CreateKeyName (fileName, fileDate);
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
				this.items.Remove (key);
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
				string shortName = System.IO.Path.GetFileName (item.FileName);

				if (this.ContainsShortName (shortName))  // nom déjà utilisé ?
				{
					string name = System.IO.Path.GetFileNameWithoutExtension (item.FileName);
					string ext  = System.IO.Path.GetExtension (item.FileName);  // extension avec le "." !

					int i=2;  // commence avec 'nom (2).ext'
					do
					{
						//	Génère un mom du style 'nom (n).ext'.
						shortName = string.Format ("{0} ({1}){2}", name, i.ToString (System.Globalization.CultureInfo.InvariantCulture), ext);
						i++;
						System.Diagnostics.Debug.Assert (i < 10000);  // faut pas pousser...
					}
					while (this.ContainsShortName (shortName));
				}

				item.ShortName = shortName;
			}
		}

		public void SyncImageProperty(Properties.Image propImage)
		{
			string fileName = propImage.FileName;
			System.DateTime fileDate = propImage.FileDate;
			
			Item item = this.Find (fileName, fileDate);

			if (item != null)
			{
				propImage.ShortName = item.ShortName;
				item.InsideDoc = propImage.InsideDoc;
			}
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
					string name = string.Format ("images/{0}", item.ShortName);
					zip.AddEntry (name, item.GlobalItem.Data);
				}
			}
		}

		public void ReadData(ZipFile zip, Document.ImageIncludeMode imageIncludeMode, Properties.Image propImage)
		{
			//	Lit les données d'une image.
			if (!this.Contains (propImage.FileName, propImage.FileDate))  // pas encore dans le cache ?
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
					string name = string.Format ("images/{0}", propImage.ShortName);
					string path = ImageCache.CreateZipPath (zip.LoadFileName, name);
					this.Add (propImage.FileName, path, propImage.FileDate);
					return;
				}

				string fileName = propImage.FileName;
				System.DateTime fileDate = GlobalImageCache.GetFileDate (propImage.FileName);
				Item item = this.Add (fileName, fileDate);  // lit le fichier image sur disque

				if (item != null)
				{
					propImage.FileDate = fileDate;
				}
			}
		}

		public static string CreateKeyName(string fileName, System.DateTime fileDate)
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

		public static string CreateZipPath(string zipFileName, string zipEntryName)
		{
			System.Diagnostics.Debug.Assert (zipEntryName.Contains (":") == false);
			return string.Concat ("zip:", zipFileName, ":", zipEntryName);
		}

		public static bool ExtractZipPathNames(string zipPath, out string zipFileName, out string zipEntryName)
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

			GlobalImageCache.Lock (ImageCache.CreateKeyName (fileName, fileDate));
		}

		public static void UnlockAll()
		{
			GlobalImageCache.UnlockAll ();
		}

		#region Item Class
		public sealed class Item
		{
			public Item(ImageCache cache, GlobalImageCache.Item globalItem)
			{
				this.cache = cache;
				this.globalItem = globalItem;
				this.useLowRes = this.cache.preferLowRes;
			}

			public string FileName
			{
				get
				{
					return this.globalItem.FileName;
				}
			}

			public Size Size
			{
				get
				{
					return this.globalItem.Size;
				}
			}

			public Drawing.Image Image
			{
				get
				{
					return this.globalItem.Image (this.useLowRes);
				}
			}

			public double Scale
			{
				get
				{
					return this.useLowRes ? this.globalItem.LowResScale : 1.0;
				}
			}

			public GlobalImageCache.Item GlobalItem
			{
				//	Item du cache statique global.
				get
				{
					return this.globalItem;
				}
			}

			public bool UseLowRes
			{
				//	Indique le type de l'image à laquelle on s'intéresse.
				get
				{
					return this.useLowRes;
				}
				set
				{
					this.useLowRes = value;
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

			private ImageCache cache;
			private GlobalImageCache.Item globalItem;
			private string shortName;
			private bool insideDoc;
			private bool useLowRes;
			private bool useFlag;
		}
		#endregion

		private Item Find(string fileName)
		{
			//	Cherche l'élément en se basant uniquement sur le nom. On prend
			//	le premier qui correspond (dans le cache local ou dans le cache
			//	global).

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
				key = global.KeyName;
				Item item = new Item (this, global);
				this.items.Add (key, item);
				return item;
			}
			else
			{
				return null;
			}
		}

		private Item FindItem(string key)
		{
			//	Retourne les données d'une image, pour autant que celle-ci soit déjà connue
			//	dans l'un des caches (local ou global).

			Item item = null;

			if (this.items.TryGetValue (key, out item))
			{
				//	Image déjà dans le cache !
			}
			else
			{
				GlobalImageCache.Item global = GlobalImageCache.Find (key);

				if (global != null)  // image dans le cache global ?
				{
					item = new Item (this, global);
					this.items.Add (key, item);  // ajoute l'image dans le cache local
				}
			}

			if (item != null)  // image trouvée ?
			{
				item.GlobalItem.SetRecentTimeStamp ();  // le plus récent
				GlobalImageCache.FreeOldest ();  // libère éventuellement des antiquités
			}

			return item;
		}

		private bool Contains(string fileName, System.DateTime fileDate)
		{
			//	Vérifie si une image est en cache.

			if (string.IsNullOrEmpty (fileName))
			{
				return false;
			}
			else
			{
				return this.items.ContainsKey (ImageCache.CreateKeyName (fileName, fileDate));
			}
		}

		private bool ContainsShortName(string shortName)
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

		private Item Add(string filename, System.DateTime dateTime)
		{
			return this.Add (filename, null, dateTime);
		}

		private Item Add(string filename, string zipPath, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.

			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (filename) == false);
			string key = ImageCache.CreateKeyName (filename, date);

			System.Diagnostics.Debug.Assert (date != System.DateTime.MinValue);

			Item item;

			if (this.items.TryGetValue (key, out item))
			{
				//	OK, l'élément est déjà dans notre cache local.
			}
			else
			{
				GlobalImageCache.Item global = GlobalImageCache.Add (key, filename, zipPath, date);

				if (global != null)
				{
					item = new Item (this, global);
					this.items.Add (key, item);
				}
			}

			return item;
		}


		private Dictionary<string, Item> items;
		private bool preferLowRes;
	}
}
