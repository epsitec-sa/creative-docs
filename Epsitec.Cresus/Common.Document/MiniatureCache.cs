using System.Collections.Generic;
using Epsitec.Common.IO;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe MiniatureCache g�re le cache statique des miniatures.
	/// </summary>
	public class MiniatureCache
	{
		public static void Add(string filename)
		{
			//	Ajoute une miniature dans le cache.
			if (!MiniatureCache.cache.ContainsKey(filename))
			{
				byte[] data = MiniatureCache.ReadMiniature(filename);
				if (data != null)
				{
					Image image = Bitmap.FromData(data);
					MiniatureCache.cache.Add(filename, image);
				}
			}
		}

		public static void Remove(string filename)
		{
			//	Supprime une miniature du cache.
			MiniatureCache.cache.Remove(filename);
		}
		
		public static Image Image(string filename)
		{
			//	Retourne une miniature contenue dans le cache.
			return MiniatureCache.cache[filename];
		}


		protected static byte[] ReadMiniature(string filename)
		{
			//	Lit les donn�es de l'image miniature associ�e au fichier.
			ZipFile zip = new ZipFile();

			if (zip.TryLoadFile(filename))
			{
				try
				{
					return zip["preview.png"].Data;  // lit les donn�es dans le fichier zip
				}
				catch
				{
					return null;
				}
			}

			return null;
		}


		protected static Dictionary<string, Image> cache = new Dictionary<string,Image>();
	}
}
