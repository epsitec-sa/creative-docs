//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Strings
	{
		public static string AddThousandSeparators(string text, string separator)
		{
			//	Si separator contient "'" :
			//	"123"       -> "123"
			//	"1234"      -> "1'234"
			//	"123456789" -> "123'456'789"
			if (!string.IsNullOrEmpty (text) && text.Length > 3)
			{
				var list = new List<string> ();

				while (text.Length != 0)
				{
					int length = System.Math.Min (text.Length, 3);
					list.Add (text.Substring (text.Length-length, length));
					text = text.Substring (0, text.Length-length);
				}

				list.Reverse ();
				return string.Join (separator, list);
			}

			return text;
		}


		public static string ComputeMd5Hash(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				byte[] array = System.Text.Encoding.ASCII.GetBytes (text);
				return Epsitec.Common.IO.Checksum.ComputeMd5Hash (array);
			}
		}

		public static string GetStandardPassword(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				return new string ('●', 8);
			}
		}

		public static string ConvertToHidePassword(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				return new string ('●', text.Length);
			}
		}
	}
}
