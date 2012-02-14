//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Data
{
	/// <summary>
	/// Données d'une occurrence d'une recherche dans un texte formatté.
	/// </summary>
	public class SearchResult
	{
		public SearchResult(int row, ColumnType column, FormattedText hilitedText)
		{
			this.Row    = row;
			this.Column = column;

			this.HilitedText = FormattedText.Concat (StringArray.SpecialContentSearchTarget, Core.TextFormatter.FormatText (hilitedText).ApplyFontColor (UIBuilder.TextOutsideSearchColor));
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

		public FormattedText HilitedText
		{
			get;
			internal set;
		}
	}
}