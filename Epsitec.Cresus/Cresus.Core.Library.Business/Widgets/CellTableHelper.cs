//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Classe d'extension pour Epsitec.Common.Widgets.CellTable.
	/// </summary>
	public static class CellTableHelper
	{
		public static void FillRow(this CellTable table, int row, params ContentAlignment[] alignments)
		{
			//	Peuple une ligne d'une table avec des StaticText, si nécessaire.
			for (int column = 0; column < alignments.Count (); column++)
			{
				if (table[column, row].IsEmpty)
				{
					var text = new StaticText
					{
						ContentAlignment = alignments[column],
						TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
						Dock = DockStyle.Fill,
						Margins = new Margins (4, 4, 0, 0),
					};

					table[column, row].Insert (text);
				}
			}
		}

		public static void ClearRow(this CellTable table, int row)
		{
			//	Efface le contenu d'une ligne d'une table avec des textes taggés.
			for (int column = 0; column < table.Columns; column++)
			{
				var text = table[column, row].Children[0] as StaticText;
				text.Text = null;
			}
		}

		public static void UpdateRow(this CellTable table, int row, params FormattedText[] values)
		{
			//	Met à jour le contenu d'une ligne d'une table avec des textes taggés.
			for (int column = 0; column < values.Count (); column++)
			{
				var text = table[column, row].Children[0] as StaticText;
				text.FormattedText = values[column];
			}
		}

		public static void UpdateRow(this CellTable table, int row, params string[] values)
		{
			//	Met à jour le contenu d'une ligne d'une table avec des textes simples.
			for (int column = 0; column < values.Count (); column++)
			{
				var text = table[column, row].Children[0] as StaticText;
				text.Text = values[column];
			}
		}

		public static StaticText GetStaticText(this CellTable table, int row, int column)
		{
			if (row < table.Rows && column < table.Columns)
			{
				return table[column, row].Children[0] as StaticText;
			}
			else
			{
				return null;
			}
		}
	}
}
