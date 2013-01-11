using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Stikers;
using Epsitec.Common.Types;

namespace Common.Pdf.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();

			Console.WriteLine ("1) Document fixe de 2 pages");
			Console.WriteLine ("2) Etiquettes");
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

			var setup = new StikersSetup ()
			{
				PaintFrame = true,
			};

			return stikers.GeneratePdf("test.pdf", 100, Program.Test2Accessor, setup);
		}

		private static FormattedText Test2Accessor(int rank)
		{
			return string.Format ("Monsieur<br/>Jean <b>Dupond</b> #{0}<br/>Place du Marché 45<br/>1000 Lausanne", (rank+1).ToString ());
		}


		private static string histoire = "Midi, l'heure du crime ! Un jeune vieillard assis-debout sur une pierre en bois lisait son journal plié en quatre dans sa poche à la lueur d'une bougie éteinte. Le tonnerre grondait en silence et les éclairs brillaient sombres dans la nuit claire. Il monta quatre à quatre les trois marches qui descendaient au grenier et vit par le trou de la serrure bouchée un nègre blanc qui déterrait un mort pour le manger vivant. N'écoutant que son courage de pleutre mal léché, il sortit son épée de fils de fer barbelés et leur coupa la tête au ras des pieds.";
	}
}
