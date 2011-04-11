//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Core.Widgets
{
	public static class HintComparer
	{
		public static HintComparerResult Compare(string text, string typed)
		{
			int index = (text == null) ? -1 : text.IndexOf (typed);

			if (index == -1)
			{
				return HintComparerResult.NoMatch;
			}
			else if (index == 0)
			{
				return HintComparerResult.PrimaryMatch;
			}
			else
			{
				return HintComparerResult.SecondaryMatch;
			}
		}

		public static HintComparerResult GetBestResult(HintComparerResult result1, HintComparerResult result2)
		{
			if (result1 < result2)
			{
				return result1;
			}
			else
			{
				return result2;
			}
		}

		public static string GetComparableText(string text)
		{
			return TextConverter.ConvertToLowerAndStripAccents (text);
		}
	}
}
