//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class JobPrintEngine : IPrintEngine
	{
		/// <summary>
		/// JobPrintEngine s'occupe de l'impression à proprement parler de sections.
		/// Un job est composé de sections. Une section est composée de pages contigües sur une seule unité d'impression
		/// Les sections utilisent toutes la même imprimante physique, mais peuvent utiliser plusieurs bacs différents.
		/// </summary>
		/// <param name="printDocument"></param>
		/// <param name="sections"></param>
		public JobPrintEngine(PrintDocument printDocument, List<SectionToPrint> sections)
		{
			this.printDocument = printDocument;
			this.sections      = sections;

			this.sectionIndex = 0;  // on commence par la première section
		}

		#region IPrintEngine Members
		public void PrepareNewPage(PageSettings settings)
		{
			var section = this.sections[this.sectionIndex];  // section <- section en cours d'impression

			PaperSource ps = System.Array.Find (printDocument.PrinterSettings.PaperSources, x => x.Name.Trim () == section.PrinterUnit.PhysicalPrinterTray.Trim ());
			if (ps != null)
			{
				settings.PaperSource = ps;
			}

			settings.Margins = new Margins (0);
		}

		public void FinishingPrintJob()
		{
		}

		public void StartingPrintJob()
		{
			var section = this.sections[this.sectionIndex];  // section <- section en cours d'impression

			section.DocumentPrinter.IsPreview = false;
			section.DocumentPrinter.CurrentPage = section.FirstPage;
		}

		public PrintEngineStatus PrintPage(PrintPort port)
		{
			var section = this.sections[this.sectionIndex];  // section <- section en cours d'impression

			Size size = section.DocumentPrinter.PageSize;
			double height = size.Height;
			double width  = size.Width;

			double xOffset = section.PrinterUnit.XOffset;
			double yOffset = section.PrinterUnit.YOffset;

			Transform transform;

			if (section.DocumentPrinter.PageSize.Width < section.DocumentPrinter.PageSize.Height)  // portrait ?
			{
				transform = Transform.Identity;
			}
			else  // paysage ?
			{
				transform = Transform.CreateRotationDegTransform (90, section.DocumentPrinter.PageSize.Height/2, section.DocumentPrinter.PageSize.Height/2);
			}

			port.Transform = transform.MultiplyByPostfix (Transform.CreateTranslationTransform (xOffset, yOffset));

			section.DocumentPrinter.PrintBackgroundCurrentPage (port);
			section.DocumentPrinter.PrintForegroundCurrentPage (port);

			section.DocumentPrinter.CurrentPage++;  // page suivante dans la section

			if (section.DocumentPrinter.CurrentPage < section.FirstPage+section.PageCount)  // section pas terminée ?
			{
				return PrintEngineStatus.MorePages;
			}

			this.sectionIndex++;  // section suivante ?

			if (this.sectionIndex < this.sections.Count)  // pas la dernière section ?
			{
				section = this.sections[this.sectionIndex];  // section <- nouvelle section à imprimer

				section.DocumentPrinter.IsPreview = false;
				section.DocumentPrinter.CurrentPage = section.FirstPage;

				return PrintEngineStatus.MorePages;
			}

			return PrintEngineStatus.FinishJob;
		}
		#endregion

		private readonly PrintDocument			printDocument;
		private readonly List<SectionToPrint>	sections;

		private int sectionIndex;
	}
}
