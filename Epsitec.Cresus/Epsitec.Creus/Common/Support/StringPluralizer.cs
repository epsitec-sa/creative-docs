//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	public static class StringPluralizer
	{
		public static IEnumerable<string> GuessPluralForms(string word)
		{
			if (word.EndsWith ("s"))
			{
				yield return word;
				yield break;
			}
			if (word.EndsWith ("y"))
			{
				yield return word.Substring (0, word.Length-1) + "ies";
				yield break;
			}
			if (word.EndsWith ("um"))
			{
				yield return word.Substring (0, word.Length-2) + "a";
			}

			yield return word + "s";
		}
	}
}
