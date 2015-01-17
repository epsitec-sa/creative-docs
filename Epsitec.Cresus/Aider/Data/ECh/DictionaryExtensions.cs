//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	internal static class DictionaryExtensions
	{
		public static void AddIfNotNull(this Dictionary<string, EChPerson> dictionary, EChPerson person)
		{
			if (person != null)
			{
				dictionary.Add (person.Id, person);
			}
		}
	}
}

