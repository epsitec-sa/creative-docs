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
			//?System.Diagnostics.Debug.WriteLine (string.Format("row={0} column={1} text={2}", row, column, text));
			this.Row    = row;
			this.Column = column;

			if (tab.IsEmpty)
			{
				return;
			}

			this.Count = tab.SearchingText.Search (ref text);

			if (this.Count != 0)
			{
				this.HilitedText = FormattedText.Concat (StringArray.SpecialContentSearchingTarget, TextFormatter.FormatText (text).ApplyFontColor (SearchResult.TextOutsideSearch));
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


		public static readonly Color TextInsideSearch  = Color.FromBrightness (0);    // noir
		public static readonly Color TextOutsideSearch = Color.FromBrightness (0.6);  // gris
		public static readonly Color BackInsideSearch  = Color.FromHexa ("fff000");   // jaune pétant
		public static readonly Color BackOutsideSearch = Color.FromAlphaColor (0.1, Color.FromHexa ("fff000"));   // jaune très transparent
		//?public static readonly Color BackOutsideSearch = Color.Empty;
	}
}