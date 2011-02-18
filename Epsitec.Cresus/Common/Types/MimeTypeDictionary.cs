//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	public static class MimeTypeDictionary
	{
		public static MimeType GetMimeTypeFromExtension(string fileExtension)
		{
			if ((string.IsNullOrEmpty (fileExtension)) ||
				(fileExtension.Length < 1))
			{
				return MimeType.Unknown;
			}

			string ext = fileExtension.ToLowerInvariant ();

			if (ext[0] == '.')
            {
				ext = ext.Substring (1);
            }

			switch (ext)
			{
				case "gif":
					return MimeType.ImageGif;

				case "jpg":
				case "jpe":
				case "jpeg":
					return MimeType.ImageJpeg;

				case "png":
					return MimeType.ImagePng;

				case "tif":
				case "tiff":
					return MimeType.ImageTiff;

				default:
					return MimeType.Unknown;
			}
		}

		public static MimeType ParseMimeType(string mime)
		{
			if (string.IsNullOrEmpty (mime))
			{
				return MimeType.Unknown;
			}

			MimeType value;
			MimeTypeDictionary.nameToType.TryGetValue (mime, out value);
			
			return value;
		}

		public static string MimeTypeToString(MimeType mime)
		{
			string value;
			MimeTypeDictionary.typeToName.TryGetValue (mime, out value);

			return value;
		}

		static MimeTypeDictionary()
		{
			var nameToType = new Dictionary<string, MimeType> ();
			var typeToName = new Dictionary<MimeType, string> ();

			foreach (MimeType value in System.Enum.GetValues (typeof (MimeType)))
			{
				if (value != MimeType.Unknown)
				{
					string name = MimeTypeDictionary.ConvertToName (value);

					nameToType[name] = value;
					typeToName[value] = name;
				}
			}

			MimeTypeDictionary.nameToType = nameToType;
			MimeTypeDictionary.typeToName = typeToName;
		}

		private static string ConvertToName(MimeType value)
		{
			var buffer = new System.Text.StringBuilder ();
			
			string mixed = value.ToString ();
			string lower = mixed.ToLowerInvariant ();

			int  pos   = 0;
			bool slash = true;

			buffer.Append (lower[pos]);
			
			while (++pos < lower.Length)
			{
				if (mixed[pos] != lower[pos])
				{
					if (slash)
					{
						buffer.Append ("/");
						slash = false;
					}
					else
					{
						buffer.Append ("-");
					}
				}

				buffer.Append (lower[pos]);
			}

			return buffer.ToString ();
		}

		private static Dictionary<string, MimeType> nameToType;
		private static Dictionary<MimeType, string> typeToName;
	}
}
