using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.IO;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for ImageCache.
	/// </summary>
	public class ImageCache
	{
		public ImageCache()
		{
			this.dico = new Dictionary<string, Item>();
			this.isLowres = true;
		}


		public bool IsLowres
		{
			//	Indique le type des images auxquelles on s'intéresse.
			get
			{
				return this.isLowres;
			}

			set
			{
				if (this.isLowres != value)
				{
					this.isLowres = value;

					//	Informe tout le cache.
					foreach (Item item in this.dico.Values)
					{
						item.IsLowres = this.isLowres;
					}
				}
			}
		}

		public void Load(string filename)
		{
			//	Essaie de charger une image dans le cache.
			//	Celle méthode est appelée chaque fois que le nom édité de l'image est changé.
			if (!string.IsNullOrEmpty(filename))
			{
				Item item = this.Get(filename);

				if (item == null)
				{
					this.Add(filename, null);
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

			if (this.dico.ContainsKey(filename))  // image déjà dans le cache ?
			{
				item = this.dico[filename];
			}
			else
			{
				GlobalImageCache.Item gItem = GlobalImageCache.Get(filename);
				if (gItem != null)  // image dans le cache global ?
				{
					item = new Item(gItem, this.isLowres);
					this.dico.Add(filename, item);  // ajoute l'image dans le cache local
				}
			}

			if (item != null)  // image trouvé ?
			{
				item.GlobalItem.SetRecentTimeStamp();  // le plus récent
				GlobalImageCache.FreeOldest();  // libère éventuellement des antiquités
			}

			return item;
		}

		public bool Contains(string filename)
		{
			//	Vérifie si une image est en cache.
			return this.dico.ContainsKey(filename);
		}

		protected Item Add(string filename, byte[] data)
		{
			//	Ajoute une nouvelle image dans le cache.
			//	Si les données 'data' n'existent pas, l'image est lue sur disque.
			//	Si les données existent, l'image est lue à partir des données en mémoire.
			if (this.dico.ContainsKey(filename))
			{
				return this.dico[filename];
			}
			else
			{
				GlobalImageCache.Item gItem = GlobalImageCache.Add(filename, data);
				if (gItem.Data == null)
				{
					return null;
				}
				else
				{
					Item item = new Item(gItem, this.isLowres);
					this.dico.Add(filename, item);
					return item;
				}
			}
		}

		public void Clear()
		{
			//	Supprime toutes les images du cache.
			foreach (KeyValuePair<string, Item> pair in this.dico)
			{
				pair.Value.Dispose();
			}

			this.dico.Clear();
		}


		public void FlushUnused(List<string> filenames)
		{
			//	Supprime toutes les images inutilisées du cache des images, c'est-à-dire
			//	dont le nom de fichier n'est pas dans la liste filenames.
			List<string> keysToDelete = new List<string>();
			foreach (string key in this.dico.Keys)
			{
				if (!filenames.Contains(key))
				{
					keysToDelete.Add(key);
				}
			}

			foreach (string key in keysToDelete)
			{
				Item item = this.dico[key];
				this.dico.Remove(key);
				item.Dispose();
			}
		}

		public void ClearInsideDoc()
		{
			//	Enlève tous les modes InsideDoc.
			foreach (Item item in this.dico.Values)
			{
				item.InsideDoc = false;
			}
		}

		public void GenerateShortNames()
		{
			//	Génère les noms courts pour toutes les images du document.
			foreach (Item item in this.dico.Values)
			{
				item.ShortName = null;
			}

			foreach (Item item in this.dico.Values)
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
			foreach (Item item in this.dico.Values)
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

			foreach (Item item in this.dico.Values)
			{
				if (item.InsideDoc || imageIncludeMode == Document.ImageIncludeMode.All)
				{
					string name = string.Format("images/{0}", item.ShortName);
					zip.AddEntry(name, item.GlobalItem.Data);
				}
			}
		}

		public void ReadData(ZipFile zip, Document.ImageIncludeMode imageIncludeMode, Properties.Image propImage)
		{
			//	Lit les données d'une image.
			if (!this.Contains(propImage.Filename))  // pas encore dans le cache ?
			{
				if (propImage.InsideDoc || imageIncludeMode == Document.ImageIncludeMode.All)
				{
					string name = string.Format("images/{0}", propImage.ShortName);
					byte[] data = zip[name].Data;  // lit les données dans le fichier zip
					this.Add(propImage.Filename, data);
				}
				else
				{
					this.Add(propImage.Filename, null);  // lit le fichier image sur disque
				}
			}
		}


		#region Class Item
		public class Item : System.IDisposable
		{
			public Item(GlobalImageCache.Item gItem, bool isLowres)
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


		protected Dictionary<string, Item>		dico;
		protected bool							isLowres;
	}
}
