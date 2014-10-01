//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Moteur général de comparaison de chaînes. Tous les modes ne sont pas encore
	/// implémentés. A voir au fur et à mesure des besoins.
	/// </summary>
	public class SearchEngine
	{
		public SearchEngine(SearchDefinition definition)
		{
			//	Avec pattern = "les", IsMatching retournera true avec text = "Salut les copains"
			//	(avec le mode par défaut).
			this.definition = definition;

			if ((this.definition.Mode & SearchMode.Regex) != 0)
			{
				try
				{
					this.regexPattern = new Regex (this.definition.Pattern, RegexOptions.Compiled);
				}
				catch
				{
					this.regexPattern = null;
				}
			}
			else
			{
				this.stringPattern  = this.ProcessString (this.definition.Pattern);
				this.decimalPattern = TypeConverters.ParseAmount (this.definition.Pattern);
				this.intPattern     = TypeConverters.ParseInt (this.definition.Pattern);
				this.datePattern    = TypeConverters.ParseDate (this.definition.Pattern, Timestamp.Now.Date, null, null);
			}
		}

		public bool IsMatching(string text)
		{
			if (!string.IsNullOrEmpty (this.stringPattern))
			{
				text = this.ProcessString (text);

				if ((this.definition.Mode & SearchMode.FullText) != 0)
				{
					return this.IsMatchingFullText (text);
				}
				else if ((this.definition.Mode & SearchMode.Fragment) != 0)
				{
					return this.IsMatchingFragment (text);
				}
				else if ((this.definition.Mode & SearchMode.Regex) != 0)
				{
					return this.IsMatchingRegex (text);
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

		public bool IsMatching(decimal? value)
		{
			if (value.HasValue)
			{
				return value == this.decimalPattern;
			}
			else
			{
				return false;
			}
		}

		public bool IsMatching(int? value)
		{
			if (value.HasValue)
			{
				return value == this.intPattern;
			}
			else
			{
				return false;
			}
		}

		public bool IsMatching(System.DateTime? date)
		{
			if (date.HasValue)
			{
				return date == this.datePattern;
			}
			else
			{
				return false;
			}
		}


		private bool IsMatchingFullText(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				return text == this.stringPattern;
			}
		}

		private bool IsMatchingFragment(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				return text.Contains (this.stringPattern);
			}
		}

		private bool IsMatchingRegex(string text)
		{
			if (this.regexPattern == null)
			{
				return false;
			}
			else
			{
				return this.regexPattern.IsMatch (text);
			}
		}


		private string ProcessString(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				if ((this.definition.Mode & SearchMode.IgnoreCase) != 0)
				{
					text = text.ToLowerInvariant ();
				}

				if ((this.definition.Mode & SearchMode.IgnoreDiacritic) != 0)
				{
					text = ApproximativeSearching.RemoveDiatritic (text);
				}

				if ((this.definition.Mode & SearchMode.Phonetic) != 0)
				{
					text = ApproximativeSearching.Phonetic (text);
				}
			}

			return text;
		}


		private readonly SearchDefinition		definition;
		private readonly string					stringPattern;
		private readonly decimal?				decimalPattern;
		private readonly int?					intPattern;
		private readonly System.DateTime?		datePattern;
		private readonly Regex					regexPattern;
	}
}
