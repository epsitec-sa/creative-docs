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
		}


		public Item Get(string filename)
		{
			//	Retourne les données d'une image.
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
				Item item = new Item(filename, data);
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
			}
		}

		public void Clear()
		{
			//	Supprime toutes les images du cache.
			this.dico.Clear();
		}


		public void FlushUnused(List<string> filenames)
		{
			//	Supprime toutes les images inutilisées du cache des images, c'est-à-dire
			//	dont le nom de fichier n'est pas dans la liste filenames.
			for (int i=0; i<this.dico.Count; i++)
			{
				//	TODO:
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


		public void WriteData(ZipFile zip)
		{
			//	Ecrit toutes les données des images.
			foreach (Item item in this.dico.Values)
			{
				if (item.InsideDoc)
				{
					string name = string.Format("images/{0}", item.ShortName);
					zip.AddEntry(name, item.Data);
				}
			}
		}

		public void ReadData(ZipFile zip, Properties.Image propImage)
		{
			//	Lit les données d'une image.
			if (!this.Contains(propImage.Filename))  // pas encore dans le cache ?
			{
				if (propImage.InsideDoc)
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
		public class Item
		{
			public Item(string filename, byte[] data)
			{
				//	Constructeur qui met en cache les données de l'image.
				//	Si les données 'data' n'existent pas, l'image est lue sur disque.
				//	Si les données existent, l'image est lue à partir des données en mémoire.
				this.filename = filename;
				this.shortName = null;

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

					this.image = Drawing.Bitmap.FromData(this.data);
				}
				catch
				{
					this.data = null;
					this.image = null;
				}

				this.imageDimmed = null;
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
				//	Retourne l'objet Drawing.Image normal.
				get
				{
					return this.image;
				}
			}

			public Drawing.Image ImageDimmed
			{
				//	Retourne l'objet Drawing.Image estompé.
				get
				{
					//?this.OpenBitmapDimmed();  // trop couteux en temps calcul et en mémoire !
					return this.imageDimmed;
				}
			}

			protected void OpenBitmapDimmed()
			{
				//	Ouvre le bitmap de la variante estompée de l'image si nécessaire.
				if ( this.image == null )  return;
				if ( this.imageDimmed != null )  return;

				this.imageDimmed = Bitmap.CopyImage(this.image);
				Pixmap.RawData data = new Pixmap.RawData(this.imageDimmed);
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

			protected string				filename;
			protected string				shortName;
			protected bool					insideDoc;
			protected byte[]				data;
			protected Drawing.Image			image;
			protected Drawing.Image			imageDimmed;
		}
		#endregion


		protected Dictionary<string, Item>	dico;
	}
}
