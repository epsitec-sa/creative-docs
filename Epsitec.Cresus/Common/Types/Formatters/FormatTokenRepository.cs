//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Formatters
{
	public static class FormatTokenRepository
	{
		/// <summary>
		/// Gets all known <see cref="FormatToken"/> instances. This is thread-safe.
		/// </summary>
		public static IEnumerable<FormatToken>	Items
		{
			get
			{
				return FormatTokenRepository.items;
			}
		}


		/// <summary>
		/// Initializes the <see cref="FormatTokenRepository"/> class and fills the collection
		/// of the sorted <see cref="FormatToken"/> instances.
		/// </summary>
		static FormatTokenRepository()
		{
			//	Tokens will be sorted from longest to shortest ("yyyy", then "yy") so that
			//	they can be evaluated one after the other in order to find the proper match:

			var sortedTokens = FormatterHelper.GetTokens ()
				.OrderByDescending (x => x.FormatString.Length)
				.ThenBy (x => x.FormatString);

			FormatTokenRepository.items = new List<FormatToken> (sortedTokens);
		}


		private readonly static List<FormatToken> items;
	}
}
