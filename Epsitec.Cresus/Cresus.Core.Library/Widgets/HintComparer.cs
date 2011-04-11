//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>HintComparer</c> class provides support methods used in conjunction with
	/// <see cref="AutoCompleteTextField"/>, for instance.
	/// </summary>
	public static class HintComparer
	{
		/// <summary>
		/// Compares two texts.
		/// </summary>
		/// <param name="text">The source text.</param>
		/// <param name="candidate">The candidate text (usually what was typed by the user).</param>
		/// <returns>The result of the comparison.</returns>
		public static HintComparerResult Compare(string text, string candidate)
		{
			int index = (text == null) ? -1 : text.IndexOf (candidate);

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

		/// <summary>
		/// Gets the best result between two comparison results.
		/// </summary>
		/// <param name="result1">The first result.</param>
		/// <param name="result2">The second result.</param>
		/// <returns>The best result (e.g. <c>PrimaryMatch</c> if passed <c>PrimaryMatch</c> and <c>SecondaryMatch</c>).</returns>
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

		/// <summary>
		/// Gets a comparable text; this will remove all accents and convert the text to
		/// lower-case, so that an ordinal compare can be applied.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The comparable text</returns>
		public static string GetComparableText(string text)
		{
			return TextConverter.ConvertToLowerAndStripAccents (text);
		}
	}
}
