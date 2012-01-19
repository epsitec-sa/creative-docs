//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données d'une occurrence d'une recherche dans un texte formatté.
	/// </summary>
	public class SearchResult
	{
		public SearchResult(int row, ColumnType column, FormattedText text, SearchingTabData tab)
		{
			this.Row    = row;
			this.Column = column;

			if (tab.IsEmpty || text.IsNullOrEmpty)
			{
				return;
			}

			//	Attention: Les mécanismes ci-dessous sont totalement incompatibles avec des textes contenants des tags !
			string simple = text.ToSimpleText ();

			if (simple.StartsWith (StringArray.SpecialContentStart))
			{
				return;
			}

			//	Regarde si cela a un sens d'effectuer une recherche numérique.
			if (tab.HasSearchingDecimal)
			{
				decimal value;
				if (decimal.TryParse (simple, out value))
				{
					if (value >= tab.SearchingDecimalLow && value <= tab.SearchingDecimalHigh)
					{
						this.HilitedText = FormattedText.Concat (StringArray.SpecialContentSearchingTarget, TextFormatter.FormatText (simple).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ());
						this.Count = 1;
					}

					return;
				}
			}

			//	Effectue une recherche textuelle.
			string prepared = SearchResult.PrepareForSearching (simple);

			int i = 0;
			while (i < simple.Length)
			{
				i = prepared.IndexOf (tab.SearchingPreparedText, i);

				if (i == -1)
				{
					break;
				}
				else
				{
					this.Count++;

					var r = TextFormatter.FormatText (simple.Substring (i, tab.SearchingText.Length)).ApplyFontColor (SearchResult.TextInsideSearch).ApplyBold ();

					prepared = prepared.Substring (0, i) + r + prepared.Substring (i+tab.SearchingText.Length);
					simple   =   simple.Substring (0, i) + r +   simple.Substring (i+tab.SearchingText.Length);

					i += r.Length;
				}
			}

			if (this.Count > 0)
			{
				this.HilitedText = FormattedText.Concat (StringArray.SpecialContentSearchingTarget, TextFormatter.FormatText (simple).ApplyFontColor (SearchResult.TextOutsideSearch));
			}
		}

		public int Row
		{
			get;
			internal set;
		}

		public ColumnType Column
		{
			get;
			internal set;
		}

		public int Count
		{
			get;
			internal set;
		}

		public FormattedText HilitedText
		{
			get;
			internal set;
		}


		public static string PrepareForSearching(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				return SearchResult.RemoveDiacritics (text.ToLower ());
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


		public static readonly Color TextInsideSearch  = Color.FromBrightness (0);    // noir
		public static readonly Color TextOutsideSearch = Color.FromBrightness (0.6);  // gris
		public static readonly Color BackInsideSearch  = Color.FromHexa ("fff000");   // jaune pétant
		public static readonly Color BackOutsideSearch = Color.FromAlphaColor (0.1, Color.FromHexa ("fff000"));   // jaune très transparent
		//?public static readonly Color BackOutsideSearch = Color.Empty;
	}
}