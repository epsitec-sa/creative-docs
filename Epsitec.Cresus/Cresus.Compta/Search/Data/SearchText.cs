//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Text.RegularExpressions;

namespace Epsitec.Cresus.Compta.Search.Data
{
	public class SearchText
	{
		public SearchText()
		{
			this.Clear ();
		}


		public string FromText
		{
			get
			{
				return this.fromText;
			}
			set
			{
				if (this.fromText != value)
				{
					this.fromText = value;
					this.PreparesSearch ();
				}
			}
		}

		public string ToText
		{
			get
			{
				return this.toText;
			}
			set
			{
				if (this.toText != value)
				{
					this.toText = value;
					this.PreparesSearch ();
				}
			}
		}

		public SearchMode Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				if (this.mode != value)
				{
					this.mode = value;
					this.PreparesSearch ();
				}
			}
		}

		public bool MatchCase
		{
			get
			{
				return this.matchCase;
			}
			set
			{
				if (this.matchCase != value)
				{
					this.matchCase = value;
					this.PreparesSearch ();
				}
			}
		}

		public bool WholeWord
		{
			get
			{
				return this.wholeWord;
			}
			set
			{
				if (this.wholeWord != value)
				{
					this.wholeWord = value;
					this.PreparesSearch ();
				}
			}
		}

		public bool Invert
		{
			get
			{
				return this.invert;
			}
			set
			{
				if (this.invert != value)
				{
					this.invert = value;
					this.PreparesSearch ();
				}
			}
		}


		public void Clear()
		{
			this.fromText  = null;
			this.toText    = null;
			this.mode      = SearchMode.Fragment;
			this.matchCase = false;
			this.wholeWord = false;
			this.invert    = false;

			this.PreparesSearch ();
		}

		public bool IsEmpty
		{
			get
			{
				if (this.mode == SearchMode.Interval)
				{
					return string.IsNullOrEmpty (this.fromText) && string.IsNullOrEmpty (this.toText);
				}
				else if (this.mode == SearchMode.Empty)
				{
					return false;
				}
				else
				{
					return string.IsNullOrEmpty (this.fromText);
				}
			}
		}

		public bool GetIntervalDates(out Date? beginDate, out Date? endDate)
		{
			beginDate = null;
			endDate   = null;

			if (this.mode == SearchMode.Interval && (this.preparedFromDate.HasValue || this.preparedToDate.HasValue))
			{
				if (this.preparedFromDate.HasValue)
				{
					beginDate = this.preparedFromDate.Value;
				}

				if (this.preparedToDate.HasValue)
				{
					endDate = this.preparedToDate.Value;
				}

				return true;
			}

			return false;
		}


		public int Search(ref FormattedText target)
		{
			//	Effectue une recherche et retourne le nombre d'occurences trouvées.

			//	Attention: Les mécanismes ci-dessous sont totalement incompatibles avec des textes contenants des tags !
			int count = 0;
			string simple = target.ToSimpleText ();

			if (string.IsNullOrEmpty (simple) || !simple.StartsWith (StringArray.SpecialContentStart))
			{
				if (this.mode == SearchMode.WholeContent)
				{
					count = this.WholeContentSearch (ref simple);

					if (count != 0)
					{
						target = simple;
					}
				}
				else if (this.mode == SearchMode.StartsWith)
				{
					count = this.StartsWithSearch (ref simple);

					if (count != 0)
					{
						target = simple;
					}
				}
				else if (this.mode == SearchMode.EndsWith)
				{
					count = this.EndsWithSearch (ref simple);

					if (count != 0)
					{
						target = simple;
					}
				}
				else if (this.mode == SearchMode.Interval)
				{
					count = this.IntervalSearch (ref simple);

					if (count != 0)
					{
						target = simple;
					}
				}
				else if (this.mode == SearchMode.Jokers)
				{
					count = this.RegExSearch (ref simple);

					if (count != 0)
					{
						target = simple;
					}
				}
				else if (this.mode == SearchMode.Empty)
				{
					if (string.IsNullOrEmpty (simple))
					{
						count = 1;
					}
				}
				else
				{
					count = this.FragmentSearch (ref simple);

					if (count != 0)
					{
						target = simple;
					}
				}
			}

			if (this.invert)
			{
				count = (count == 0) ? 1:0;
			}

			return count;
		}

		private int FragmentSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				string prepared = this.PreparesSearch (target);

				int i = 0;
				while (i < target.Length)
				{
					i = prepared.IndexOf (this.preparedFromText, i);

					if (i == -1)
					{
						break;
					}
					else
					{
						if (this.wholeWord)  // mot entier ?
						{
							//	Vérifie la présence d'un séparateur de mots avant la chaîne.
							if (i > 0 && !SearchText.IsWordSeparator (prepared[i-1]))
							{
								i++;
								continue;
							}

							//	Vérifie la présence d'un séparateur de mots après la chaîne.
							if (i+this.preparedFromText.Length < prepared.Length && !SearchText.IsWordSeparator (prepared[i+this.preparedFromText.Length]))
							{
								i++;
								continue;
							}
						}

						count++;

						var r = TextFormatter.FormatText (target.Substring (i, this.preparedFromText.Length)).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ();

						prepared = prepared.Substring (0, i) + r + prepared.Substring (i+this.preparedFromText.Length);
						target   =   target.Substring (0, i) + r +   target.Substring (i+this.preparedFromText.Length);

						i += r.Length;
					}
				}
			}

			return count;
		}

		private static bool IsWordSeparator(char c)
		{
			return !((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_'); 
		}

		private int StartsWithSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				string prepared = this.PreparesSearch (target);

				if (prepared.StartsWith (this.preparedFromText))
				{
					if (this.wholeWord)  // mot entier ?
					{
						//	Vérifie la présence d'un séparateur de mots après la chaîne.
						if (this.preparedFromText.Length < prepared.Length && !SearchText.IsWordSeparator (prepared[this.preparedFromText.Length]))
						{
							return count;
						}
					}

					count = 1;

					int i = this.preparedFromText.Length;
					var found = TextFormatter.FormatText (target.Substring (0, i)).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
					target = found + target.Substring (i);
				}
			}

			return count;
		}

		private int EndsWithSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				string prepared = this.PreparesSearch (target);

				if (prepared.EndsWith (this.preparedFromText))
				{
					if (this.wholeWord)  // mot entier ?
					{
						//	Vérifie la présence d'un séparateur de mots avant la chaîne.
						if (this.preparedFromText.Length < prepared.Length && !SearchText.IsWordSeparator (prepared[prepared.Length-this.preparedFromText.Length-1]))
						{
							return count;
						}
					}

					count = 1;

					int i = target.Length - this.preparedFromText.Length;
					var found = TextFormatter.FormatText (target.Substring (i)).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
					target = target.Substring (0, i) + found;
				}
			}

			return count;
		}

		private int WholeContentSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				string prepared = this.PreparesSearch (target);

				if (prepared == this.preparedFromText)
				{
					count = 1;
					target = TextFormatter.FormatText (target).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
				}
			}

			return count;
		}

		private int RegExSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				string prepared = this.PreparesSearch (target);

				if (this.preparedRegEx != null && this.preparedRegEx.IsMatch (prepared))
				{
					count = 1;
					target = TextFormatter.FormatText (target).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
				}
			}

			return count;
		}

		private int IntervalSearch(ref string target)
		{
			if (!string.IsNullOrEmpty (target))
			{
				if (this.preparedFromDecimal.HasValue || this.preparedToDecimal.HasValue)
				{
					decimal? value = Converters.ParseMontant (target);
					if (value.HasValue)
					{
						if (this.preparedFromDecimal.HasValue && value.Value < this.preparedFromDecimal.Value)
						{
							return 0;
						}

						if (this.preparedToDecimal.HasValue && value.Value > this.preparedToDecimal.Value)
						{
							return 0;
						}
					}
				}
				else if (this.preparedFromDate.HasValue || this.preparedToDate.HasValue)
				{
					var date = Converters.ParseDate (target);
					if (date.HasValue)
					{
						if (this.preparedFromDate.HasValue && date.Value < this.preparedFromDate.Value)
						{
							return 0;
						}

						if (this.preparedToDate.HasValue && date.Value > this.preparedToDate.Value)
						{
							return 0;
						}
					}
				}
			}

			target = TextFormatter.FormatText (target).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
			return 1;
		}


		private void PreparesSearch()
		{
			this.preparedFromText = this.PreparesSearch (this.fromText);
			this.preparedToText   = this.PreparesSearch (this.toText);

			this.preparedFromDecimal = null;
			this.preparedToDecimal   = null;

			this.preparedFromDate = null;
			this.preparedToDate   = null;

			this.preparedRegEx = null;

			if (!string.IsNullOrEmpty (this.fromText))
			{
				decimal? value = Converters.ParseMontant (this.fromText);
				if (value.HasValue)
				{
					this.preparedFromDecimal = value.Value;
				}
				else
				{
					this.preparedFromDate = Converters.ParseDate (this.fromText);
				}
			}

			if (!string.IsNullOrEmpty (this.toText))
			{
				decimal? value = Converters.ParseMontant (this.toText);
				if (value.HasValue)
				{
					this.preparedToDecimal = value.Value;
				}
				else
				{
					this.preparedToDate = Converters.ParseDate (this.toText);
				}
			}

			if (!string.IsNullOrEmpty (this.fromText))
			{
				var options = RegexFactory.Options.Compiled;

				if (!this.matchCase)
				{
					options |= RegexFactory.Options.IgnoreCase;
				}

				this.preparedRegEx = RegexFactory.FromSimpleJoker (this.fromText, options);
			}
		}

		private string PreparesSearch(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				if (!this.matchCase)
				{
					text = Converters.PreparingForSearh (text);
				}
			}

			return text;
		}


		public bool CompareTo(SearchText other)
		{
			return SearchText.CompareFormattedText (other.fromText, this.fromText)  &&
				   SearchText.CompareFormattedText (other.toText,   this.toText  )  &&
				   other.mode      == this.mode      &&
				   other.matchCase == this.matchCase &&
				   other.wholeWord == this.wholeWord &&
				   other.invert    == this.invert;
		}

		private static bool CompareFormattedText(FormattedText t1, FormattedText t2)
		{
			if (t1.Length == 0 && t2.Length == 0)
			{
				return true;
			}

			return t1 == t2;
		}

		public void CopyTo(SearchText dst)
		{
			dst.fromText  = this.fromText;
			dst.toText    = this.toText;
			dst.mode      = this.mode;
			dst.matchCase = this.matchCase;
			dst.wholeWord = this.wholeWord;
			dst.invert    = this.invert;

			dst.PreparesSearch ();
		}


		public FormattedText GetSummary(FormattedText columnName)
		{
			if (this.IsEmpty)
			{
				return FormattedText.Empty;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				if (!columnName.IsNullOrEmpty)
				{
					builder.Append (columnName);
				}

				if (this.invert)
				{
					builder.Append ("≠");
				}
				if (!this.invert && !columnName.IsNullOrEmpty)
				{
					builder.Append ("=");
				}

				if (this.mode == SearchMode.Empty)
				{
					builder.Append ("vide");
				}
				else if (this.mode == SearchMode.Interval)
				{
					builder.Append ("\"");
					builder.Append (this.fromText);
					builder.Append ("...");
					builder.Append (this.toText);
					builder.Append ("\"");
				}
				else
				{
					builder.Append ("\"");
					builder.Append (this.fromText);
					builder.Append ("\"");
				}

				return builder.ToString ();
			}
		}


		private string					fromText;
		private string					toText;
		private SearchMode				mode;
		private bool					matchCase;
		private bool					wholeWord;
		private bool					invert;

		private string					preparedFromText;
		private string					preparedToText;
		private decimal?				preparedFromDecimal;
		private decimal?				preparedToDecimal;
		private Date?					preparedFromDate;
		private Date?					preparedToDate;
		private Regex					preparedRegEx;
	}
}
