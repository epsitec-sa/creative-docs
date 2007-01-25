using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe DocumentCache g�re le cache statique des miniatures et des statistiques.
	/// </summary>
	public static class DocumentCache
	{
		public static void Add(string filename)
		{
			//	Ajoute une miniature et une statistique dans le cache, si elles n'y sont pas d�j�.
			//	Si la miniature ou la statistique ont chang�, il faut au pr�alable ex�cuter Remove.
			filename = filename.ToLower();  // voir (*)

			lock (DocumentCache.cache)
			{
				if (DocumentCache.cache.ContainsKey (filename))
				{
					return;
				}
			}

			//	Pour l'instant, le cache ne contient aucune information pour ce document; on va
			//	le charger, extraire l'image et les statistiques, puis mettre � jour le cache.
			
			byte[] dataImage;
			byte[] dataDocInfo;
			DocumentCache.ReadData (filename, out dataImage, out dataDocInfo);

			if (dataImage != null || dataDocInfo != null)
			{
				Item item = new Item ();

				if (dataImage != null)
				{
					item.Image = Bitmap.FromData (dataImage);
				}

				if (dataDocInfo != null)
				{
					item.DocumentInfo = Serialization.DeserializeFromMemory (dataDocInfo) as DocumentInfo;
				}

				lock (DocumentCache.cache)
				{
					//	Il faut encore s'assurer que le cache n'a pas �t� mis � jour entre temps
					//	par quelqu'un d'autre, auquel cas on peut simplement mettre � la poubelle
					//	les informations (item) :

					if (!DocumentCache.cache.ContainsKey (filename))
					{
						DocumentCache.cache.Add (filename, item);
					}
				}
			}
		}

		public static void Remove(string filename)
		{
			//	Supprime une miniature et une statistique du cache.
			filename = filename.ToLower();  // voir (*)

			lock (DocumentCache.cache)
			{
				DocumentCache.cache.Remove (filename);
			}
		}

		public static Image FindImage(string filename)
		{
			//	Retourne une miniature contenue dans le cache.
			//	Retourne null si la miniature n'est pas dans le cache.

			return DocumentCache.FindItem (filename).Image;
		}

		public static DocumentInfo FindDocumentInfo(string filename)
		{
			//	Retourne une statistique contenue dans le cache.
			//	Retourne null si la statistique n'est pas dans le cache.
			
			return DocumentCache.FindItem (filename).DocumentInfo;
		}

		// (*)	Il faut consid�rer 'Abc' = 'abc' � cause de certaines anomalies.
		//		Par exemple, Document.DirectoryMySamples retourne :
		//		C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Cr�sus Documents\Mes exemples
		//		'Cr�sus Documents' au lieu de 'Cr�sus documents' qui est le vrai nom !

		private static Item FindItem(string filename)
		{
			filename = filename.ToLower ();  // voir (*)

			lock (DocumentCache.cache)
			{
				Item item;
				DocumentCache.cache.TryGetValue (filename, out item);
				return item;
			}
		}

		private static void ReadData(string filename, out byte[] dataImage, out byte[] dataDocInfo)
		{
			string extension = System.IO.Path.GetExtension (filename).ToLower ();

			switch (extension)
			{
				case ".crdoc":
				case ".crmod":
					DocumentCache.ReadDocData (filename, out dataImage, out dataDocInfo);
					break;

				default:
					dataImage = null;
					dataDocInfo = null;
					break;
			}
		}

		private static void ReadDocData(string filename, out byte[] dataImage, out byte[] dataDocInfo)
		{
			//	Lit les donn�es (miniature et statistique) associ�es au fichier.
			ZipFile zip = new ZipFile();

			dataImage = null;
			dataDocInfo = null;

			bool ok = zip.TryLoadFile (filename,
				delegate (string entryName)
				{
					return (entryName == "preview.png")
						|| (entryName == "statistics.data");
				});

			if (ok)
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
					dataDocInfo = zip["statistics.data"].Data;  // lit les donn�es dans le fichier zip
				}
				catch
				{
					dataDocInfo = null;
				}
			}
		}

		private struct Item
		{
			public Image Image;
			public DocumentInfo DocumentInfo;
		}


		private static Dictionary<string, Item> cache = new Dictionary<string, Item>();
	}
}
