//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Epsitec.Cresus.Assets.Core.Helpers;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Moteur général de comparaison de chaînes. Tous les modes ne sont pas encore
	/// implémentés. A voir au fur et à mesure des besoins.
	/// </summary>
	public class SearchEngine
	{
		public SearchEngine(string filter, SearchMode mode = SearchMode.IgnoreCase | SearchMode.IgnoreDiacritic | SearchMode.Fragment)
		{
			this.mode = mode;

			if ((this.mode & SearchMode.Regex) != 0)
			{
				try
				{
					this.regexFilter = new Regex (filter, RegexOptions.Compiled);
				}
				catch
				{
					this.regexFilter = null;
				}
			}
			else
			{
				this.processFilter = this.Process (filter);
			}
		}

		public bool IsMatching(string text)
		{
			if (!this.processFilter.IsEmpty)
			{
				var richText = this.Process (text);

				if ((this.mode & SearchMode.FullText) != 0)
				{
					return this.IsMatchingFullText (richText);
				}
				else if ((this.mode & SearchMode.Fragment) != 0)
				{
					return this.IsMatchingFragment (richText);
				}
				else if ((this.mode & SearchMode.Regex) != 0)
				{
					return this.IsMatchingRegex (richText);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}


		private bool IsMatchingFullText(RichText text)
		{
			if (this.processFilter.Amount.HasValue)
			{
				return text.Amount == this.processFilter.Amount;
			}
			else
			{
				return text.Text == this.processFilter.Text;
			}
		}

		private bool IsMatchingFragment(RichText text)
		{
			if (this.processFilter.Amount.HasValue)
			{
				return text.Amount == this.processFilter.Amount;
			}
			else
			{
				return text.Text.Contains (this.processFilter.Text);
			}
		}

		private bool IsMatchingRegex(RichText text)
		{
			if (this.regexFilter == null)
			{
				return false;
			}
			else
			{
				return this.regexFilter.IsMatch (text.Text);
			}
		}


		private RichText Process(string text)
		{
			decimal? amount = null;

			if (!string.IsNullOrEmpty (text))
			{
				if ((this.mode & SearchMode.IgnoreCase) != 0)
				{
					text = text.ToLowerInvariant ();
				}

				if ((this.mode & SearchMode.IgnoreDiacritic) != 0)
				{
					text = ApproximativeSearching.RemoveDiatritic (text);
				}

				if ((this.mode & SearchMode.Phonetic) != 0)
				{
					text = ApproximativeSearching.Phonetic (text);
				}

				amount = TypeConverters.ParseAmount (text);
			}

			return new RichText (text, amount);
		}


		private struct RichText
		{
			public RichText(string text, decimal? amount)
			{
				this.Text   = text;
				this.Amount = amount;
			}

			public readonly string				Text;
			public readonly decimal?			Amount;

			public bool IsEmpty
			{
				get
				{
					return string.IsNullOrEmpty (this.Text)
						&& !this.Amount.HasValue;
				}
			}
		}


		private readonly RichText				processFilter;
		private readonly Regex					regexFilter;
		private readonly SearchMode				mode;
	}
}
