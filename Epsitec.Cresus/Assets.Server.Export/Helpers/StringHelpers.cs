//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Export.Helpers
{
	public static class StringHelpers
	{
		public static string ToCamelCase(this string text)
		{
			//	Transforme "valeur comptable" en "ValeurComptable".
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();
				bool upper = true;

				foreach (char c in text)
				{
					if (c == ' ')
					{
						upper = true;
					}
					else
					{
						if (upper)
						{
							builder.Append (c.ToString ().ToUpper ());
							upper = false;
						}
						else
						{
							builder.Append (c);
						}
					}
				}

				return builder.ToString ();
			}
		}
	}
}
