﻿//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Array;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Labels;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Common.Types;

using System;
using System.Collections.Generic;

namespace Common.Pdf.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();

			while (true)
			{
				Console.WriteLine ("0) tous les documents ci-dessous");
				Console.WriteLine ("1) 2 pages basiques");
				Console.WriteLine ("2) 6 pages contenant 100 étiquettes");
				Console.WriteLine ("3) 4 pages contenant un tableau de 100 lignes");
				Console.WriteLine ("4) 4 pages contenant un tableau de 50 lignes");
				Console.WriteLine ("5) 6 pages contenant un texte plein");
				Console.WriteLine ("6) 4 pages contenant un texte plein");
				string choice = Console.ReadLine ();

				int result = 1;
				if (!int.TryParse (choice, out result))
				{
					break;
				}

				try
				{
					string path = null;

					switch (result)
					{
						case 0:
							Program.Test1 ();
							Program.Test2 ();
							Program.Test3 ();
							Program.Test4 ();
							Program.Test5 ();
							Program.Test6 ();
							break;
						case 1:
							path = Program.Test1 ();
							break;
						case 2:
							path = Program.Test2 ();
							break;
						case 3:
							path = Program.Test3 ();
							break;
						case 4:
							path = Program.Test4 ();
							break;
						case 5:
							path = Program.Test5 ();
							break;
						case 6:
							path = Program.Test6 ();
							break;
					}

					if (path != null)
					{
						System.Diagnostics.Process.Start (path);
					}

					Console.WriteLine ("Export ok");
				}
				catch (PdfExportException ex)
				{
					Console.WriteLine ("message = " + ex.Message);
				}

				Console.WriteLine ("");
			}

			Console.WriteLine ("Fin du test de Epsitec.Common.Pdf");
		}


		private static string Test1()
		{
			//	Génération d'un document fixe de 2 pages.
			var info = new ExportPdfInfo ();
			var export = new Export (info);
			var path = "test1.pdf";
			export.ExportToFile (path, 2, Program.Renderer1);
			return path;
		}

		private static void Renderer1(Port port, int page)
		{
			if (page == 1)
			{
				Program.Renderer11 (port);
			}

			if (page == 2)
			{
				Program.Renderer12 (port);
			}
		}

		private static void Renderer11(Port port)
		{
			{
				var style = new TextStyle ()
				{
					Font = Font.GetFont ("Times New Roman", "Regular"),
					FontSize = 50.0,
				};

				port.PaintText (new Rectangle (100.0, 2800.0, 2000, 100), FormattedText.FromSimpleText ("Petit tralala en Times..."), style);
			}

			{
				var style = new TextStyle ()
				{
					Font = Font.GetFont ("Arial", "Regular"),
					FontSize = 100.0,
				};

				port.PaintText (new Rectangle (100.0, 2700.0, 2000, 100), FormattedText.FromSimpleText ("Grand tralala en Arial..."), style);
			}

			Program.RenderRectangle (port, new Rectangle (100, 2500, 100, 100), Color.FromName ("Red"));
			Program.RenderRectangle (port, new Rectangle (100, 1900, 500, 500), Color.FromName ("Green"));

			Program.RenderRectangle (port, new Rectangle (80, 980, 1040, 184), Color.FromName ("Yellow"));
			{
				var image = Bitmap.FromFile (Program.logo);
				port.PaintImage (image, new Rectangle (100.0, 1000.0, 1000, 144));
			}
			Program.RenderRectangle (port, new Rectangle (520, 850, 400, 400), Color.FromAlphaColor (0.2, Color.FromName ("Blue")));
		}

		private static void Renderer12(Port port)
		{
			Program.PaintTextBox (port, new Rectangle (100, 2000, 1000, 500), Program.histoire, 30);
			Program.PaintTextBox (port, new Rectangle (100, 1400, 800, 400), Program.histoire, 40);
			Program.PaintTextBox (port, new Rectangle (100, 800, 500, 500), "Voici un texte contenant un mot assez long anti-constitutionnellement pour forcer les césures dans les mots.", 40);

			{
				var style = new TextStyle ()
				{
					Font = Font.GetFont ("Times New Roman", "Regular"),
					FontSize = 100.0,
				};

				port.PaintText (new Rectangle (100.0, 300.0, 2000, 100), "Times <font size=\"150\">grand</font> et <font color=\"#ff0000\">rouge</font>, super !", style);
			}

			{
				var style = new TextStyle ()
				{
					Font = Font.GetFont ("Arial", "Regular"),
					FontSize = 50.0,
				};

				port.PaintText (new Rectangle (100.0, 100.0, 2000, 100), "Arial avec un mot en <b>gras</b> et un autre en <i>italique</i>.", style);
			}
		}

		private static void PaintTextBox(Port port, Rectangle box, FormattedText text, double fontSize)
		{
			{
				var path = new Path ();
				path.MoveTo (box.Left - 10.0, box.Top + 10.0);
				path.LineTo (box.Left - 10.0, box.Bottom - 20.0);
				path.LineTo (box.Right + 20.0, box.Bottom - 20.0);
				path.LineTo (box.Right + 20.0, box.Top + 10.0);
				path.Close ();

				port.LineWidth = 2.0;
				port.Color = Color.FromName ("Blue");
				port.PaintOutline (path);
			}

			var style = new TextStyle
			{
				Font       = Font.GetFont("Arial", "Regular"),
				FontSize   = fontSize,
				Alignment  = ContentAlignment.BottomLeft,
				BreakMode  = TextBreakMode.Hyphenate,
				JustifMode = TextJustifMode.All,
			};

			port.PaintText (box, text, style);
		}


		private static string Test2()
		{
			//	Génération d'étiquettes.
			var info = new ExportPdfInfo ()
			{
				PrintCropMarks = true,
			};

			var setup = new LabelPageLayout ()
			{
				ShouldPaintFrame = true,
			};

			var labels = new LabelGenerator (info, setup, new LabelRenderer ());
			Program.AddFixElements (labels, setup);

			var path = "test2.pdf";
			labels.GeneratePdf (path, 100, Program.Test2DataAccessor, () => FormattedText.FromSimpleText ("test"));
			return path;
		}

		private static FormattedText Test2DataAccessor(int rank)
		{
			return string.Format ("Monsieur<br/>Jean-Paul <b>Dupond</b> #{0}<br/>Place du Marché 45<br/>1000 Lausanne", (rank+1).ToString ());
		}


		private static string Test3()
		{
			//	Génération d'un tableau.
			var info = new ExportPdfInfo ()
			{
				PageSize = new Size (2970.0, 2100.0),  // A4 horizontal
			};

			var setup = new ArraySetup ()
			{
				EvenBackgroundColor = Color.FromAlphaColor(0.1, Color.FromHexa ("ffff00")),  // jaune 
				OddBackgroundColor  = Color.FromAlphaColor(0.1, Color.FromHexa ("00aaff")),  // bleu
			};
			Program.AddHeaderAndFooter (setup);

			var array = new Epsitec.Common.Pdf.Array.Array (info, setup);
			Program.AddFixElements (array, setup);

			var columns = new List<ColumnDefinition> ();
			columns.Add (new ColumnDefinition ("N°",       ColumnType.Absolute, absoluteWidth: 100.0, alignment: ContentAlignment.TopRight));
			columns.Add (new ColumnDefinition ("Titre",    ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Nom",      ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Prénom",   ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Adresse",  ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("NPA",      ColumnType.Automatic, alignment: ContentAlignment.TopRight));
			columns.Add (new ColumnDefinition ("Ville",    ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Remarque", ColumnType.Stretch, fontSize: 20.0));

			var path = "test3.pdf";
			array.GeneratePdf (path, 100, columns, Program.TestArrayDataAccessor);
			return path;
		}

		private static string Test4()
		{
			//	Génération d'un tableau.
			var info = new ExportPdfInfo ()
			{
			};

			var setup = new ArraySetup ()
			{
				PageMargins = new Margins(100.0),
			};
			setup.TextStyle.FontSize = 40.0;
			Program.AddHeaderAndFooter (setup);

			var array = new Epsitec.Common.Pdf.Array.Array (info, setup);
			Program.AddFixElements (array, setup);

			var columns = new List<ColumnDefinition> ();
			columns.Add (new ColumnDefinition ("N°",       ColumnType.Stretch, stretchFactor: 1.0, alignment: ContentAlignment.TopCenter));
			columns.Add (new ColumnDefinition ("Titre",    ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Nom",      ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Prénom",   ColumnType.Automatic));
			columns.Add (new ColumnDefinition ("Adresse",  ColumnType.Stretch, stretchFactor: 2.0));
			columns.Add (new ColumnDefinition ("NPA",      ColumnType.Automatic, alignment: ContentAlignment.BottomRight));
			columns.Add (new ColumnDefinition ("Ville",    ColumnType.Automatic, alignment: ContentAlignment.BottomLeft));

			var path = "test4.pdf";
			array.GeneratePdf (path, 100, columns, Program.TestArrayDataAccessor);
			return path;
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
						return new CellContent ("Julie", Color.FromAlphaColor(0.2, Color.FromName ("Red")));
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
					return new CellContent (Program.histoire);
				}

				if (row == 2 || row == 12 || row == 99)
				{
					return new CellContent ("À modifier...");
				}

				if ((row >= 14 && row <= 20) || row == 52 || row == 98)
				{
					return new CellContent (string.Format ("<img src=\"{0}\" width=\"50\" height=\"50\"/> Changement d'adresse", Program.warning));
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


		private static string Test5()
		{
			//	Génération d'un tableau.
			var info = new ExportPdfInfo ()
			{
			};

			var setup = new TextDocumentSetup ()
			{
				PageMargins = new Margins (250.0),
			};
			setup.TextStyle.FontSize = 52.0;
			Program.AddHeaderAndFooter (setup);

			var doc = new TextDocument (info, setup);
			Program.AddFixElements (doc, setup);

			string h = Program.histoire.Replace ("assis-debout", "<i>assis-debout</i>").Replace ("descendaient", "<i>descendaient</i>");

			var builder = new System.Text.StringBuilder ();
			for (int i=0; i<20; i++)
			{
				builder.Append (string.Format ("<font size=\"80\"><b>#{0}</b></font><br/>", (i+1).ToString ()));
				builder.Append (h);

				if (i==4)
				{
					builder.Append (string.Format ("<br/><img src=\"{0}\" width=\"600\" height=\"600\"/><br/>", Program.warning));
				}
				else
				{
					builder.Append ("<br/><br/>");
				}
			}

			var path = "test5.pdf";
			doc.GeneratePdf (path, builder.ToString ());
			return path;
		}


		private static string Test6()
		{
			//	Génération d'un tableau.
			var info = new ExportPdfInfo ()
			{
			};

			var setup = new TextDocumentSetup ()
			{
				PageMargins = new Margins (250.0),
			};
			setup.TextStyle.Font = Font.GetFont ("Times New Roman", "Regular");
			setup.TextStyle.FontSize = 48.0;
			Program.AddHeaderAndFooter (setup);

			var doc = new Epsitec.Common.Pdf.TextDocument.TextDocument (info, setup);
			Program.AddFixElements (doc, setup);

			string h = Program.histoire.Replace ("assis-debout", "<i>assis-debout</i>").Replace ("descendaient", "<i>descendaient</i>");

			var builder = new System.Text.StringBuilder ();
			for (int i=0; i<20; i++)
			{
				builder.Append (string.Format ("<font size=\"80\"><b>#{0}</b></font><br/>", (i+1).ToString ()));
				builder.Append (string.Format ("<font size=\"{0}\">", (20+i*2).ToString ()));
				builder.Append (h);
				builder.Append ("<br/></font><br/>");
			}

			var path = "test6.pdf";
			doc.GeneratePdf (path, builder.ToString ());
			return path;
		}


		private static void RenderRectangle(Port port, Rectangle rect, Color color)
		{
			var path = new Path ();
			path.MoveTo (rect.Left, rect.Bottom);
			path.LineTo (rect.Left, rect.Top);
			path.LineTo (rect.Right, rect.Top);
			path.LineTo (rect.Right, rect.Bottom);
			path.Close ();

			port.Color = color;
			port.PaintSurface (path);
		}

		private static void AddFixElements(CommonPdf common, CommonSetup setup)
		{
			common.AddWatermark ("SPECIMEN");

			var style = new TextStyle (setup.TextStyle)
			{
				FontSize = 20.0,
			};

			var tagImage = string.Format ("<img src=\"{0}\" width=\"200.0\" height=\"28.8\"/>", Program.logo);
			common.AddTopLeftLayer (tagImage, 50.0, style: style);
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
	}
}
