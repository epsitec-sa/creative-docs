using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// La classe Misc contient des fonctions de conversion
	/// </summary>
	class Misc
	{

		public static string StringNull(string value)
		{
			return value ?? "~";
		}

		public static string NullString(string value)
		{
			if (value == "~")
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		public static byte BoolToByte(bool value)
		{
			if (value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public static bool ByteToBool(byte value)
		{
			System.Diagnostics.Debug.Assert(value == 0 || value == 1) ;
			return value == 1;
		}

		/// <summary>
		/// Convertit un string en double. Si la conversion foire, retourne 0
		/// </summary>
		/// <param name="str">string à convertir</param>
		/// <returns>valeur convertie</returns>
		public static double ParseDouble(string str)
		{
			double value;
			double.TryParse (str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
			return value;
		}

		/// <summary>
		/// Convertit un string en int. Si la conversion foire, retourne 0
		/// En plus le nombre peut s'arrêter sur un caractère non numérique
		/// </summary>
		/// <param name="str">string à convertir</param>
		/// <returns>valeur convertie</returns>
		public static int ParseInt(string str)
		{
			int value;

			int i ;

			for (i = str.Length - 1; i >= 0; i--)
			{
				if (char.IsDigit (str[i]))
					break;
			}

			str = str.Substring (0, i + 1);

			int.TryParse (str, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out value);

			return value;
		}
	}
}
