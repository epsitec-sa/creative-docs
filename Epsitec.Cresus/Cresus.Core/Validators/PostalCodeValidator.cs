//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;


namespace Epsitec.Cresus.Core.Validators
{
	public static class PostalCodeValidator
	{
		public static bool Validate(string value)
		{
			//	Indique si un texte est un numéro postal suisse ou étranger.
			//	Accepte "1400", "CH-1023", "F-10123" et "US-55678", mais refuse "123", "12345" ou "F10123".
			if (string.IsNullOrEmpty (value))
			{
				return false;
			}

			return PostalCodeValidator.RegexNPA.IsMatch (value);
		}


		private static readonly Regex RegexNPA = new Regex (@"^[0-9]{4}$|^[a-zA-Z]{1,2}[-][0-9]{4,5}$", RegexOptions.Compiled);
	}
}
