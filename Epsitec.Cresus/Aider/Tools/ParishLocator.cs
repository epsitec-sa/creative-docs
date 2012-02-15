//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Tools
{
	internal sealed class ParishLocator
	{
		private ParishLocator()
		{
		}

		public string GetParishName(int zipCode, string streetName, int streetNumber)
		{
			return "";
		}


		public static readonly ParishLocator	Instance = new ParishLocator ();
	}
}
