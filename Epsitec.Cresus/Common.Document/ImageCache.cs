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

		public System.DateTime Load(string filename)
		{
			//	Essaie de charger une image dans le cache.
			//	Celle méthode est appelée chaque fois que le nom édité de l'image est changé.
			if (string.IsNullOrEmpty (filename))
			{
				return System.DateTime.MinValue;
			}
			else
			{
				Item item = this.Get(filename);

				if (item == null)
				{
					item = this.Add(filename);
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

		public Item Get(string filename)
		{
			//	Retourne les données d'une image.
			if (string.IsNullOrEmpty(filename))
			{
				return null;
			}

			Item item = null;
			
			filename = filename.ToLowerInvariant ();

			if (this.items.TryGetValue(filename, out item))
			{
				//	Image déjà dans le cache !
			}
			else
			{
				GlobalImageCache.Item gItem = GlobalImageCache.Get(filename);
				if (gItem != null)  // image dans le cache global ?
				{
					item = new Item(gItem, this.preferLowres);
					this.items.Add(filename, item);  // ajoute l'image dans le cache local
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
			//	Vérifie si une image est en cache.
			return this.items.ContainsKey(filename.ToLowerInvariant ());
		}

		protected Item Add(string filename)
		{
			return this.Add (filename, null, null, null, System.DateTime.MinValue);
		}

		protected Item Add(string filename, string zipFilename, string zipShortName, byte[] data, System.DateTime date)
		{
			//	Ajoute une nouvelle image dans le cache.
			//	Si les données 'data' n'existent pas, l'image est lue sur disque.
			//	Si les données existent, l'image est lue à partir des données en mémoire.
			
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (filename) == false);
			filename = filename.ToLowerInvariant ();

			if (data == null)
			{
				System.Diagnostics.Debug.Assert(date == System.DateTime.MinValue);
			}
			else
			{
				System.Diagnostics.Debug.Assert(date != System.DateTime.MinValue);
			}

			Item item;

			if (this.items.TryGetValue (filename, out item))
			{
				//	OK, l'élément est déjà dans notre cache local.
			}
			else
			{
				GlobalImageCache.Item gItem = GlobalImageCache.Add(filename, zipFilename, zipShortName, data, date);
				if (gItem == null || !gItem.IsData)
				{
					item = null;
				}
				else
				{
					item = new Item(gItem, this.preferLowres);
					this.items.Add(filename, item);
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
				GlobalImageCache.Remove(item.GlobalItem.Filename);
				item.Dispose();
			}

			this.items.Clear();
		}


		public void FlushUnused(IList<string> usedFilenames)
		{
			//	Supprime toutes les images inutilisées du cache des images, c'est-à-dire
			//	dont le nom de fichier n'est pas dans la liste usedFilenames.
			List<string> keysToDelete = new List<string>();
			List<string> filenames = new List<string> ();

			foreach (string filename in usedFilenames)
			{
				filenames.Add (filename.ToLowerInvariant ());
			}

			foreach (string key in this.items.Keys)
			{
				if (!filenames.Contains(key))
				{
					keysToDelete.Add(key);
					filenames.Remove (key);		//	pour éviter de chercher deux fois le même nom
				}
			}

			foreach (string key in keysToDelete)
			{
				Item item = this.items[key];
				this.items.Remove(key);
				item.Dispose();
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
			if (!this.Contains(propImage.Filename))  // pas encore dans le cache ?
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
					byte[] data = zip[name].Data;  // lit les données dans le fichier zip

					if (data != null)
					{
						this.Add(propImage.Filename, zipFilename, propImage.ShortName, data, propImage.Date);
						return;
					}
				}

				Item item = this.Add(propImage.Filename);  // lit le fichier image sur disque
				if (item != null)
				{
					propImage.Date = item.GlobalItem.Date;
				}
			}
		}


		public static void Lock(IList<string> list)
		{
			GlobalImageCache.UnlockAll ();

			foreach (string name in list)
			{
				GlobalImageCache.Lock (name.ToLowerInvariant ());
			}
		}

		public static void UnlockAll()
		{
			GlobalImageCache.UnlockAll ();
		}

		#region Class Item
		public class Item : System.IDisposable
		{
			internal Item(GlobalImageCache.Item gItem, bool isLowres)
			{
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

			#region IDisposable Members
			public void Dispose()
			{
				if (this.gItem != null)
				{
					this.gItem = null;
				}
			}
			#endregion

			protected GlobalImageCache.Item		gItem;			
			protected string					shortName;
			protected bool						insideDoc;
			protected bool						isLowres;
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
