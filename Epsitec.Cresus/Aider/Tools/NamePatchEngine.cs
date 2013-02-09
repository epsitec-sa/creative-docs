using Epsitec.Common.Support.Extensions;

using System;

using System.Linq;

namespace Epsitec.Aider.Tools
{
	public static class NamePatchEngine
	{
		public static string SanitizeCapitalization(string name)
		{
			// This method is really simplistic. It will transform names which are all upper case
			// to names in lower case with the first letter upper case. It handles names with spaces
			// and with '-'. However, it won't handle names like "VON SIEBENTHAL" where the "V"
			// should be lower case. And it won't convert "HERVE" to "Hervé" because it can't know
			// about accents.
			// But it's still better than nothing and the input data is wrong about this anyway.

			if (string.IsNullOrEmpty (name))
			{
				return name;
			}

			if (!name.IsAllUpperCase ())
			{
				return name;
			}

			var separators = new char[] { ' ', '-', };
			var chars = name.ToCharArray ();

			for (int i = 0; i < chars.Length; i++)
			{
				if (i > 0 && !separators.Contains (chars[i-1]))
				{
					chars[i] = char.ToLower (chars[i]);
				}
			}

			return new String (chars);
		}
	}
}
