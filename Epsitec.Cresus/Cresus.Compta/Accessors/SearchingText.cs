//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Entities;

namespace Epsitec.Cresus.Compta.Accessors
{
	public class SearchingText
	{
		public SearchingText(ComptaEntity comptaEntity)
		{
			this.comptaEntity = comptaEntity;
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
					this.PreparesSearching ();
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
					this.PreparesSearching ();
				}
			}
		}

		public SearchingMode Mode
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
					this.PreparesSearching ();
				}
			}
		}

		public bool ConvertDiacritics
		{
			get
			{
				return this.convertDiacritics;
			}
			set
			{
				if (this.convertDiacritics != value)
				{
					this.convertDiacritics = value;
					this.PreparesSearching ();
				}
			}
		}

		public bool ConvertCase
		{
			get
			{
				return this.convertCase;
			}
			set
			{
				if (this.convertCase != value)
				{
					this.convertCase = value;
					this.PreparesSearching ();
				}
			}
		}




		public void Clear()
		{
			this.fromText          = null;
			this.toText            = null;
			this.mode              = SearchingMode.Fragment;
			this.convertDiacritics = true;
			this.convertCase       = true;

			this.PreparesSearching ();
		}

		public bool IsEmpty
		{
			get
			{
				if (this.mode == SearchingMode.Interval)
				{
					return string.IsNullOrEmpty (this.fromText) && string.IsNullOrEmpty (this.toText);
				}
				else if (this.mode == SearchingMode.Empty)
				{
					return false;
				}
				else
				{
					return string.IsNullOrEmpty (this.fromText);
				}
			}
		}


		public int Search(ref FormattedText target)
		{
			//	Effectue une recherche et retourne le nombre d'occurences trouvées.

			//	Attention: Les mécanismes ci-dessous sont totalement incompatibles avec des textes contenants des tags !
			string simple = target.ToSimpleText ();

			if (!string.IsNullOrEmpty (simple) && simple.StartsWith (StringArray.SpecialContentStart))
			{
				return 0;
			}

			int count = 0;

			if (this.mode == SearchingMode.WholeContent)
			{
				count = this.WholeContentSearch (ref simple);

				if (count != 0)
				{
					target = simple;
				}
			}
			else if (this.mode == SearchingMode.StartsWith)
			{
				count = this.StartsWithSearch (ref simple);

				if (count != 0)
				{
					target = simple;
				}
			}
			else if (this.mode == SearchingMode.EndsWith)
			{
				count = this.EndsWithSearch (ref simple);

				if (count != 0)
				{
					target = simple;
				}
			}
			else if (this.mode == SearchingMode.Interval)
			{
				count = this.IntervalSearch (ref simple);

				if (count != 0)
				{
					target = simple;
				}
			}
			else if (this.mode == SearchingMode.Empty)
			{
				if (string.IsNullOrEmpty (simple))
				{
					count = 1;
				}
			}
			else  // SearchingMode.Fragment ou SearchingMode.WholeWord ?
			{
				count = this.FragmentSearch (ref simple);

				if (count != 0)
				{
					target = simple;
				}
			}

			return count;
		}

		private int FragmentSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				string prepared = this.PrepareForSearching (target);

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
						if (this.mode == SearchingMode.WholeWord)  // mot entier ?
						{
							//	Vérifie la présence d'un séparateur de mots avant la chaîne.
							if (i > 0 && !SearchingText.IsWordSeparator (prepared[i-1]))
							{
								i++;
								continue;
							}

							//	Vérifie la présence d'un séparateur de mots après la chaîne.
							if (i+this.preparedFromText.Length < prepared.Length && !SearchingText.IsWordSeparator (prepared[i+this.preparedFromText.Length]))
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
				string prepared = this.PrepareForSearching (target);

				if (prepared.StartsWith (this.preparedFromText))
				{
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
				string prepared = this.PrepareForSearching (target);

				if (prepared.EndsWith (this.preparedFromText))
				{
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
				string prepared = this.PrepareForSearching (target);

				if (prepared == this.preparedFromText)
				{
					count = 1;
					target = TextFormatter.FormatText (target).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
				}
			}

			return count;
		}

		private int IntervalSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				if (this.preparedFromDecimal.HasValue && this.preparedToDecimal.HasValue)
				{
					decimal value;
					if (decimal.TryParse (target, out value))
					{
						if (value >= this.preparedFromDecimal && value <= this.preparedToDecimal)
						{
							count++;
						}
					}
				}
				else if (this.preparedFromDate.HasValue && this.preparedToDate.HasValue)
				{
					var date = this.ParseDate (target);
					if (date.HasValue && date.Value >= this.preparedFromDate && date.Value <= this.preparedToDate)
					{
						count++;
					}
				}

				if (count != 0)
				{
					target = TextFormatter.FormatText (target).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ().ToString ();
				}
			}

			return count;
		}


		private void PreparesSearching()
		{
			this.preparedFromText = this.PrepareForSearching (this.fromText);
			this.preparedToText   = this.PrepareForSearching (this.toText);

			this.preparedFromDecimal = null;
			this.preparedToDecimal   = null;

			this.preparedFromDate = null;
			this.preparedToDate   = null;

			if (!string.IsNullOrEmpty (this.fromText))
			{
				decimal value;
				if (decimal.TryParse (this.fromText, out value))
				{
					this.preparedFromDecimal = value;
				}
				else
				{
					this.preparedFromDate = this.ParseDate (this.fromText);
				}
			}

			if (!string.IsNullOrEmpty (this.toText))
			{
				decimal value;
				if (decimal.TryParse (this.toText, out value))
				{
					this.preparedToDecimal = value;
				}
				else
				{
					this.preparedToDate = this.ParseDate (this.toText);
				}
			}
		}

		private string PrepareForSearching(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				if (this.convertDiacritics)
				{
					text = SearchingText.RemoveDiacritics (text);
				}

				if (this.convertCase)
				{
					text = text.ToLower();
				}
			}

			return text;
		}

		private static string RemoveDiacritics(string text)
		{
			string norm = text.Normalize (System.Text.NormalizationForm.FormD);
			var builder = new System.Text.StringBuilder ();

			for (int i = 0; i < norm.Length; i++)
			{
				var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory (norm[i]);
				if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					builder.Append (norm[i]);
				}
			}

			return builder.ToString ().Normalize (System.Text.NormalizationForm.FormC);
		}


		private Date? ParseDate(string text)
		{
			System.DateTime d;
			if (System.DateTime.TryParse (text, out d))
			{
				return new Date (d);
			}

			return null;
		}


		private readonly ComptaEntity	comptaEntity;

		private string					fromText;
		private string					toText;
		private SearchingMode			mode;
		private bool					convertDiacritics;
		private bool					convertCase;

		private string					preparedFromText;
		private string					preparedToText;
		private decimal?				preparedFromDecimal;
		private decimal?				preparedToDecimal;
		private Date?					preparedFromDate;
		private Date?					preparedToDate;
	}
}
