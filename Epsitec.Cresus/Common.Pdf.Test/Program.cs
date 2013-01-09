using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf;

namespace Common.Pdf.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine ("Début du test de Epsitec.Common.Pdf");

			var info = new ExportPdfInfo ();
			var export = new Export (info);
			var message = export.ExportToFile ("test.pdf", 2, Program.Renderer);

			if (string.IsNullOrEmpty (message))
			{
				Console.WriteLine ("Export ok");
			}
			else
			{
				Console.WriteLine ("message = " + message);
			}

			Console.WriteLine ("Fin du test de Epsitec.Common.Pdf");
			Console.ReadLine ();
		}

		private static void Renderer(Port port, int page)
		{
			if (page == 1)
			{
				Program.Renderer1 (port);
			}

			if (page == 2)
			{
				Program.Renderer2 (port);
			}
		}

		private static void Renderer1(Port port)
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

			{
				port.Color = Color.FromName ("Blue");
				port.PaintText (100.0, 300.0, "Tralala en bleu", Font.GetFont ("Arial", "Regular"), 100.0);
			}

			{
				port.Color = Color.FromName ("Black");
				port.PaintText (100.0, 400.0, "Tralala en noir", Font.GetFont ("Times New Roman", "Regular"), 50.0);
			}
		}

		private static void Renderer2(Port port)
		{
			{
				port.Color = Color.FromName ("Red");
				port.PaintText (100.0, 100.0, "Arial immense", Font.GetFont ("Arial", "Regular"), 500.0);
			}

			{
				port.Color = Color.FromName ("Black");
				port.PaintText (100.0, 400.0, "Times grand", Font.GetFont ("Times New Roman", "Regular"), 300.0);
			}
		}
	}
}
