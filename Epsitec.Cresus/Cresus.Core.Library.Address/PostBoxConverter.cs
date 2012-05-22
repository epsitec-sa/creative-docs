//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Library.Address
{
	public static class PostBoxConverter
	{
		public static string MergePostBox(string prefix, int? number, string suffix)
		{
			var value = InvariantConverter.ConvertToString (number);
			return string.Concat (prefix, value, suffix);
		}

		public static System.Tuple<string, int?, string> SplitPostBox(string value)
		{
			var extractor = new CharacterExtractor (value);

			string prefix = extractor.GetNextText ();
			int?   number = extractor.GetNextDigits ();
			string suffix = extractor.GetNextText ();

			return new System.Tuple<string, int?, string> (prefix, number, suffix);
		}
	}
}
