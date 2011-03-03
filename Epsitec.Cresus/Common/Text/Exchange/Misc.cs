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
#if false
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
#endif
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


		public static byte ParseByte(string str)
		{
			byte b ;
			byte.TryParse(str, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out b) ;
			return b;
		}

		/// <summary>
		/// Convertiti un string en bool. Retourne false pour "0" et true pour "1".
		/// Asserte pour toute autre chaîne que "0" ou "1"
		/// </summary>
		/// <param name="str"></param>
		/// <returns>tValeur convertie true ou false</returns>
		public static bool ParseBool(string str)
		{
			byte b;

			b = Misc.ParseByte (str);

			if (b == 0)
				return false;

			if (b == 1)
				return true;

			System.Diagnostics.Debug.Assert (false);
			return true;
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


		public static string NextElement(ref string line, char separator)
		{
			int index = line.IndexOf (separator);
			string retval ;

			if (index == -1)
				return "";

			retval = line.Substring(0, index) ;
			line = line.Substring (index + 1);

			return retval;
		}
	}
}
