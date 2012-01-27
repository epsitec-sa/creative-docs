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


		public void Clear()
		{
			this.fromText = null;
			this.toText   = null;
			this.mode     = SearchingMode.Normal;

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

			if (this.mode == SearchingMode.Exact)
			{
				count = this.ExactSearch (ref simple);

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
			else
			{
				count = this.NormalSearch (ref simple);

				if (count != 0)
				{
					target = simple;
				}
			}

			return count;
		}

		private int NormalSearch(ref string target)
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
						if (this.mode == SearchingMode.WholeWord)
						{
							// TODO: ...
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

		private int ExactSearch(ref string target)
		{
			int count = 0;

			if (!string.IsNullOrEmpty (target))
			{
				if (this.preparedFromDecimal.HasValue)
				{
					decimal value;
					if (decimal.TryParse (target, out value))
					{
						if (value == this.preparedFromDecimal)
						{
							count = 1;
						}
					}
				}
				else if (this.preparedFromDate.HasValue)
				{
					var date = this.ParseDate (target);
					if (date.HasValue && date.Value == this.preparedFromDate)
					{
						count = 1;
					}
				}
				else
				{
					if (target == this.preparedFromText)
					{
						count = 1;
					}
				}

				if (count != 0)
				{
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
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				if (this.mode == SearchingMode.Exact)
				{
					return text;
				}
				else
				{
					return SearchingText.RemoveDiacritics (text.ToLower ());
				}
			}
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

			return (builder.ToString ().Normalize (System.Text.NormalizationForm.FormC));
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

		private string					preparedFromText;
		private string					preparedToText;
		private decimal?				preparedFromDecimal;
		private decimal?				preparedToDecimal;
		private Date?					preparedFromDate;
		private Date?					preparedToDate;
	}
}
