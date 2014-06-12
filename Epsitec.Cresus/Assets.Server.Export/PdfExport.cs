//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Array;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Cresus.Assets.Export.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Exportation au format pdf.
	/// </summary>
	public class PdfExport<T> : AbstractExport<T>
		where T : struct
	{
		public override void Export(ExportInstructions instructions, AbstractExportProfile profile, AbstractTreeTableFiller<T> filler, ColumnsState columnsState)
		{
			this.humanFormat = true;

			base.Export (instructions, profile, filler, columnsState);

			this.FillArray (false);
			this.ExportPdf ();
		}


		private void ExportPdf()
		{
			var info = new ExportPdfInfo ()
			{
				PageSize = this.PageSize,
			};

			var setup = new ArraySetup ()
			{
				PageMargins          = this.PageMargins,
				CellMargins          = this.CellMargins,
				BorderThickness      = this.BorderThickness,
				LabelBackgroundColor = this.LabelBackgroundColor,
				EvenBackgroundColor  = this.EvenBackgroundColor, 
				OddBackgroundColor   = this.OddBackgroundColor,
				BorderColor          = this.BorderColor,
			};

			if (!string.IsNullOrEmpty (this.Profile.Header))
			{
				setup.HeaderText    = this.GetSizedText (this.Profile.Header, 1.5);
				setup.HeaderMargins = this.HeaderMargins;
			}

			if (!string.IsNullOrEmpty (this.Profile.Footer))
			{
				setup.FooterText    = this.GetSizedText (this.Profile.Footer, 1.5);
				setup.FooterMargins = this.FooterMargins;
			}

			var array = new Array (info, setup);

			if (!string.IsNullOrEmpty (this.Profile.Watermark))
			{
				array.AddWatermark (this.Profile.Watermark);
			}

			if (!this.Profile.AutomaticColumnWidths)
			{
				this.totalColumnWidths = this.TotalColumnWidths;
			}

			var columns = new List<ColumnDefinition> ();
			columns.AddRange (this.ColumnDefinitions);

			array.GeneratePdf (this.instructions.Filename, this.rowCount, columns, this.GetCellContent);
		}


		private CellContent GetCellContent(int row, int column)
		{
			//	Appelé par la génération du pdf (en callback) pour obtenir le contenu d'une cellule.
			var content = this.array[column, row];

			if (column == 0)  // seule la première colonne peut être en mode 'tree'
			{
				int level = this.levels[row];
				if (level > 0)
				{
					content = this.GetLeveledText (content, level);
				}
			}

			return new CellContent (this.GetSizedText (content));
		}

		private int TotalColumnWidths
		{
			//	Retourne la somme de toutes les largeurs de colonnes définies par l'utilisateur dans
			//	le TreeTable, utile en mode AutomaticColumnWidths == false.
			get
			{
				return this.columnsState.Columns.Select (x => x.FinalWidth).Sum ();
			}
		}

		private IEnumerable<ColumnDefinition> ColumnDefinitions
		{
			//	Retourne toutes les définitions de colonnes.
			get
			{
				var columnsState = this.columnsState.Columns.ToArray ();
				var columnDescriptions = this.filler.Columns;

				for (int abs=0; abs<columnsState.Length; abs++)
				{
					var mapped = this.columnsState.AbsoluteToMapped (abs);
					var columnState = columnsState[mapped];
					if (!columnState.Hide)
					{
						var description = columnDescriptions.Where (x => x.Field == columnState.Field).FirstOrDefault ();
						yield return this.GetColumnDefinition (description, columnState.FinalWidth);
					}
				}
			}
		}

		private ColumnDefinition GetColumnDefinition(TreeTableColumnDescription description, int width)
		{
			var alignment = this.GetContentAlignment (description);

			if (this.Profile.AutomaticColumnWidths)
			{
				var columnType = this.GetColumnType (description);
				return new ColumnDefinition (this.GetSizedText (description.Header), columnType, alignment: alignment);
			}
			else
			{
				double pageWidth = this.Profile.PageSize.Width - this.Profile.PageMargins.Width;

				double absoluteWidth = width * 10.0 * pageWidth / this.totalColumnWidths;
				return new ColumnDefinition (this.GetSizedText (description.Header), ColumnType.Absolute, absoluteWidth, alignment: alignment);
			}
		}

		private ColumnType GetColumnType(TreeTableColumnDescription description)
		{
			return ColumnType.Automatic;
		}

		private ContentAlignment GetContentAlignment(TreeTableColumnDescription description)
		{
			//	Retourne le mode d'alignement d'une colonne. Les nombres sont alignés à droite.
			switch (description.Type)
			{
				case TreeTableColumnType.AmortizedAmount:
				case TreeTableColumnType.Amount:
				case TreeTableColumnType.ComputedAmount:
				case TreeTableColumnType.Decimal:
				case TreeTableColumnType.DetailedAmortizedAmount:
				case TreeTableColumnType.DetailedComputedAmount:
				case TreeTableColumnType.Int:
				case TreeTableColumnType.Rate:
					return ContentAlignment.TopRight;

				default:
					return ContentAlignment.TopLeft;
			}
		}

		private Margins PageMargins
		{
			//	Retourne les marges de la page en dixièmes de millimètres.
			get
			{
				return new Margins
				(
					this.Profile.PageMargins.Left   * 10.0,
					this.Profile.PageMargins.Right  * 10.0,
					this.Profile.PageMargins.Top    * 10.0,
					this.Profile.PageMargins.Bottom * 10.0
				);
			}
		}

		private Margins CellMargins
		{
			//	Retourne les marges d'une cellule en dixièmes de millimètres.
			get
			{
				return new Margins
				(
					this.Profile.CellMargins.Left   * 10.0,
					this.Profile.CellMargins.Right  * 10.0,
					this.Profile.CellMargins.Top    * 10.0,
					this.Profile.CellMargins.Bottom * 10.0
				);
			}
		}

		private Margins HeaderMargins
		{
			get
			{
				return new Margins (0, 0, 0, this.Profile.FontSize*2.0);
			}
		}

		private Margins FooterMargins
		{
			get
			{
				return new Margins (0, 0, this.Profile.FontSize*2.0, 0);
			}
		}

		private double BorderThickness
		{
			//	Retourne la largeur des traits.
			get
			{
				return this.Profile.Style.BorderThickness * 10.0;
			}
		}

		private Color LabelBackgroundColor
		{
			//	Retourne la couleur pour les en-têtes.
			get
			{
				return this.Profile.Style.LabelColor.GetColor ();
			}
		}

		private Color EvenBackgroundColor
		{
			//	Retourne la couleur pour les lignes paires.
			get
			{
				return this.Profile.Style.EvenColor.GetColor ();
			}
		}

		private Color OddBackgroundColor
		{
			//	Retourne la couleur pour les lignes impaires.
			get
			{
				return this.Profile.Style.OddColor.GetColor ();
			}
		}

		private Color BorderColor
		{
			//	Retourne la couleur pour les traits.
			get
			{
				return this.Profile.Style.BorderColor.GetColor ();
			}
		}

		private Size PageSize
		{
			//	Retourne les dimensions d'une page en dixièmes de millimètres.
			get
			{
				return new Size
				(
					this.Profile.PageSize.Width  * 10.0,
					this.Profile.PageSize.Height * 10.0
				);
			}
		}


		private string GetLeveledText(string text, int level)
		{
			//	Retourne un texte précédé 'level' fois du motif d'indentation.
			var builder = new System.Text.StringBuilder ();

			for (int i=0; i<level; i++)
			{
				builder.Append (this.Profile.Indent);
			}

			builder.Append (text);

			return builder.ToString ();
		}

		private string GetSizedText(string text, double scale = 1.0)
		{
			//	Retourne un texte enrichi de tags pour déterminer la taille de la police.
			var fontSize = this.Profile.FontSize * scale * 3.2;  // 3.2 -> facteur empyrique, pour matcher sur les tailles usuelles dans Word
			var size = fontSize.ToString (System.Globalization.CultureInfo.InvariantCulture);
			return string.Format ("<font size=\"{0}\">{1}</font>", size, text);
		}


		private PdfExportProfile Profile
		{
			get
			{
				return this.profile as PdfExportProfile;
			}
		}


		private int								totalColumnWidths;
	}
}