using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.IO;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// L'énumération <c>ImageCacheResolution</c> définit les résolutions utiles
	/// pour une image.
	/// </summary>
	public enum ImageCacheResolution
	{
		Low,	//	image réduite, environ 512x512 pixels
		High	//	image non réduite, résolution maximale
	}

	/// <summary>
	/// La classe <c>ImageCache</c> gère un cache par document des images.
	/// </summary>
	public sealed class ImageCache
	{
		public ImageCache(Document document)
		{
			this.document = document;
			this.items = new Dictionary<string, Item>();
			this.resolution = ImageCacheResolution.Low;
			GlobalImageCache.Register(this);
		}

		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}
		
		public void SetResolution(ImageCacheResolution resolution)
		{
			//	Indique à quelle résolution d'images on s'intéresse.

			if (this.resolution != resolution)
			{
				this.resolution = resolution;

				//	Informe toutes les images du cache.
				foreach (Item item in this.items.Values)
				{
					item.Resolution = this.resolution;
				}
			}
		}

		public System.DateTime LoadFromFile(string fileName)
		{
			//	Essaie de charger une image dans le cache, à partir d'un fichier réel.
			//	Celle méthode est appelée chaque fois que le nom édité de l'image est changé.
			if (string.IsNullOrEmpty(fileName))
			{
				return System.DateTime.MinValue;
			}
			else
			{
				Item item;
				System.DateTime fileDate = GlobalImageCache.GetFileDate(fileName);

				if (fileDate.Ticks == 0)
				{
					//	Aucun fichier n'existe sur le disque, mais peut-être que le nom de
					//	ce fichier est connu quand-même par le cache, parce que le fichier
					//	fait partie du document...
					item = this.Find(fileName);
				}
				else
				{
					//	Le fichier a été trouvé sur le disque; peut-être qu'il existe déjà
					//	une entrée dans le cache pour ce fichier ? On vérifie en se basant
					//	aussi sur la date du fichier, car on peut avoir un document avec
					//	plusieurs versions du même fichier :
					item = this.Find(fileName, fileDate);

					if (item == null)
					{
						item = this.Add(fileName, fileDate);
					}
				}

				if (item != null)
				{
					return item.GetFileDate();
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
			//	spécifiée. Cherche au besoin aussi dans le cache global. Ne charge
			//	jamais d'image si elle n'est pas trouvée dans le cache.
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			if (fileDate.Ticks == 0)
			{
				return null;
			}

			return this.FindItem(ImageManager.GetKey(fileName, fileDate));
		}

		public void Dispose()
		{
			//	Supprime toutes les images du cache.
			DocumentManager manager = this.Document == null ? null : this.Document.DocumentManager;
			string zipFilePath = manager == null ? null : manager.GetLocalFilePath();

			if (!string.IsNullOrEmpty(zipFilePath))
			{
				GlobalImageCache.RemoveReferenceToZip(zipFilePath);
			}

			GlobalImageCache.Unregister(this);
			this.items.Clear();
		}

		public void ResetUsedFlags()
		{
			//	Pour toutes les images trouvées dans le cache, remet à zéro le
			//	marqueur d'utilisation; à combiner avec des appels à SetUsedFlag.
			foreach (Item item in this.items.Values)
			{
				item.UsedInDocument = false;
			}
		}

		public void SetUsedFlag(string fileName, System.DateTime fileDate)
		{
			//	Définit qu'une image est utilisée par le document auquel ce cache
			//	est associé.
			if (string.IsNullOrEmpty(fileName))
			{
				return;
			}

			string key = ImageManager.GetKey(fileName, fileDate);

			if (this.items.ContainsKey(key))
			{
				this.items[key].UsedInDocument = true;
			}
		}

		public void FlushUnused()
		{
			//	Supprime toutes les images inutilisées du cache des images. Appeler
			//	ResetUsedFlags suivi de n x SetUsedFlag pour mettre à jour les marqueurs
			//	avant...
			List<string> keysToDelete = new List<string>();

			foreach (KeyValuePair<string, Item> pair in this.items)
			{
				if (pair.Value.UsedInDocument == false)
				{
					keysToDelete.Add(pair.Key);
				}
			}

			foreach (string key in keysToDelete)
			{
				this.items.Remove(key);
			}
		}

		public void ClearEmbeddedInDocument()
		{
			//	Enlève tous les modes EmbeddedInDocument.
			foreach (Item item in this.items.Values)
			{
				item.EmbeddedInDocument = false;
			}
		}

		public void GenerateShortNames()
		{
			//	Génère les noms courts pour toutes les images du document. Chaque
			//	image reçoit un nom unique qui permet de les distinguer dans le ZIP
			//	contenant le document et ses images.
			foreach (Item item in this.items.Values)
			{
				item.ShortName = null;
			}

			foreach (Item item in this.items.Values)
			{
				string shortName = System.IO.Path.GetFileName(item.FileName);

				if (this.IsShortNameUsed(shortName))  // nom déjà utilisé ?
				{
					string name = System.IO.Path.GetFileNameWithoutExtension(item.FileName);
					string ext  = System.IO.Path.GetExtension(item.FileName);  // extension avec le "." !

					int i=2;  // commence avec 'nom (2).ext'
					do
					{
						//	Génère un mom du style 'nom (n).ext'.
						shortName = string.Format("{0} ({1}){2}", name, i.ToString(System.Globalization.CultureInfo.InvariantCulture), ext);
						i++;
						System.Diagnostics.Debug.Assert(i < 10000);  // faut pas pousser...
					}
					while (this.IsShortNameUsed(shortName));
				}

				item.ShortName = shortName;
			}
		}

		public void SyncImageProperty(Properties.Image propImage)
		{
			//	Synchronise la propriété de l'image avec les informations contenues
			//	dans le cache à propos de celle-ci.
			string fileName = propImage.FileName;
			System.DateTime fileDate = propImage.FileDate;

			Item item = this.Find(fileName, fileDate);

			if (item != null)
			{
				propImage.ShortName = item.ShortName;
				item.EmbeddedInDocument = propImage.InsideDoc;
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
				if (item.EmbeddedInDocument || imageIncludeMode == Document.ImageIncludeMode.All)
				{
					string name = string.Format("images/{0}", item.ShortName);
					zip.AddEntry(name, item.GetImageData(), false);
				}
			}
		}

		public void ReadData(ZipFile zip, Document.ImageIncludeMode imageIncludeMode, Properties.Image propImage)
		{
			//	Lit les données d'une image : ceci se contente de créer un item
			//	dans le cache mais ne lit aucune donnée, ni depuis le disque, ni
			//	depuis le document ZIP.
			if (string.IsNullOrEmpty(propImage.FileName))
			{
				return;
			}

			if (!this.Contains(propImage.FileName, propImage.FileDate))  // pas encore dans le cache ?
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
					string path = GlobalImageCache.CreateZipPath(zip.LoadFileName, name);
					this.Add(propImage.FileName, path, propImage.FileDate);
				}
				else
				{
					string fileName = propImage.FileName;
					System.DateTime fileDate = GlobalImageCache.GetFileDate(propImage.FileName);
					
					if (fileDate.Ticks > 0)
					{
						Item item = this.Add(fileName, fileDate);  // lit le fichier image sur disque

						if (item != null)
						{
							propImage.FileDate = fileDate;
						}
					}
				}
			}
		}

		public void Lock(string fileName, System.DateTime fileDate)
		{
			//	Verrouille l'image - elle est en cours d'utilisation dans la
			//	page courante.
			if (string.IsNullOrEmpty(fileName))
			{
				return;
			}

			GlobalImageCache.Lock(ImageManager.GetKey(fileName, fileDate), null);
			GlobalImageCache.Lock(ImageManager.GetKey(fileName, fileDate), this.GetWorkingDocumentFileName());
		}

		public void UnlockAll()
		{
			//	Déverouille toutes les images. Elles peuvent être recyclées
			//	s'il n'y a plus assez de mémoire.
			GlobalImageCache.UnlockAll();
		}

		private string GetWorkingDocumentFileName()
		{
			if (this.document == null || this.document.DocumentManager == null)
			{
				return null;
			}
			else
			{
				return this.document.DocumentManager.GetLocalFilePath();
			}
		}

		private Item Find(string fileName)
		{
			//	Retourne les données d'une image en se basant uniquement sur le
			//	nom. On prend le premier qui correspond (dans le cache local ou
			//	dans le cache global).
			string key = ImageManager.GetKeyPrefix(fileName);
			string doc = this.GetWorkingDocumentFileName();

			foreach (KeyValuePair<string, Item> pair in this.items)
			{
				if (pair.Key.StartsWith(key))
				{
					return pair.Value;
				}
			}

			GlobalImageCache.Item global = GlobalImageCache.FindStartingWith(key, doc);

			if (global != null)
			{
				key = global.LocalKeyName;
				Item item = new Item(global, this.resolution);
				this.items.Add(key, item);
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

			if (this.items.TryGetValue(key, out item))
			{
				//	Image déjà dans le cache !
			}
			else
			{
				GlobalImageCache.Item global = GlobalImageCache.Find(key, this.GetWorkingDocumentFileName());

				if (global == null)
				{
					global = GlobalImageCache.Find(key, null);
				}

				if (global != null)  // image dans le cache global ?
				{
					item = new Item(global, this.resolution);
					this.items.Add(key, item);  // ajoute l'image dans le cache local
				}
			}

			if (item != null)  // image trouvée ?
			{
				item.SetRecentTimeStamp();  // le plus récent
				GlobalImageCache.FreeOldest();  // libère éventuellement des antiquités
			}

			return item;
		}

		private bool Contains(string fileName, System.DateTime fileDate)
		{
			//	Vérifie si une image est en cache.
			if (string.IsNullOrEmpty(fileName))
			{
				return false;
			}
			else
			{
				return this.items.ContainsKey(ImageManager.GetKey(fileName, fileDate));
			}
		}

		private bool IsShortNameUsed(string shortName)
		{
			//	Vérifie si un nom court donné est déjà utilisé.
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
			//	Ajoute une nouvelle image dans le cache.
			return this.Add(filename, null, dateTime);
		}

		private Item Add(string filename, string zipPath, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.
			System.Diagnostics.Debug.Assert(string.IsNullOrEmpty(filename) == false);
			System.Diagnostics.Debug.Assert(date.Ticks > 0);

			string key = ImageManager.GetKey(filename, date);
			Item item;

			if (this.items.TryGetValue(key, out item))
			{
				//	OK, l'élément est déjà dans notre cache local.
			}
			else
			{
				GlobalImageCache.Item global = GlobalImageCache.Add(key, filename, zipPath, date);

				if (global != null)
				{
					item = new Item(global, this.resolution);
					this.items.Add(key, item);
				}
			}

			return item;
		}


		#region Item Class
		public sealed class Item
		{
			internal Item(GlobalImageCache.Item globalItem, ImageCacheResolution resolution)
			{
				this.globalItem = globalItem;
				this.resolution = resolution;
			}

			public string FileName
			{
				get
				{
					return this.globalItem.FileName;
				}
			}

			public Drawing.Size Size
			{
				get
				{
					Drawing.Size size = this.globalItem.Size;

					if (size == Drawing.Size.Zero)
					{
						this.globalItem.GetImage(this.resolution, true);
						size = this.globalItem.Size;
					}

					return size;
				}
			}

			public Drawing.Image Image
			{
				get
				{
					this.SetRecentTimeStamp();
					GlobalImageCache.FreeOldest();
					return this.globalItem.GetImage(this.resolution, true);
				}
			}

			public Drawing.Image CachedImage
			{
				get
				{
					return this.globalItem.GetImage(this.resolution, false);
				}
			}

			public double Scale
			{
				get
				{
					switch (this.resolution)
					{
						case ImageCacheResolution.Low:
							return this.globalItem.LowResScale;

						case ImageCacheResolution.High:
							return 1.0;
					}

					throw new System.InvalidOperationException();
				}
			}

			public ImageCacheResolution Resolution
			{
				//	Indique la résolution de l'image à laquelle on s'intéresse.
				get
				{
					return this.resolution;
				}
				set
				{
					this.resolution = value;
				}
			}

			public string ShortName
			{
				//	Nom court utilisé pour la sérialisation ZIP.
				get
				{
					return this.shortName;
				}
				set
				{
					this.shortName = value;
				}
			}

			public bool EmbeddedInDocument
			{
				//	Image incorporée au document (fichier ZIP) ?
				get
				{
					return this.embeddedInDocument;
				}
				set
				{
					this.embeddedInDocument = value;
				}
			}

			public bool UsedInDocument
			{
				//	Image utilisée par le document associé au cache ?
				get
				{
					return this.usedInDocument;
				}
				set
				{
					this.usedInDocument = value;
				}
			}

			internal byte[] GetImageData()
			{
				return this.globalItem.GetImageData();
			}

			internal byte[] GetImageDataBypassingCache()
			{
				return this.globalItem.LowLeveReadImageData ();
			}

			internal bool ReloadImage()
			{
				return this.globalItem.Reload();
			}

			internal void FreeImage()
			{
				this.globalItem.Free(GlobalImageCache.ImagePart.LargeOriginal);
				this.globalItem.Free(GlobalImageCache.ImagePart.Data);
			}

			internal void ExportImage(string path)
			{
				byte[] imageData = this.GetImageData();
				
				if (imageData != null)
				{
					System.IO.File.WriteAllBytes(path, imageData);
				}
			}

			internal System.DateTime GetFileDate()
			{
				return this.globalItem.FileDate;
			}

			internal void SetRecentTimeStamp()
			{
				this.globalItem.SetRecentTimeStamp();
			}
			
			private GlobalImageCache.Item		globalItem;
			private ImageCacheResolution		resolution;
			private string						shortName;
			private bool						embeddedInDocument;
			private bool						usedInDocument;
		}
		#endregion


		private Document						document;
		private Dictionary<string, Item>		items;
		private ImageCacheResolution			resolution;
	}
}
