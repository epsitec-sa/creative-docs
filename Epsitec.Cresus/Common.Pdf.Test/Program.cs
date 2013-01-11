//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Stikers;
using Epsitec.Common.Pdf.Array;
using Epsitec.Common.Types;

namespace Common.Pdf.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();

			Console.WriteLine ("1) 2 pages basiques");
			Console.WriteLine ("2) 6 pages contenant 100 étiquettes");
			Console.WriteLine ("3) 3 pages contenant un tableau de 100 lignes");
			string choice = Console.ReadLine ();

			int result = 1;
			int.TryParse (choice, out result);

			PdfExportException ex = null;

			switch (result)
			{
				case 1:
					ex = Program.Test1 ();
					break;
				case 2:
					ex = Program.Test2 ();
					break;
				case 3:
					ex = Program.Test3 ();
					break;
			}

			if (ex == null)
			{
				Console.WriteLine ("Export ok");
			}
			else
			{
				Console.WriteLine ("message = " + ex.Message);
			}

			Console.WriteLine ("Fin du test de Epsitec.Common.Pdf");
			Console.ReadLine ();
		}


		private static PdfExportException Test1()
		{
			//	Génération d'un document fixe de 2 pages.
			var info = new ExportPdfInfo ();
			var export = new Export (info);
			return export.ExportToFile ("test.pdf", 2, Program.Renderer1);
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
				var path = new Path ();
				path.MoveTo (100.0, 100.0);
				path.LineTo (100.0, 200.0);
				path.LineTo (200.0, 200.0);
				path.LineTo (200.0, 100.0);
				path.Close ();

				port.Color = Color.FromName ("Red");
				port.PaintSurface (path);
			}

			{
				var path = new Path ();
				path.MoveTo (100.0, 1000.0);
				path.LineTo (100.0, 2000.0);
				path.LineTo (1100.0, 2000.0);
				path.LineTo (1100.0, 1000.0);
				path.Close ();

				port.Color = Color.FromName ("Green");
				port.PaintSurface (path);
			}

			port.PaintText (100.0, 400.0, new Size (2000, 100), FormattedText.FromSimpleText ("Plus petit tralala..."), Font.GetFont ("Times New Roman", "Regular"), 50.0);
			port.PaintText (100.0, 300.0, new Size (2000, 100), FormattedText.FromSimpleText ("Grand tralala..."), Font.GetFont ("Arial", "Regular"), 100.0);
		}

		private static void Renderer12(Port port)
		{
			Program.PaintTextBox (port, new Rectangle (100, 2000, 1000, 500), Program.histoire, 30);
			Program.PaintTextBox (port, new Rectangle (100, 1400, 800, 400), Program.histoire, 40);
			Program.PaintTextBox (port, new Rectangle (100, 800, 500, 500), "Voici un texte contenant un mot assez long anti-constitutionnellement pour forcer les césures dans les mots.", 40);

			port.PaintText (100.0, 300.0, new Size (2000, 100), "Times <font size=\"150\">grand</font> et <font color=\"#ff0000\">rouge</font>, super !", Font.GetFont ("Times New Roman", "Regular"), 100.0);
			port.PaintText (100.0, 100.0, new Size (2000, 100), "Arial avec un mot en <b>gras</b> et un autre en <i>italique</i>.", Font.GetFont ("Arial", "Regular"), 50.0);
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
				Alignment  = ContentAlignment.BottomLeft,
				BreakMode  = TextBreakMode.Hyphenate,
				JustifMode = TextJustifMode.All,
			};

			port.PaintText (box.Left, box.Bottom, box.Size, text, Font.GetFont ("Arial", "Regular"), fontSize, style);
		}


		private static PdfExportException Test2()
		{
			//	Génération d'étiquettes.
			var stikers = new Stikers ();

			var info = new ExportPdfInfo ();

			var setup = new StikersSetup ()
			{
				PaintFrame = true,
			};

			return stikers.GeneratePdf("test.pdf", 100, Program.Test2Accessor, info, setup);
		}

		private static FormattedText Test2Accessor(int rank)
		{
			return string.Format ("Monsieur<br/>Jean <b>Dupond</b> #{0}<br/>Place du Marché 45<br/>1000 Lausanne", (rank+1).ToString ());
		}


		private static PdfExportException Test3()
		{
			//	Génération d'étiquettes.
			var array = new Epsitec.Common.Pdf.Array.Array ();

			var info = new ExportPdfInfo ()
			{
				PageSize = new Size (2970.0, 2100.0),  // A4 horizontal
			};

			var setup = new ArraySetup ()
			{
				Header = "<font size=\"400\">Tableau de test bidon</font><br/>Deuxième ligne de l'en-tête",
				Footer = "<i>Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland</i>",
			};

			var columns = new List<ColumnDefinition> ();
			columns.Add (new ColumnDefinition ("Titre",    200.0));
			columns.Add (new ColumnDefinition ("Nom",      300.0));
			columns.Add (new ColumnDefinition ("Prénom",   300.0));
			columns.Add (new ColumnDefinition ("Adresse",  300.0));
			columns.Add (new ColumnDefinition ("NPA",      150.0, ContentAlignment.TopRight));
			columns.Add (new ColumnDefinition ("Ville",    300.0));
			columns.Add (new ColumnDefinition ("Remarque", null, fontSize: 20.0));

			return array.GeneratePdf ("test.pdf", 100, columns, Program.Test3Accessor, info, setup);
		}

		private static FormattedText Test3Accessor(int row, int column)
		{
			if (row >= 20 && row <= 50)
			{
				switch (column)
				{
					case 0:
						return "Madame";
					case 1:
						return "Julie";
					case 2:
						return string.Format ("<i>Dubosson</i> #{0}", (row+1).ToString ());
				}
			}

			if (column == 6)
			{
				if (row == 10)
				{
					return Program.histoire;
				}

				if (row == 2 || row == 12)
				{
					return "À modifier";
				}
			}

			switch (column)
			{
				case 0:
					return "Monsieur";
				case 1:
					return "Jean";
				case 2:
					return string.Format("<b>Dupond</b> #{0}", (row+1).ToString ());
				case 3:
					return "Place du Marché 45";
				case 4:
					return (1000+row).ToString ();
				case 5:
					return "Lausanne";
			}

			return null;
		}


		private static string histoire = "Midi, l'heure du crime ! Un jeune vieillard assis-debout sur une pierre en bois lisait son journal plié en quatre dans sa poche à la lueur d'une bougie éteinte. Le tonnerre grondait en silence et les éclairs brillaient sombres dans la nuit claire. Il monta quatre à quatre les trois marches qui descendaient au grenier et vit par le trou de la serrure bouchée un nègre blanc qui déterrait un mort pour le manger vivant. N'écoutant que son courage de pleutre mal léché, il sortit son épée de fils de fer barbelés et leur coupa la tête au ras des pieds.";
	}
}
