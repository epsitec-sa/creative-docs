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
		/// </summary>
		/// <param name="str">string à convertir</param>
		/// <returns>valeur convertie</returns>
		public static int ParseInt(string str)
		{
			int value = 0;

			try
			{
				value = int.Parse (str, System.Globalization.CultureInfo.InvariantCulture);
			}
			catch
			{

			}

			return value;
		}
	}
}
