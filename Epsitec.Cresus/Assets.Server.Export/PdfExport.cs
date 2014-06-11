//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Array;
using Epsitec.Common.Pdf.Engine;
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
				LabelBackgroundColor = Color.FromBrightness (0.9),
				EvenBackgroundColor  = this.EvenBackgroundColor, 
				OddBackgroundColor   = this.OddBackgroundColor,
			};

			var array = new Array (info, setup);

			if (!string.IsNullOrEmpty (this.Profile.Watermark))
			{
				array.AddWatermark (this.Profile.Watermark);
			}

			var columns = new List<ColumnDefinition> ();
			columns.AddRange (this.ColumnDefinitions);

			array.GeneratePdf (this.instructions.Filename, this.rowCount, columns, this.GetCellContent);
		}


		private CellContent GetCellContent(int row, int column)
		{
			var content = this.array[column, row];

			if (column == 0)
			{
				int level = this.levels[row];
				if (level > 0)
				{
					content = this.GetLeveledText (content, level);
				}
			}

			return new CellContent (this.GetSizedText (content));
		}

		private IEnumerable<ColumnDefinition> ColumnDefinitions
		{
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
						yield return this.GetColumnDefinition (description);
					}
				}
			}
		}

		private ColumnDefinition GetColumnDefinition(TreeTableColumnDescription description)
		{
			var columnType = this.GetColumnType (description);
			var alignment = this.GetContentAlignment (description);
			return new ColumnDefinition (this.GetSizedText (description.Header), columnType, alignment: alignment);
		}

		private ColumnType GetColumnType(TreeTableColumnDescription description)
		{
			return ColumnType.Automatic;
		}

		private ContentAlignment GetContentAlignment(TreeTableColumnDescription description)
		{
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
					return ContentAlignment.MiddleRight;

				default:
					return ContentAlignment.MiddleLeft;
			}
		}

		private Margins PageMargins
		{
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

		private Color EvenBackgroundColor
		{
			get
			{
				if (this.Profile.EvenOddGrey)
				{
					return Color.FromBrightness (0.95);
				}
				else
				{
					return Color.Empty;
				}
			}
		}

		private Color OddBackgroundColor
		{
			get
			{
				if (this.Profile.EvenOddGrey)
				{
					return Color.Empty;
				}
				else
				{
					return Color.Empty;
				}
			}
		}

		private Size PageSize
		{
			get
			{
				var w = this.Profile.PageSize.Width  * 10.0;
				var h = this.Profile.PageSize.Height * 10.0;

				return new Size (w, h);
			}
		}


		private string GetLeveledText(string text, int level)
		{
			var builder = new System.Text.StringBuilder ();

			for (int i=0; i<level; i++)
			{
				builder.Append (this.Profile.Indent);
			}

			builder.Append (text);

			return builder.ToString ();
		}

		private string GetSizedText(string text)
		{
			var fontSize = this.Profile.FontSize * 3.2;  // facteur empyrique, pour matcher sur les tailles usuelles dans Word
			var size = fontSize.ToString (System.Globalization.CultureInfo.InvariantCulture);
			return string.Format ("<font size=\"{0}\">{1}</font>", size, text);
		}


#if false
		private static void Test3(string filename)
		{
			//	Génération d'un tableau.
			var info = new ExportPdfInfo ()
			{
				PageSize = new Size (2970.0, 2100.0),  // A4 horizontal
			};

			var setup = new ArraySetup ()
			{
				EvenBackgroundColor = Color.FromAlphaColor (0.1, Color.FromHexa ("ffff00")),  // jaune 
				OddBackgroundColor  = Color.FromAlphaColor (0.1, Color.FromHexa ("00aaff")),  // bleu
			};
			PdfExport<T>.AddHeaderAndFooter (setup);

			var array = new Epsitec.Common.Pdf.Array.Array (info, setup);
			//?PdfExport<T>.AddFixElements (array, setup);

			var columns = new List<ColumnDefinition> ();
			columns.Add (new ColumnDefinition ("N°", ColumnType.Absolute, absoluteWidth: 100.0, alignment: ContentAlignment.TopRight));
			columns.Add (new ColumnDefinition ("Titre", ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Nom", ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Prénom", ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Adresse", ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("NPA", ColumnType.Automatic, alignment: ContentAlignment.TopRight));
			columns.Add (new ColumnDefinition ("Ville", ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Remarque", ColumnType.Stretch, fontSize: 20.0));

			array.GeneratePdf (filename, 100, columns, PdfExport<T>.TestArrayDataAccessor);
		}

		private static CellContent TestArrayDataAccessor(int row, int column)
		{
			if (row == 5 && column == 3)
			{
				return new CellContent ("<font size=\"50\"><b>Grand !</b></font>");
			}

			if (row == 55 && column == 5)
			{
				return new CellContent ("<font color=\"White\">F-75001</font>", Color.FromName ("Blue"));
			}

			if (row >= 20 && row <= 50)
			{
				switch (column)
				{
					case 1:
						return new CellContent ("Madame");
					case 2:
						return new CellContent ("Julie", Color.FromAlphaColor (0.2, Color.FromName ("Red")));
					case 3:
						return new CellContent ("<i>Dubosson</i>");
					case 4:
						return new CellContent ("Av. de la Gare 12<br/>Case postale 1234");
				}
			}

			if (column == 7)
			{
				if (row == 10)
				{
					return new CellContent (PdfExport<T>.histoire);
				}

				if (row == 2 || row == 12 || row == 99)
				{
					return new CellContent ("À modifier...");
				}

				if ((row >= 14 && row <= 20) || row == 52 || row == 98)
				{
					//?return new CellContent (string.Format ("<img src=\"{0}\" width=\"50\" height=\"50\"/> Changement d'adresse", PdfExport<T>.warning));
				}
			}

			switch (column)
			{
				case 0:
					return new CellContent ((row+1).ToString ());
				case 1:
					return new CellContent ("Monsieur");
				case 2:
					return new CellContent ("Jean-Paul");
				case 3:
					return new CellContent ("<b>Dupond</b>");
				case 4:
					return new CellContent ("Place du Marché 45");
				case 5:
					return new CellContent ((1000+row).ToString ());
				case 6:
					return new CellContent ("Lausanne");
			}

			return null;
		}

		private static void AddFixElements(CommonPdf common, CommonSetup setup)
		{
			common.AddWatermark ("SPECIMEN");

			var style = new TextStyle (setup.TextStyle)
			{
				FontSize = 20.0,
			};

			//?var tagImage = string.Format ("<img src=\"{0}\" width=\"200.0\" height=\"28.8\"/>", PdfExport<T>.logo);
			//?common.AddTopLeftLayer (tagImage, 50.0, style: style);
			common.AddTopCenterLayer ("<font color=\"Blue\">— Document test —</font>", 50.0, style: style);
			common.AddTopRightLayer ("<font color=\"Blue\">Crésus</font>", 50.0, style: style);

			common.AddBottomLeftLayer ("<font color=\"Blue\"><i>Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland</i></font>", 50.0, style: style);
			common.AddBottomRightLayer ("<font color=\"Blue\">Page {0}</font>", 50.0, style: style);
		}

		private static void AddHeaderAndFooter(CommonSetup setup)
		{
			const string headerText = "<font color=\"Red\"><font size=\"80\">En-tête</font><br/>Deuxième ligne de l'en-tête</font>";
			const string footerText = "<font color=\"Red\"><font size=\"80\">Pied de page</font><br/>Deuxième ligne du pied de page</font>";

			if (setup is ArraySetup)
			{
				var s = setup as ArraySetup;

				s.HeaderText = headerText;
				s.FooterText = footerText;
			}

			if (setup is TextDocumentSetup)
			{
				var s = setup as TextDocumentSetup;

				s.HeaderText = headerText;
				s.FooterText = footerText;
			}
		}

		private static string logo = "S:\\Epsitec.Cresus\\External\\epsitec.png";
		private static string warning = "S:\\Epsitec.Cresus\\External\\warning.tif";
		private static string histoire = "Midi, l'heure du crime ! Un jeune vieillard assis-debout sur une pierre en bois lisait son journal plié en quatre dans sa poche à la lueur d'une bougie éteinte. Le tonnerre grondait en silence et les éclairs brillaient sombres dans la nuit claire. Il monta quatre à quatre les trois marches qui descendaient au grenier et vit par le trou de la serrure bouchée un nègre blanc qui déterrait un mort pour le manger vivant. N'écoutant que son courage de pleutre mal léché, il sortit son épée de fils de fer barbelés et leur coupa la tête au ras des pieds.";
#endif


		private PdfExportProfile Profile
		{
			get
			{
				return this.profile as PdfExportProfile;
			}
		}
	}
}