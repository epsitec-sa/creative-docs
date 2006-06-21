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

		public static string StringNull(string thestring)
		{
			if (thestring == null)
				return "~";
			else
				return thestring;
		}

		public static string NullString(string thestring)
		{
			if (thestring == "~")
				return null;
			else
				return thestring;
		}

		public static byte boolTobyte(bool thebool)
		{
			if (thebool)
				return 1;
			else
				return 0;
		}

		public static bool byteTobool(byte thebyte)
		{
			System.Diagnostics.Debug.Assert(thebyte == 0 || thebyte == 1) ;
			return thebyte == 1;
		}

		/// <summary>
		/// Convertit un string en double. Si la conversion foire, retourne 0
		/// </summary>
		/// <param name="str">string à convertir</param>
		/// <returns>valeur convertie</returns>
		public static double ParseDouble(string str)
		{
			double value = 0.0;

			try
			{
				value = double.Parse (str, System.Globalization.CultureInfo.InvariantCulture);
			}
			catch
			{
				
			}

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

			try
			{
				value = int.Parse (str, System.Globalization.CultureInfo.InvariantCulture);
			}
			catch
			{
				value = 0;
			}

			return value;
		}
	}
}
