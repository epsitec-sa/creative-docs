//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Print.Serialization;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print
{
	public class XmlJobPrintEngine : IPrintEngine
	{
		/// <summary>
		/// JobPrintEngine2 s'occupe de l'impression à proprement parler de sections.
		/// Un job est composé de sections. Une section est composée de pages contigües sur une seule unité d'impression
		/// Les sections utilisent toutes la même imprimante physique, mais peuvent utiliser plusieurs bacs différents.
		/// </summary>
		/// <param name="printDocument"></param>
		/// <param name="sections"></param>
		public XmlJobPrintEngine(CoreData coreData, PrintDocument printDocument, List<DeserializedSection> sections)
		{
			this.coreData      = coreData;
			this.printDocument = printDocument;
			this.sections      = sections;

			this.sectionIndex = 0;  // on commence par la première section
			this.pageIndex    = 0;  // on commence par la première page
		}

		#region IPrintEngine Members
		public void PrepareNewPage(PageSettings pageSettings)
		{
			var section = this.sections[this.sectionIndex];  // section <- section en cours d'impression
			this.printingUnit = Common.GetPrintingUnit (section.PrinterLogicalName);

			if (!string.IsNullOrWhiteSpace (section.PrinterPhysicalTray))
			{
				PaperSource paperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, x => x.Name.Trim () == section.PrinterPhysicalTray.Trim ());
				if (paperSource != null)
				{
					pageSettings.PaperSource = paperSource;
				}
			}

			PaperSize paperSize = System.Array.Find (printDocument.PrinterSettings.PaperSizes, x => Common.PageSizeCompare (x.Size, section.PageSize) != Common.PageSizeCompareEnum.Different);
			if (paperSize != null)
			{
				pageSettings.PaperSize = paperSize;
				this.swap = Common.PageSizeCompare (pageSettings.PaperSize.Size, section.PageSize) == Common.PageSizeCompareEnum.Swaped ^ pageSettings.Landscape;
			}

			if (this.printingUnit != null)
			{
				// TODO: Hélas, cela semble n'avoir aucun effet lors d'une impression réelle !
				pageSettings.PrinterSettings.Duplex = this.printingUnit.PhysicalDuplexMode;
			}

			pageSettings.Margins = new Margins (0);
		}

		public void FinishingPrintJob()
		{
		}

		public void StartingPrintJob()
		{
		}

		public PrintEngineStatus PrintPage(PrintPort port)
		{
			var section = this.sections[this.sectionIndex];  // section <- section en cours d'impression
			var page = section.Pages[this.pageIndex];

			//	Prépare le port graphique (translation et rotation).
			double xOffset = 0;
			double yOffset = 0;

			if (this.printingUnit != null)
			{
				xOffset = this.printingUnit.XOffset;
				yOffset = this.printingUnit.YOffset;
			}

			if (this.swap)  // format inversé ?
			{
				port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationDegTransform (90, section.PageSize.Height/2, section.PageSize.Height/2));
			}

			port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateTranslationTransform (xOffset, yOffset));

			//	Dessine la page à imprimer.
			var xmlPort = new XmlPort (page.XRoot);
			xmlPort.Deserialize (id => PrintEngine.GetImage (this.coreData, id), port);

			//	Cherche la page suivante.
			this.pageIndex++;

			if (this.pageIndex >= section.Pages.Count)
			{
				this.pageIndex = 0;

				this.sectionIndex++;

				if (this.sectionIndex >= this.sections.Count)
				{
					return PrintEngineStatus.FinishJob;
				}
			}

			return PrintEngineStatus.MorePages;
		}
		#endregion


		private readonly CoreData					coreData;
		private readonly PrintDocument				printDocument;
		private readonly List<DeserializedSection>	sections;

		private int									sectionIndex;
		private int									pageIndex;
		private PrintingUnit						printingUnit;
		private bool								swap;
	}
}
