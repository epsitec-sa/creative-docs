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
