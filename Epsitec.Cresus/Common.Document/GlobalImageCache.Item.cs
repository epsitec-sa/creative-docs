//	Copyright © 2006-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Cache statique global des images de l'application.
	/// </summary>
	public static partial class GlobalImageCache
	{
		public class Item : System.IDisposable
		{
			public Item(string filename, string zipPath, System.DateTime date)
			{
				//	Constructeur qui met en cache les données de l'image.
				System.Diagnostics.Debug.Assert (date != System.DateTime.MinValue);

				this.filename = filename;

				if (string.IsNullOrEmpty (zipPath))
				{
				}
				else
				{
					GlobalImageCache.ExtractZipPathNames (zipPath, out this.zipFilename, out this.zipEntryName);
				}

				this.date = date;
				this.SetRecentTimeStamp ();
			}

			public bool Reload()
			{
				//	Relit l'image sur disque. Utile lorsque le fichier a changé.
				//	Retourne false en cas d'erreur (fichier n'existe pas).

				if (this.zipFilename == null)
				{
					this.date = GlobalImageCache.GetFileDate (this.filename);
				}

				this.data          = null;
				this.originalImage = null;
				this.lowResImage   = null;

				return this.date != System.DateTime.MinValue;
			}

			public long Free(ImagePart part)
			{
				//	Libère si possible une partie de l'image.
				//	Retourne la taille libérée en KB.
				long total = 0;

				if (this.IsFreeable (part))
				{
					//?System.Diagnostics.Debug.WriteLine(string.Format("GlobalImageCache: Free {0} {1}", this.filename, part.ToString()));
					total = this.KBUsed (part);

					if (part == ImagePart.LargeOriginal || part == ImagePart.SmallOriginal)
					{
//!						this.originalImage.Dispose();
						this.originalImage = null;
					}

					if (part == ImagePart.Lowres)
					{
//!						this.lowresImage.Dispose();
						this.lowResImage = null;
					}

					if (part == ImagePart.Data)
					{
						this.data = null;
					}
				}

				return total;
			}

			public bool IsFreeable(ImagePart part)
			{
				//	Indique s'il est possible de libérer une partie de cette image.
				if (part == ImagePart.LargeOriginal)
				{
					return (this.originalImage != null && this.originalImage.IsAlive && this.IsLargeOriginal);
				}

				if (part == ImagePart.SmallOriginal)
				{
					return (this.originalImage != null && this.originalImage.IsAlive);
				}

				if (part == ImagePart.Lowres)
				{
					return (this.lowResImage != null);
				}

				if (part == ImagePart.Data)
				{
					return (this.data != null);
				}

				return false;
			}

			public long KBUsed()
			{
				//	Retourne la taille totale utilisée par l'image (toutes les parties) en KB.
				//	Prend en compte l'image originale, l'image basse résolution et les données.
				long total = 0;

				total += this.KBUsed (ImagePart.LargeOriginal);  // ne pas compter LargeOriginal + SmallOriginal !
				total += this.KBUsed (ImagePart.Lowres);
				total += this.KBUsed (ImagePart.Data);

				return total;
			}

			public long KBUsed(ImagePart part)
			{
				//	Retourne la taille utilisée par une partie de l'image.
				//	Si la partie n'est pas utilisée, retourne zéro.
				long total = 0;

				if (part == ImagePart.LargeOriginal || part == ImagePart.SmallOriginal)
				{
					if (this.originalImage != null && this.originalImage.IsAlive)
					{
						total += this.KBOriginalWeight;
					}
				}

				if (part == ImagePart.Lowres)
				{
					if (this.lowResImage != null)
					{
						double w = this.originalSize.Width  / this.lowResScale;
						double h = this.originalSize.Height / this.lowResScale;
						total += ((long) w * (long) h) / (1024/4);
					}
				}

				if (part == ImagePart.Data)
				{
					if (this.data != null)
					{
						total += this.data.Length/1024;
					}
				}

				return total;
			}

			public long KBOriginalWeight
			{
				//	Retourne la taille de l'image originale en KB, qu'elle existe ou pas.
				get
				{
					return ((long) this.originalSize.Width * (long) this.originalSize.Height) / (1024/4);
				}
			}

			protected bool IsLargeOriginal
			{
				//	Retourne 'true' si l'image originale dépasse la limite.
				get
				{
					return (this.KBOriginalWeight > GlobalImageCache.ImageLimit);
				}
			}

			public long TimeStamp
			{
				//	Retourne la marque de vieillesse.
				get
				{
					return this.timeStamp;
				}
			}

			public void SetRecentTimeStamp()
			{
				//	Met la marque de vieillesse la plus récente.
				this.timeStamp = GlobalImageCache.timeStamp++;
			}

			public bool Locked
			{
				get
				{
					return this.locked;
				}
				set
				{
					this.locked = value;
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

			public double LowResScale
			{
				//	Retourne l'échelle de l'image pour l'affichage (>= 1).
				get
				{
					return this.lowResScale;
				}
			}

			public string LocalKeyName
			{
				get
				{
					return ImageManager.GetKey (this.filename, this.date);
				}
			}

			public string GlobalKeyName
			{
				get
				{
					return GlobalImageCache.GetGlobalKeyName (this.LocalKeyName, this.zipFilename);
				}
			}

			public string FileName
			{
				//	Retourne le nom de fichier avec le chemin complet.
				get
				{
					return this.filename;
				}
			}

			public string ZipFileName
			{
				//	Nom du fichier zip contenant l'image.
				get
				{
					return this.zipFilename;
				}
			}

			public string ZipEntryName
			{
				//	Nom de l'image dans le fichier zip.
				get
				{
					return this.zipEntryName;
				}
			}

			public System.DateTime FileDate
			{
				//	Retourne la date de dernière modification.
				get
				{
					return this.date;
				}
			}

			public bool HasData
			{
				//	Indique si les données brutes de l'image existent.
				get
				{
					return this.data != null;
				}
			}

			public byte[] GetImageData()
			{
				//	Données brutes de l'image.
				this.TryReadImageData (true);
				return this.data;
			}

			public Drawing.Image GetImage(ImageCacheResolution resolution, bool read)
			{
				//	Retourne l'objet Drawing.Image.
				if (resolution == ImageCacheResolution.Low)
				{
					this.ReadLowresImage (read);

					if (this.lowResImage != null)
					{
						return this.lowResImage;
					}
				}

				return this.ReadOriginalImage (read);
			}

			protected Drawing.Image ReadLowresImage(bool read)
			{
				//	Si l'image originale est trop grosse, crée l'image basse résolution
				//	pour l'affichage et libère l'image originale.
				if (this.lowResImage != null || read == false)
				{
					return this.lowResImage;
				}

				//	Cherche d'abord l'image dans le cache persistant.
				string path = this.zipFilename == null ? string.Concat ("file:", this.filename) : GlobalImageCache.CreateZipPath (this.zipFilename, this.zipEntryName);
				ImageData imageData = GlobalImageCache.ImageManager.GetImage (path, this.filename, this.date);
				var sampleImage = imageData.GetSampleImage ();

				if (sampleImage == null)
				{
					this.originalSize = new Size (imageData.SourceWidth, imageData.SourceHeight);
					this.lowResScale = 1.0;
				}
				else
				{
					this.originalSize = new Size (imageData.SourceWidth, imageData.SourceHeight);
					this.lowResImage = Bitmap.FromImage (sampleImage);
					
					if (this.lowResImage == null)
					{
						this.lowResScale = 1.0;
					}
					else
					{
						this.lowResImage.AssigneUniqueId (this.lowResUniqueId);
						this.lowResScale = (double) this.originalSize.Width / this.lowResImage.Width;
						this.lowResUniqueId = this.lowResImage.UniqueId;
					}
				}

				return this.lowResImage;
			}

			protected Drawing.Image ReadOriginalImage(bool read)
			{
				//	Lit l'image originale, si nécessaire.
				Drawing.Image image = this.originalImage == null ? null : this.originalImage.Target;

				if (image != null)
				{
					return image;
				}

				this.TryReadImageData (read);

				if (this.data == null || read == false)
				{
					return null;
				}
				if (this.originalImage != null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("GC collected image {0}", this.filename));
				}

				image = this.TryLoadImageFromData ();

				if (image == null)
                {
					//	We really need to make more memory available; try to free everything we can here

					GlobalImageCache.FreeEverything ();
					image = this.TryLoadImageFromData ();
				}
				
				if (image != null)
				{
					this.originalImage = new Weak<Image> (image);
					this.originalSize = image.Size;
					image.AssigneUniqueId (this.originalUniqueId);
					this.originalUniqueId = image.UniqueId;
					this.SetRecentTimeStamp ();
				}
				else
				{
					this.originalImage = null;
				}

				return image;
			}

			private Drawing.Image TryLoadImageFromData()
			{
				Drawing.Image image;
				
				try
				{
					image = Bitmap.FromData (this.data);
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine ("ReadOriginalImage failed: " + ex.Message);
					image = null;
				}
				
				return image;
			}

			protected void TryReadImageData(bool read)
			{
				int attemptCount = 0;

				while (attemptCount < 5)
				{
					try
					{
						this.ReadImageData (read);
						return;
					}
					catch (System.OutOfMemoryException ex)
					{
						System.Diagnostics.Debug.WriteLine ("TryReadOriginalImage failed: " + ex.Message);

						this.data = null;
						this.date = System.DateTime.MinValue;

						GlobalImageCache.FreeEverything ();
						System.GC.Collect ();
						System.Threading.Thread.Sleep (10);

						attemptCount++;
					}
				}
			}

			protected void ReadImageData(bool read)
			{
				//	Relit les données de l'image, si nécessaire.
				if (this.data != null || read == false)
				{
					return;
				}

				this.data = this.LowLeveReadImageData ();

				this.SetRecentTimeStamp ();
			}

			public byte[] LowLeveReadImageData()
			{
				byte[] data = null;

				if (this.zipFilename == null)
				{
					data      = System.IO.File.ReadAllBytes (this.filename);
					this.date = GlobalImageCache.GetFileDate (this.filename);
				}
				else
				{
					IO.ZipFile zip = new IO.ZipFile ();

					bool ok = zip.TryLoadFile (this.zipFilename,
						delegate (string entryName)
						{
							return (entryName == this.zipEntryName);
						});

					if (ok)
					{
						data = zip[this.zipEntryName].Data;  // lit les données dans le fichier zip
					}
				}

				return data;
			}

			#region IDisposable Members
			public void Dispose()
			{
				this.data = null;

				if (this.originalImage != null)
				{
					//!					this.originalImage.Dispose();
					this.originalImage = null;
				}

				if (this.lowResImage != null)
				{
					//!					this.lowresImage.Dispose();
					this.lowResImage = null;
				}
			}
			#endregion

			protected string				filename;
			protected string				zipFilename;
			protected string				zipEntryName;
			protected System.DateTime		date;
			protected byte[]				data;
			
			private Weak<Drawing.Image>		originalImage;
			private Drawing.Image			lowResImage;
			
			protected Size					originalSize;
			protected double				lowResScale;
			protected long					timeStamp;
			protected bool					locked;

			private long					originalUniqueId;
			private long					lowResUniqueId;
		}
	}
}
