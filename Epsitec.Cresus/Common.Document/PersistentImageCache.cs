using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.IO;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Cache statique persistant (sur disque) des images de l'application.
	/// </summary>
	public class PersistentImageCache
	{
		static PersistentImageCache()
		{
			//	Constructeur statique.
			System.IO.Directory.CreateDirectory(PersistentImageCache.Directory);
			PersistentImageCache.Read();
		}

		public static string Key(string filename, string zipFilename, string zipShortName)
		{
			//	Fabrique une clé d'accés unique pour une image.
			if (zipFilename == null)
			{
				return filename;
			}
			else
			{
				return string.Concat(filename, "\n", zipFilename, "\n", zipShortName);
			}
		}

		public static void Add(string key, System.DateTime date, Drawing.Image image, Size originalSize)
		{
			//	Ajoute une image dans le cache.
			if (!PersistentImageCache.dico.ContainsKey(key))
			{
				byte[] data = image.BitmapImage.Save(ImageFormat.Jpeg);

				long fileStamp = PersistentImageCache.fileStamp++;
				string cacheFilename = PersistentImageCache.ImageCacheFilename(fileStamp);
				PersistentImageCache.WriteCacheImage(cacheFilename, data);

				PersistentImage pi = new PersistentImage(originalSize, fileStamp, date);

				PersistentImageCache.dico.Add(key, pi);
				PersistentImageCache.Write();
			}
		}

		public static bool Get(string key, System.DateTime date, out Drawing.Image image, out Size originalSize)
		{
			//	Retourne une image contenue dans le cache.
			if (PersistentImageCache.dico.ContainsKey(key))
			{
				PersistentImage pi = PersistentImageCache.dico[key];
				string cacheFilename = PersistentImageCache.ImageCacheFilename(pi.FileStamp);

				if (date == pi.Date)
				{
					pi.SetRecentTimeStamp();

					byte[] data = PersistentImageCache.ReadCacheImage(cacheFilename);
					if (data != null)
					{
						image = Bitmap.FromData(data);
						originalSize = pi.OriginalSize;
						return true;
					}
				}

				PersistentImageCache.DeleteCacheImage(cacheFilename);
				PersistentImageCache.dico.Remove(key);
				PersistentImageCache.Write();
			}

			image = null;
			originalSize = Size.Empty;
			return false;
		}


		public static bool Read()
		{
			//	Lit le fichier "cache.data".
			try
			{
				using (Stream stream = File.OpenRead(PersistentImageCache.CacheFilename))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Binder = new VersionDeserializationBinder();

					try
					{
						PersistentImageCache.dico = (Dictionary<string, PersistentImage>) formatter.Deserialize(stream);

						long timeStamp = 0;
						long fileStamp = 0;
						foreach (PersistentImage pi in PersistentImageCache.dico.Values)
						{
							timeStamp = System.Math.Max(timeStamp, pi.TimeStamp);
							fileStamp = System.Math.Max(fileStamp, pi.FileStamp);
						}
						PersistentImageCache.timeStamp = timeStamp+1;
						PersistentImageCache.fileStamp = fileStamp+1;
					}
					catch
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool Write()
		{
			//	Ecrit le fichier "cache.data".
			try
			{
				using (Stream stream = File.OpenWrite(PersistentImageCache.CacheFilename))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, PersistentImageCache.dico);
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		sealed class VersionDeserializationBinder : IO.GenericDeserializationBinder
		{
			public VersionDeserializationBinder()
			{
			}
		}


		protected static void DeleteCacheImage(string cacheFilename)
		{
			//	Supprime une image dans le cache.
			string filename = string.Concat(PersistentImageCache.Directory, cacheFilename);
			System.IO.File.Delete(filename);
		}

		protected static void WriteCacheImage(string cacheFilename, byte[] data)
		{
			//	Ecrit une image dans le cache.
			string filename = string.Concat(PersistentImageCache.Directory, cacheFilename);
			System.IO.File.WriteAllBytes(filename, data);
		}

		protected static byte[] ReadCacheImage(string cacheFilename)
		{
			//	Lit une image dans le cache.
			string filename = string.Concat(PersistentImageCache.Directory, cacheFilename);
			return System.IO.File.ReadAllBytes(filename);
		}

		protected static string ImageCacheFilename(long fileStamp)
		{
			//	Donne le nom d'une image dans le dossier du cache.
			return string.Concat(fileStamp.ToString(System.Globalization.CultureInfo.InvariantCulture), ".jpg");
		}

		protected static string CacheFilename
		{
			//	Donne le nom du fichier "cache.data".
			get
			{
				return string.Concat(PersistentImageCache.Directory, "cache.data");
			}
		}

		protected static string Directory
		{
			//	Retourne le nom du dossier du cache de l'application.
			//	Le dossier est qq chose du genre:
			//	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus documents\ImageCache\
			get
			{
				string dir = Globals.Directories.UserAppData;
				int i = dir.LastIndexOf("\\");
				if (i > 0)
				{
					dir = dir.Substring(0, i);  // supprime le dossier "1.0.0.0" à la fin
				}

				return string.Concat(dir, "\\ImageCache\\");
			}
		}


		[System.Serializable()]
		public class PersistentImage : ISerializable
		{
			protected PersistentImage()
			{
				this.originalSize = Size.Empty;
				this.date = System.DateTime.MinValue;
				this.timeStamp = 0;
				this.fileStamp = 0;
			}

			public PersistentImage(Size originalSize, long fileStamp, System.DateTime date)
			{
				this.originalSize = originalSize;
				this.fileStamp = fileStamp;
				this.date = date;
				this.SetRecentTimeStamp();
			}

			public Size OriginalSize
			{
				get
				{
					return this.originalSize;
				}
			}

			public System.DateTime Date
			{
				get
				{
					return this.date;
				}
			}

			public long FileStamp
			{
				//	Retourne la marque du fichier.
				get
				{
					return this.fileStamp;
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
				this.timeStamp = PersistentImageCache.timeStamp++;
			}

			#region Serialization
			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				//	Sérialise les données.
				info.AddValue("Version", 1);

				info.AddValue("OriginalSize", this.originalSize);
				info.AddValue("Date", this.date);
				info.AddValue("TimeStamp", this.timeStamp);
				info.AddValue("FileStamp", this.fileStamp);
			}

			protected PersistentImage(SerializationInfo info, StreamingContext context) : this()
			{
				//	Constructeur qui désérialise les données.
				int version = info.GetInt32("Version");

				this.originalSize = (Size) info.GetValue("OriginalSize", typeof(Size));
				this.date = (System.DateTime) info.GetValue("Date", typeof(System.DateTime));
				this.timeStamp = info.GetInt64("TimeStamp");
				this.fileStamp = info.GetInt64("FileStamp");
			}
			#endregion

			protected Size							originalSize;
			protected System.DateTime				date;
			protected long							timeStamp;
			protected long							fileStamp;
		}


		protected static Dictionary<string, PersistentImage>	dico = new Dictionary<string, PersistentImage>();
		protected static long									timeStamp = 0;
		protected static long									fileStamp = 0;
	}
}
