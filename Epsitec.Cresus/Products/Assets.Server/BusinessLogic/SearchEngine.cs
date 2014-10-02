//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Moteur général de comparaison de chaînes. Toutes les options ne sont pas encore
	/// implémentées. A voir au fur et à mesure des besoins.
	/// </summary>
	public class SearchEngine
	{
		public SearchEngine(SearchDefinition definition)
		{
			//	Avec pattern = "les", IsMatching retournera true avec text = "Salut les copains"
			//	(avec les options par défaut).
			this.definition = definition;

			if (this.definition.IsActive)
			{
				if ((this.definition.Options & SearchOptions.Regex) != 0)
				{
					try
					{
						var pattern = TextConverter.ConvertToSimpleText (this.definition.Pattern);
						this.regexPattern = new Regex (pattern, RegexOptions.Compiled);
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
		}

		public bool IsMatching(string text)
		{
			if (this.definition.IsActive)
			{
				text = this.ProcessString (text);

				if ((this.definition.Options & SearchOptions.Regex) != 0)
				{
					return this.IsMatchingRegex (text);
				}
				else if ((this.definition.Options & SearchOptions.Prefix) != 0)
				{
					return this.IsMatchingPrefix (text);
				}
				else if ((this.definition.Options & SearchOptions.Sufffix) != 0)
				{
					return this.IsMatchingSuffix (text);
				}
				else if ((this.definition.Options & SearchOptions.FullText) != 0)
				{
					return this.IsMatchingFullText (text);
				}
				else
				{
					return this.IsMatchingFragment (text);
				}
			}
			else
			{
				return false;
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


		private bool IsMatchingFragment(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				if ((this.definition.Options & SearchOptions.WholeWords) != 0)
				{
					return this.GetWords (text).Contains (this.stringPattern);
				}
				else
				{
					return text.Contains (this.stringPattern);
				}
			}
		}

		private bool IsMatchingPrefix(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				if ((this.definition.Options & SearchOptions.WholeWords) != 0)
				{
					return this.GetWords (text).First () == this.stringPattern;
				}
				else
				{
					int index = text.IndexOf (this.stringPattern);
					return index == 0;
				}
			}
		}

		private bool IsMatchingSuffix(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				if ((this.definition.Options & SearchOptions.WholeWords) != 0)
				{
					return this.GetWords (text).Last () == this.stringPattern;
				}
				else
				{
					int index = text.IndexOf (this.stringPattern);
					return index != -1
						&& index == text.Length - this.stringPattern.Length;
				}
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

		private bool IsMatchingRegex(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				text = TextConverter.ConvertToSimpleText (text);
				return this.regexPattern.IsMatch (text);
			}
		}


		private string[] GetWords(string text)
		{
			return text.Split (
				' ', '.', ',', ':', ';', '!', '?', '"',
				'+', '-', '*', '/', '=', '<', '>',
				'(', ')', '[', ']');
		}

		private string ProcessString(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				//	Converti "&amp;" en "&". Sans cela, le texte "R&D" (codé de façon
				//	interne en "R&amp;D") n'est pas trouvé en mode SearchOptions.WholeWords,
				//	puisqu'il serait considéré comme ayant les 2 mots "R&amp" et "D" !
				text = TextConverter.ConvertToSimpleText (text);

				if ((this.definition.Options & SearchOptions.IgnoreCase) != 0)
				{
					text = text.ToLowerInvariant ();
				}

				if ((this.definition.Options & SearchOptions.IgnoreDiacritic) != 0)
				{
					text = ApproximativeSearching.RemoveDiatritic (text);
				}

				if ((this.definition.Options & SearchOptions.Phonetic) != 0)
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
