using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.IO;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// L'�num�ration <c>ImageCacheResolution</c> d�finit les r�solutions utiles
	/// pour une image.
	/// </summary>
	public enum ImageCacheResolution
	{
		Low,	//	image r�duite, environ 512x512 pixels
		High	//	image non r�duite, r�solution maximale
	}

	/// <summary>
	/// La classe <c>ImageCache</c> g�re un cache par document des images.
	/// </summary>
	public sealed class ImageCache
	{
		public ImageCache(Document document)
		{
			this.document = document;
			this.items = new Dictionary<string, Item> ();
			this.resolution = ImageCacheResolution.Low;
			GlobalImageCache.Register (this);
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
			//	Indique � quelle r�solution d'images on s'int�resse.

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
			//	Essaie de charger une image dans le cache, � partir d'un fichier r�el.
			//	Celle m�thode est appel�e chaque fois que le nom �dit� de l'image est chang�.
			if (string.IsNullOrEmpty (fileName))
			{
				return System.DateTime.MinValue;
			}
			else
			{
				Item item;
				System.DateTime fileDate = ImageCache.GetFileDate (fileName);

				if (fileDate == System.DateTime.MinValue)
				{
					//	Aucun fichier n'existe sur le disque, mais peut-�tre que le nom de
					//	ce fichier est connu quand-m�me par le cache, parce que le fichier
					//	fait partie du document...

					item = this.Find (fileName);
				}
				else
				{
					//	Le fichier a �t� trouv� sur le disque; peut-�tre qu'il existe d�j�
					//	une entr�e dans le cache pour ce fichier ? On v�rifie en se basant
					//	aussi sur la date du fichier, car on peut avoir un document avec
					//	plusieurs versions du m�me fichier :

					item = this.Find (fileName, fileDate);

					if (item == null)
					{
						item = this.Add (fileName, fileDate);
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
			//	Cherche l'�l�ment correspondant exactement au nom et � la date
			//	sp�cifi�e. Cherche au besoin aussi dans le cache global. Ne charge
			//	jamais d'image si elle n'est pas trouv�e dans le cache.

			if (string.IsNullOrEmpty (fileName))
			{
				return null;
			}
			if (fileDate == System.DateTime.MinValue)
			{
				return null;
			}

			return this.FindItem (Opac.ImageManager.Cache.GetKey (fileName, fileDate));
		}

		public void Dispose()
		{
			//	Supprime toutes les images du cache.
			DocumentManager manager = this.Document == null ? null : this.Document.DocumentManager;
			string zipFilePath = manager == null ? null : manager.GetLocalFilePath ();

			if (!string.IsNullOrEmpty (zipFilePath))
			{
				GlobalImageCache.RemoveReferenceToZip (zipFilePath);
			}

			GlobalImageCache.Unregister (this);
			this.items.Clear ();
		}

		public void ResetUsedFlags()
		{
			//	Pour toutes les images trouv�es dans le cache, remet � z�ro le
			//	marqueur d'utilisation; � combiner avec des appels � SetUsedFlag.
			foreach (Item item in this.items.Values)
			{
				item.UsedInDocument = false;
			}
		}

		public void SetUsedFlag(string fileName, System.DateTime fileDate)
		{
			//	D�finit qu'une image est utilis�e par le document auquel ce cache
			//	est associ�.
			if (string.IsNullOrEmpty (fileName))
			{
				return;
			}

			string key = Opac.ImageManager.Cache.GetKey (fileName, fileDate);
			this.items[key].UsedInDocument = true;
		}

		public void FlushUnused()
		{
			//	Supprime toutes les images inutilis�es du cache des images. Appeler
			//	ResetUsedFlags suivi de n x SetUsedFlag pour mettre � jour les marqueurs
			//	avant...
			List<string> keysToDelete = new List<string> ();

			foreach (KeyValuePair<string, Item> pair in this.items)
			{
				if (pair.Value.UsedInDocument == false)
				{
					keysToDelete.Add (pair.Key);
				}
			}

			foreach (string key in keysToDelete)
			{
				this.items.Remove (key);
			}
		}

		public void ClearEmbeddedInDocument()
		{
			//	Enl�ve tous les modes EmbeddedInDocument.
			foreach (Item item in this.items.Values)
			{
				item.EmbeddedInDocument = false;
			}
		}

		public void GenerateShortNames()
		{
			//	G�n�re les noms courts pour toutes les images du document. Chaque
			//	image re�oit un nom unique qui permet de les distinguer dans le ZIP
			//	contenant le document et ses images.
			foreach (Item item in this.items.Values)
			{
				item.ShortName = null;
			}

			foreach (Item item in this.items.Values)
			{
				string shortName = System.IO.Path.GetFileName (item.FileName);

				if (this.IsShortNameUsed (shortName))  // nom d�j� utilis� ?
				{
					string name = System.IO.Path.GetFileNameWithoutExtension (item.FileName);
					string ext  = System.IO.Path.GetExtension (item.FileName);  // extension avec le "." !

					int i=2;  // commence avec 'nom (2).ext'
					do
					{
						//	G�n�re un mom du style 'nom (n).ext'.
						shortName = string.Format ("{0} ({1}){2}", name, i.ToString (System.Globalization.CultureInfo.InvariantCulture), ext);
						i++;
						System.Diagnostics.Debug.Assert (i < 10000);  // faut pas pousser...
					}
					while (this.IsShortNameUsed (shortName));
				}

				item.ShortName = shortName;
			}
		}

		public void SyncImageProperty(Properties.Image propImage)
		{
			//	Synchronise la propri�t� de l'image avec les informations contenues
			//	dans le cache � propos de celle-ci.
			
			string fileName = propImage.FileName;
			System.DateTime fileDate = propImage.FileDate;
			
			Item item = this.Find (fileName, fileDate);

			if (item != null)
			{
				propImage.ShortName = item.ShortName;
				item.EmbeddedInDocument = propImage.InsideDoc;
			}
		}

		public void WriteData(ZipFile zip, Document.ImageIncludeMode imageIncludeMode)
		{
			//	Ecrit toutes les donn�es des images.
			if (imageIncludeMode == Document.ImageIncludeMode.None)
			{
				return;
			}

			foreach (Item item in this.items.Values)
			{
				if (item.EmbeddedInDocument || imageIncludeMode == Document.ImageIncludeMode.All)
				{
					string name = string.Format ("images/{0}", item.ShortName);
					zip.AddEntry (name, item.GetImageData (), false);
				}
			}
		}

		public void ReadData(ZipFile zip, Document.ImageIncludeMode imageIncludeMode, Properties.Image propImage)
		{
			//	Lit les donn�es d'une image : ceci se contente de cr�er un item
			//	dans le cache mais ne lit aucune donn�e, ni depuis le disque, ni
			//	depuis le document ZIP.

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
					string path = GlobalImageCache.CreateZipPath (zip.LoadFileName, name);
					this.Add (propImage.FileName, path, propImage.FileDate);
				}
				else
				{
					string fileName = propImage.FileName;
					System.DateTime fileDate = ImageCache.GetFileDate (propImage.FileName);
					Item item = this.Add (fileName, fileDate);  // lit le fichier image sur disque

					if (item != null)
					{
						propImage.FileDate = fileDate;
					}
				}
			}
		}

		public void Lock(string fileName, System.DateTime fileDate)
		{
			//	Verrouille l'image - elle est en cours d'utilisation dans la
			//	page courante.

			if (string.IsNullOrEmpty (fileName))
			{
				return;
			}

			GlobalImageCache.Lock (Opac.ImageManager.Cache.GetKey (fileName, fileDate), null);
			GlobalImageCache.Lock (Opac.ImageManager.Cache.GetKey (fileName, fileDate), this.GetWorkingDocumentFileName ());
		}

		public void UnlockAll()
		{
			//	D�verouille toutes les images. Elles peuvent �tre recycl�es
			//	s'il n'y a plus assez de m�moire.

			GlobalImageCache.UnlockAll ();
		}

		#region Item Class
		
		public sealed class Item
		{
			internal Item(ImageCache cache, GlobalImageCache.Item globalItem)
			{
				this.cache = cache;
				this.globalItem = globalItem;
				this.resolution = this.cache.resolution;
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
					return this.globalItem.Size;
				}
			}

			public Drawing.Image Image
			{
				get
				{
					return this.globalItem.GetImage (this.resolution);
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

					throw new System.InvalidOperationException ();
				}
			}

			public ImageCacheResolution Resolution
			{
				//	Indique la r�solution de l'image � laquelle on s'int�resse.
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
				//	Nom court utilis� pour la s�rialisation ZIP.
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
				//	Image incorpor�e au document (fichier ZIP) ?
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
				//	Image utilis�e par le document associ� au cache ?
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
				return this.globalItem.GetImageData ();
			}

			internal bool ReloadImage()
			{
				return this.globalItem.Reload ();
			}

			internal void ExportImage(string path)
			{
				byte[] imageData = this.GetImageData ();
				
				if (imageData != null)
				{
					System.IO.File.WriteAllBytes (path, imageData);
				}
			}

			internal System.DateTime GetFileDate()
			{
				return this.globalItem.FileDate;
			}

			internal void SetRecentTimeStamp()
			{
				this.globalItem.SetRecentTimeStamp ();
			}
			
			private ImageCache					cache;
			private GlobalImageCache.Item		globalItem;
			private string						shortName;
			private ImageCacheResolution		resolution;
			private bool						embeddedInDocument;
			private bool						usedInDocument;
		}
		#endregion

		private string GetWorkingDocumentFileName()
		{
			if ((this.document == null) ||
				(this.document.DocumentManager == null))
			{
				return null;
			}
			else
			{
				return this.document.DocumentManager.GetLocalFilePath ();
			}
		}

		private Item Find(string fileName)
		{
			//	Retourne les donn�es d'une image en se basant uniquement sur le
			//	nom. On prend le premier qui correspond (dans le cache local ou
			//	dans le cache global).

			string key = Opac.ImageManager.Cache.GetKeyPrefix (fileName);
			string doc = this.GetWorkingDocumentFileName ();

			foreach (KeyValuePair<string, Item> pair in this.items)
			{
				if (pair.Key.StartsWith (key))
				{
					return pair.Value;
				}
			}

			GlobalImageCache.Item global = GlobalImageCache.FindStartingWith (key, doc);

			if (global != null)
			{
				key = global.LocalKeyName;
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
			//	Retourne les donn�es d'une image, pour autant que celle-ci soit d�j� connue
			//	dans l'un des caches (local ou global).

			Item item = null;

			if (this.items.TryGetValue (key, out item))
			{
				//	Image d�j� dans le cache !
			}
			else
			{
				GlobalImageCache.Item global = GlobalImageCache.Find (key, this.GetWorkingDocumentFileName ());

				if (global == null)
				{
					global = GlobalImageCache.Find (key, null);
				}

				if (global != null)  // image dans le cache global ?
				{
					item = new Item (this, global);
					this.items.Add (key, item);  // ajoute l'image dans le cache local
				}
			}

			if (item != null)  // image trouv�e ?
			{
				item.SetRecentTimeStamp ();  // le plus r�cent
				GlobalImageCache.FreeOldest ();  // lib�re �ventuellement des antiquit�s
			}

			return item;
		}

		private bool Contains(string fileName, System.DateTime fileDate)
		{
			//	V�rifie si une image est en cache.

			if (string.IsNullOrEmpty (fileName))
			{
				return false;
			}
			else
			{
				return this.items.ContainsKey (Opac.ImageManager.Cache.GetKey (fileName, fileDate));
			}
		}

		private bool IsShortNameUsed(string shortName)
		{
			//	V�rifie si un nom court donn� est d�j� utilis�.
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
			return this.Add (filename, null, dateTime);
		}

		private Item Add(string filename, string zipPath, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.

			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (filename) == false);
			System.Diagnostics.Debug.Assert (date != System.DateTime.MinValue);
			
			string key = Opac.ImageManager.Cache.GetKey (filename, date);
			Item item;

			if (this.items.TryGetValue (key, out item))
			{
				//	OK, l'�l�ment est d�j� dans notre cache local.
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

		private static System.DateTime GetFileDate(string path)
		{
			//	Retourne la date de derni�re modification d'un fichier.
			System.IO.FileInfo info = new System.IO.FileInfo (path);
			if (info.Exists)
			{
				return info.LastWriteTime;
			}
			else
			{
				return System.DateTime.MinValue;
			}
		}


		private Document						document;
		private Dictionary<string, Item>		items;
		private ImageCacheResolution			resolution;
	}
}
