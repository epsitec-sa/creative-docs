using System.Collections.Generic;
using Epsitec.Common.IO;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe DocumentCache g�re le cache statique des miniatures et des statistiques.
	/// </summary>
	public class DocumentCache
	{
		public static void Add(string filename)
		{
			//	Ajoute une miniature et une statistique dans le cache, si elles n'y sont pas d�j�.
			//	Si la miniature ou la statistique ont chang�, il faut au pr�alable ex�cuter Remove.
			filename = filename.ToLower();  // voir (*)

			if (!DocumentCache.cache.ContainsKey(filename))
			{
				byte[] dataImage;
				byte[] dataStatistics;
				DocumentCache.ReadData(filename, out dataImage, out dataStatistics);

				if (dataImage != null || dataStatistics != null)
				{
					Item item = new Item();

					if (dataImage != null)
					{
						item.Image = Bitmap.FromData(dataImage);
					}

					if (dataStatistics != null)
					{
						Document.Statistics stat = new Document.Statistics();
						item.Statistics = Serialization.DeserializeFromMemory(dataStatistics) as Document.Statistics;
					}

					DocumentCache.cache.Add(filename, item);
				}
			}
		}

		public static void Remove(string filename)
		{
			//	Supprime une miniature et une statistique du cache.
			filename = filename.ToLower();  // voir (*)

			DocumentCache.cache.Remove(filename);
		}

		public static Image Image(string filename)
		{
			//	Retourne une miniature contenue dans le cache.
			//	Retourne null si la miniature n'est pas dans le cache.
			filename = filename.ToLower();  // voir (*)

			if (DocumentCache.cache.ContainsKey(filename))
			{
				return DocumentCache.cache[filename].Image;
			}
			else
			{
				return null;
			}
		}

		public static Document.Statistics Statistics(string filename)
		{
			//	Retourne une statistique contenue dans le cache.
			//	Retourne null si la statistique n'est pas dans le cache.
			filename = filename.ToLower();  // voir (*)

			if (DocumentCache.cache.ContainsKey(filename))
			{
				return DocumentCache.cache[filename].Statistics;
			}
			else
			{
				return null;
			}
		}

		// (*)	Il faut consid�rer 'Abc' = 'abc' � cause de certaines anomalies.
		//		Par exemple, Document.DirectoryMySamples retourne :
		//		C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Cr�sus Documents\Mes exemples
		//		'Cr�sus Documents' au lieu de 'Cr�sus documents' qui est le vrai nom !


		protected static void ReadData(string filename, out byte[] dataImage, out byte[] dataStatistics)
		{
			//	Lit les donn�es (miniature et statistique) associ�es au fichier.
			ZipFile zip = new ZipFile();

			dataImage = null;
			dataStatistics = null;

			if (zip.TryLoadFile(filename, DocumentCache.IsLoading))
			{
				try
				{
					dataImage = zip["preview.png"].Data;  // lit les donn�es dans le fichier zip
				}
				catch
				{
					dataImage = null;
				}

				try
				{
					dataStatistics = zip["statistics.data"].Data;  // lit les donn�es dans le fichier zip
				}
				catch
				{
					dataStatistics = null;
				}
			}
		}

		protected static bool IsLoading(string entryName)
		{
			return (entryName == "preview.png" || entryName == "statistics.data");
		}


		protected struct Item
		{
			public Image Image;
			public Document.Statistics Statistics;
		}


		protected static Dictionary<string, Item> cache = new Dictionary<string, Item>();
	}
}
