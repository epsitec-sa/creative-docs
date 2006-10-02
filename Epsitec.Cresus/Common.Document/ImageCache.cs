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
			this.isDisplay = true;
		}


		public bool IsDisplay
		{
			//	Indique le type des images auxquelles on s'intéresse.
			get
			{
				return this.isDisplay;
			}

			set
			{
				if (this.isDisplay != value)
				{
					this.isDisplay = value;

					//	Informe tout le cache.
					foreach (Item item in this.dico.Values)
					{
						item.IsDisplay = this.isDisplay;
					}
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

			if (this.dico.ContainsKey(filename))
			{
				return this.dico[filename];
			}
			else
			{
				return null;
			}
		}

		public bool Contains(string filename)
		{
			//	Vérifie si une image est en cache.
			return this.dico.ContainsKey(filename);
		}

		public Item Add(string filename, byte[] data)
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
				Item item = new Item(filename, data, this.isDisplay);
				this.dico.Add(filename, item);
				return item;
			}
		}

		public void Remove(string filename)
		{
			//	Supprime une image dans le cache.
			Item item = this.dico[filename];
			if (item != null)
			{
				this.dico.Remove(filename);
				item.Dispose();
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


		public void Free()
		{
			//	Libère toutes les images.
			long total = 0;
			foreach (Item item in this.dico.Values)
			{
				total += item.KBWeight;
			}

			if (total > 100000)  // occupe plus de 100 MB ?
			{
				foreach (KeyValuePair<string, Item> pair in this.dico)
				{
					pair.Value.FreeOriginal();
				}
			}
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
					zip.AddEntry(name, item.Data);
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
			public Item(string filename, byte[] data, bool isDisplay)
			{
				//	Constructeur qui met en cache les données de l'image.
				//	Si les données 'data' n'existent pas, l'image est lue sur disque.
				//	Si les données existent, l'image est lue à partir des données en mémoire.
				this.filename = filename;
				this.shortName = null;
				this.isDisplay = isDisplay;

				try
				{
					if (data == null)  // lecture sur disque ?
					{
						this.data = System.IO.File.ReadAllBytes(this.filename);
					}
					else  // image en mémoire ?
					{
						this.data = data;
					}

					this.originalImage = Bitmap.FromData(this.data);
					this.originalSize = this.originalImage.Size;
					this.CreateDisplayImage();
				}
				catch
				{
					this.data = null;
					this.originalImage = null;
					this.originalSize = Size.Empty;
				}

				this.dimmedImage = null;
			}

			public bool Reload()
			{
				//	Relit l'image sur disque.
				//	Retourne false en cas d'erreur.
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

				Drawing.Image initialImage = this.originalImage;

				try
				{
					this.originalImage = Bitmap.FromData(this.data);
					this.CreateDisplayImage();
				}
				catch
				{
					this.data = initialData;
					this.originalImage = initialImage;
					return false;
				}

				this.dimmedImage = null;
				return true;
			}

			protected void CreateDisplayImage()
			{
				//	Crée l'image pour l'affichage.
				//	Libère l'image originale si elle est trop grosse.
				System.Diagnostics.Debug.Assert(this.originalImage != null);

				if (this.IsBigOriginal)  // image dépasse 200 KB ?
				{
					//	Génère une image pour l'affichage (this.displayImage) qui pèse
					//	environ 200 KB.
					this.displayScale = System.Math.Sqrt(this.KBWeight/200);
					int dx = (int) (this.originalSize.Width/this.displayScale);
					int dy = (int) (this.originalSize.Height/this.displayScale);
					this.displayImage = ImageCache.ResizeImage(this.originalImage, dx, dy);

					this.originalImage.Dispose();  // oublie tout de suite l'image originale
					this.originalImage = null;
				}
				else
				{
					this.displayImage = this.originalImage;
					this.displayScale = 1.0;
				}
			}

			public void FreeOriginal()
			{
				//	Libère l'image originale, si elle est grosse.
				if (this.originalImage != null && this.IsBigOriginal)
				{
					this.originalImage.Dispose();
					this.originalImage = null;
				}
			}

			public void Write(string otherFilename)
			{
				//	Exporte l'image originale dans un fichier quelconque.
				System.IO.File.WriteAllBytes(otherFilename, this.data);
			}

			public bool IsDisplay
			{
				//	Indique le type de l'image a laquelle on s'intéresse.
				get
				{
					return this.isDisplay;
				}
				set
				{
					this.isDisplay = value;
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

			public double Scale
			{
				//	Retourne l'échelle de l'image pour l'affichage (>= 1).
				get
				{
					return this.isDisplay ? this.displayScale : 1.0;
				}
			}

			public bool IsBigOriginal
			{
				//	Retourne 'true' si l'image originale dépasse 200 KB.
				get
				{
					return (this.KBWeight > 200);
				}
			}

			public long KBWeight
			{
				//	Retourne la taille de l'image en KB.
				get
				{
					return ((long) this.originalSize.Width * (long) this.originalSize.Height) / (1024/4);
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

			public byte[] Data
			{
				//	Données brutes de l'image.
				get
				{
					return this.data;
				}
				set
				{
					this.data = value;
				}
			}

			public Drawing.Image Image
			{
				//	Retourne l'objet Drawing.Image.
				get
				{
					if (this.isDisplay)
					{
						return this.displayImage;
					}
					else
					{
						if (this.originalImage == null)
						{
							this.originalImage = Bitmap.FromData(this.data);
						}

						return this.originalImage;
					}
				}
			}

			public Drawing.Image DimmedImage
			{
				//	Retourne l'objet Drawing.Image estompé.
				get
				{
					//?this.OpenBitmapDimmed();  // trop couteux en temps calcul et en mémoire !
					return this.dimmedImage;
				}
			}

			protected void OpenBitmapDimmed()
			{
				//	Ouvre le bitmap de la variante estompée de l'image si nécessaire.
				if ( this.originalImage == null )  return;
				if ( this.dimmedImage != null )  return;

				this.dimmedImage = Bitmap.CopyImage(this.originalImage);
				Pixmap.RawData data = new Pixmap.RawData(this.dimmedImage);
				using ( data )
				{
					Color pixel;
					double intensity;

					for ( int y=0 ; y<data.Height ; y++ )
					{
						for ( int x=0 ; x<data.Width ; x++ )
						{
							pixel = data[x,y];

							intensity = pixel.GetBrightness();
							intensity = System.Math.Max(intensity*2.0-1.0, 0.0);
							pixel.R = intensity;
							pixel.G = intensity;
							pixel.B = intensity;
							pixel.A *= 0.2;  // très transparent

							data[x,y] = pixel;
						}
					}
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

				if (this.displayImage != null)
				{
					this.displayImage.Dispose();
					this.displayImage = null;
				}

				if (this.dimmedImage != null)
				{
					this.dimmedImage.Dispose();
					this.dimmedImage = null;
				}
			}
			#endregion
						
			protected string				filename;
			protected string				shortName;
			protected bool					insideDoc;
			protected byte[]				data;
			protected Drawing.Image			originalImage;
			protected Drawing.Image			displayImage;
			protected Drawing.Image			dimmedImage;
			protected Size					originalSize;
			protected double				displayScale;
			protected bool					isDisplay;
		}
		#endregion


		protected static Drawing.Image ResizeImage(Drawing.Image image, int dx, int dy)
		{
			//	Retourne une image redimensionnée.
			Graphics gfx = new Graphics();
			gfx.SetPixmapSize(dx, dy);
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(1, -1, 0, 0);

			gfx.ImageFilter = new ImageFilter(ImageFilteringMode.Bilinear);
			gfx.PaintImage(image, new Rectangle(0, 0, dx, dy));

			return Bitmap.FromPixmap(gfx.Pixmap) as Bitmap;
		}


		protected Dictionary<string, Item>	dico;
		protected bool						isDisplay;
	}
}
