//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Documents.Verbose;

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Print.Serialization
{
	public static class SerializationEngine
	{
		public static string SerializeJobs(List<JobToPrint> jobs)
		{
			//	Retourne la chaîne xmlSource qui sérialise une liste de jobs d'impression.
			if (jobs == null)
			{
				return null;
			}

			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			var xDocument = new XDocument
			(
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp)
			);

			var xRoot = new XElement ("jobs");
			xDocument.Add (xRoot);

			foreach (var job in jobs)
			{
				List<SectionToPrint> sectionsToPrint = job.Sections.Where (x => x.Enable).ToList ();

				if (sectionsToPrint.Count != 0)
				{
					var xJob = new XElement ("job");
					xJob.Add (new XAttribute ("title", job.JobFullName));
					xJob.Add (new XAttribute ("printer-name", job.PrinterPhysicalName));

					foreach (var section in sectionsToPrint)
					{
						var xSection = new XElement ("section");
						xSection.Add (new XAttribute ("printer-unit",     section.PrintingUnit.LogicalName));
						xSection.Add (new XAttribute ("printer-tray",     section.PrintingUnit.PhysicalPrinterTray));
						xSection.Add (new XAttribute ("printer-x-offset", section.PrintingUnit.XOffset));
						xSection.Add (new XAttribute ("printer-y-offset", section.PrintingUnit.YOffset));
						xSection.Add (new XAttribute ("printer-width",    section.PageSize.Width));
						xSection.Add (new XAttribute ("printer-height",   section.PageSize.Height));

						for (int page = section.FirstPage; page < section.FirstPage+section.PageCount; page++)
						{
							var xPage = new XElement ("page");
							xPage.Add (new XAttribute ("rank", page));

							var port = new XmlPort (xPage);
							section.DocumentPrinter.CurrentPage = page;
							section.DocumentPrinter.SetPrintingUnit (section.PrintingUnit, section.Options, PreviewMode.Print);
							section.DocumentPrinter.BuildSections ();
							section.DocumentPrinter.PrintBackgroundCurrentPage (port);
							section.DocumentPrinter.PrintForegroundCurrentPage (port);

							xSection.Add (xPage);
						}

						xJob.Add (xSection);
					}

					xRoot.Add (xJob);
				}
			}

			return xDocument.ToString (SaveOptions.None);
		}

		public static List<DeserializedJob> DeserializeJobs(IBusinessContext businessContext, string xmlSource, double zoom=0)
		{
			//	Désérialise une liste de jobs d'impression.
			//	Si le zoom est différent de zéro, on génère des bitmaps miniatures des pages.
			var jobs = new List<DeserializedJob> ();

			if (!string.IsNullOrWhiteSpace (xmlSource))
			{
				XDocument doc = XDocument.Parse (xmlSource, LoadOptions.None);
				XElement root = doc.Element ("jobs");

				foreach (var xJob in root.Elements ())
				{
					string title               = (string) xJob.Attribute ("title");
					string printerPhysicalName = (string) xJob.Attribute ("printer-name");

					var job = new DeserializedJob (title, printerPhysicalName);

					foreach (var xSection in xJob.Elements ())
					{
						string printerLogicalName  = (string) xSection.Attribute ("printer-unit");
						string printerPhysicalTray = (string) xSection.Attribute ("printer-tray");
						double width               = (double) xSection.Attribute ("printer-width");
						double height              = (double) xSection.Attribute ("printer-height");

						var section = new DeserializedSection (job, printerLogicalName, printerPhysicalTray, new Size (width, height));

						foreach (var xPage in xSection.Elements ())
						{
							int pageRank = (int) xPage.Attribute ("rank");

							var page = new DeserializedPage (section, pageRank, xPage);

							if (zoom > 0)  // génère une miniature de la page ?
							{
								var port = new XmlPort (xPage);
								page.Miniature = port.Deserialize (id => PrintEngine.GetImage (businessContext, id), new Size (width, height), zoom);
							}

							section.Pages.Add (page);
						}

						job.Sections.Add (section);
					}

					jobs.Add (job);
				}
			}

			return jobs;
		}
	}
}
