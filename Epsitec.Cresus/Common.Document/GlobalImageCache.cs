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
			//	Retourne les données d'une image.
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
			//	Vérifie si une image est en cache.
			return GlobalImageCache.dico.ContainsKey(filename);
		}

		public static Item Add(string filename, byte[] data)
		{
			//	Ajoute une nouvelle image dans le cache.
			//	Si les données 'data' n'existent pas, l'image est lue sur disque.
			//	Si les données existent, l'image est lue à partir des données en mémoire.
			if (GlobalImageCache.dico.ContainsKey(filename))
			{
				return GlobalImageCache.dico[filename];
			}
			else
			{
				Item item = new Item(filename, data);
				if (item.Data == null)
				{
					return null;
				}
				else
				{
					GlobalImageCache.dico.Add(filename, item);
					GlobalImageCache.UpdateBigSize();
					return item;
				}
			}
		}

		public static void Remove(string filename)
		{
			//	Supprime une image dans le cache.
			Item item = GlobalImageCache.dico[filename];
			if (item != null)
			{
				GlobalImageCache.dico.Remove(filename);
				GlobalImageCache.UpdateBigSize();
				item.Dispose();
			}
		}

		public static void Free()
		{
			//	Libère toutes les images, si nécessaire.
			if (GlobalImageCache.isBigSize)
			{
				foreach (Item item in GlobalImageCache.dico.Values)
				{
					item.FreeOriginal();
				}
			}
		}

		public static bool IsBigSize
		{
			get
			{
				return GlobalImageCache.isBigSize;
			}
		}

		protected static void UpdateBigSize()
		{
			//	Indique si le total des images originales en cache dépasse 0.5 GB.
			long total = 0;
			foreach (Item item in GlobalImageCache.dico.Values)
			{
				total += item.KBWeight;
			}

			GlobalImageCache.isBigSize = (total > 500000);  // dépasse 0.5 GB ?
		}


		#region Class Item
		public class Item : System.IDisposable
		{
			public Item(string filename, byte[] data)
			{
				//	Constructeur qui met en cache les données de l'image.
				//	Si les données 'data' n'existent pas, l'image est lue sur disque.
				//	Si les données existent, l'image est lue à partir des données en mémoire.
				this.filename = filename;

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
				}
				catch
				{
					this.data = null;
				}
			}

			public bool Reload()
			{
				//	Relit l'image sur disque. Utile lorsque le fichier a changé.
				//	Retourne false en cas d'erreur (fichier n'existe pas).
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

				this.originalImage = null;
				this.lowresImage = null;
				return true;
			}

			public void FreeOriginal()
			{
				//	Libère l'image originale, si elle est grosse.
				if (this.data == null)
				{
					return;
				}

				if (this.originalImage != null && this.IsBigOriginal)
				{
					this.originalImage.Dispose();
					this.originalImage = null;
				}
			}

			public void Write(string otherFilename)
			{
				//	Exporte l'image originale dans un fichier quelconque.
				if (this.data == null)
				{
					return;
				}

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
				//	Retourne l'échelle de l'image pour l'affichage (>= 1).
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

			public Drawing.Image Image(bool isLowres)
			{
				//	Retourne l'objet Drawing.Image.
				if (this.data == null)
				{
					return null;
				}

				if (isLowres)
				{
					this.ReadLowresImage();
					return this.lowresImage;
				}
				else
				{
					this.ReadOriginalImage();
					return this.originalImage;
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

			protected bool IsBigOriginal
			{
				//	Retourne 'true' si l'image originale dépasse 200 KB.
				get
				{
					return (this.KBWeight > 200);
				}
			}

			protected void ReadOriginalImage()
			{
				//	Lit l'image originale, si nécessaire.
				if (this.data == null || this.originalImage != null)
				{
					return;
				}

				this.originalImage = Bitmap.FromData(this.data);
				System.Diagnostics.Debug.Assert(this.originalImage != null);
				this.originalSize = this.originalImage.Size;
			}

			protected void ReadLowresImage()
			{
				//	Crée l'image pour l'affichage, si nécessaire.
				//	Libère l'image originale si elle est trop grosse.
				if (this.data == null || this.lowresImage != null)
				{
					return;
				}

				this.ReadOriginalImage();

				if (this.IsBigOriginal)  // image dépasse 200 KB ?
				{
					//	Génère une image pour l'affichage (this.lowresImage) qui pèse
					//	environ 200 KB.
					this.lowresScale = System.Math.Sqrt(this.KBWeight/200);
					int dx = (int) (this.originalSize.Width/this.lowresScale);
					int dy = (int) (this.originalSize.Height/this.lowresScale);
					this.lowresImage = GlobalImageCache.ResizeImage(this.originalImage, dx, dy);

					this.originalImage.Dispose();  // oublie tout de suite l'image originale
					this.originalImage = null;
				}
				else
				{
					this.lowresImage = this.originalImage;
					this.lowresScale = 1.0;
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

				if (this.lowresImage != null)
				{
					this.lowresImage.Dispose();
					this.lowresImage = null;
				}
			}
			#endregion
						
			protected string				filename;
			protected byte[]				data;
			protected Drawing.Image			originalImage;
			protected Drawing.Image			lowresImage;
			protected Size					originalSize;
			protected double				lowresScale;
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


		protected static Dictionary<string, Item>	dico = new Dictionary<string, Item>();
		protected static bool						isBigSize = false;
	}
}
