//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class EncodingHelpers
	{
		public static string Labels
		{
			//	Retourne le texte pour peupler le ComboStackedController.
			get
			{
				return string.Join ("<br/>", EncodingHelpers.Encodings.Select (x => EncodingHelpers.GetEncodingName (x)));
			}
		}

		public static Encoding IntToEncoding(int? value)
		{
			if (value.HasValue)
			{
				var e = EncodingHelpers.Encodings.ToArray ();

				if (value >= 0 && value < e.Length)
				{
					return e[value.Value];
				}
			}

			return Encoding.Default;
		}

		public static int? EncodingToInt(Encoding encoding)
		{
			int value = 0;

			foreach (var e in EncodingHelpers.Encodings)
			{
				if (e == encoding)
				{
					return value;
				}

				value++;
			}

			return null;
		}


		private static string GetEncodingName(Encoding encoding)
		{
			if (encoding == Encoding.UTF7)
			{
				return "UTF-7";
			}
			else if (encoding == Encoding.UTF8)
			{
				return "UTF-8";
			}
			else if (encoding == Encoding.UTF32)
			{
				return "UTF-32";
			}
			else if (encoding == Encoding.Unicode)
			{
				return "Unicode (normal)";
			}
			else if (encoding == Encoding.BigEndianUnicode)
			{
				return "Unicode (big endian)";
			}
			else if (encoding == Encoding.ASCII)
			{
				return "ASCII";
			}
			else
			{
				return null;
			}
		}

		private static IEnumerable<Encoding> Encodings
		{
			//	Retourne la liste des encodages supportés, dans l'ordre où ils apparaissent
			//	dans la UI.
			get
			{
				yield return Encoding.UTF7;
				yield return Encoding.UTF8;
				yield return Encoding.UTF32;
				yield return Encoding.Unicode;
				yield return Encoding.BigEndianUnicode;
				yield return Encoding.ASCII;
			}
		}
	}
}