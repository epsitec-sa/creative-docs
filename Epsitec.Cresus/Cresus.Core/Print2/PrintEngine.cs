//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print2.Verbose;
using Epsitec.Cresus.Core.Print2.Deserializers;

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print2
{
	public static class PrintEngine
	{
		static PrintEngine()
		{
			PrintEngine.RegisterFonts ();
		}

		public static void Setup()
		{
			//	Force execution of static constructor.
		}


		#region Command handlers
		public static void PrintCommand(CoreData coreData, AbstractEntity entity)
		{
			//	La commande 'Print' du ruban a été activée.
			if (entity == null)
			{
				var message = "Il n'y a aucune donnée à imprimer.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var xml = PrintEngine.Print (coreData, entity);

			if (string.IsNullOrEmpty (xml))
			{
				var message = "Ce type de donnée ne peut pas être imprimé.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var deserializeJobs = PrintEngine.DeserializeJobs (coreData, xml);
			PrintEngine.PrintJobs (coreData, deserializeJobs);
		}

		public static void PreviewCommand(CoreData coreData, AbstractEntity entity)
		{
			//	La commande 'Preview' du ruban a été activée.
			if (entity == null)
			{
				var message = "Il n'y a aucune donnée à visualiser.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var xml = PrintEngine.Print (coreData, entity);

			if (string.IsNullOrEmpty (xml))
			{
				var message = "Ce type de donnée ne peut pas être visualisé.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var deserializeJobs = PrintEngine.DeserializeJobs (coreData, xml);

			var dialog = new Dialogs.XmlPreviewerDialog (CoreProgram.Application, coreData, deserializeJobs);
			dialog.IsModal = true;
			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)  // imprimer ?
			{
				PrintEngine.PrintJobs (coreData, deserializeJobs);
			}
		}

		private static void PrintJobs(CoreData coreData, List<DeserializedJob> jobs)
		{
			foreach (var job in jobs)
			{
				PrintDocument printDocument = new PrintDocument ();

				printDocument.DocumentName = job.JobFullName;
				printDocument.SelectPrinter (FormattedText.Unescape (job.PrinterPhysicalName));
				printDocument.PrinterSettings.Copies = 1;
				printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);

				var engine = new XmlJobPrintEngine (coreData, printDocument, job.Sections);
				printDocument.Print (engine);
			}
		}
		#endregion


		public static string Print(CoreData coreData, AbstractEntity entity)
		{
			//	Imprime un document et retourne le source xml correspondant, sans aucune interaction.
			//	Si l'entité est de type DocumentMetadataEntity, on utilise les options et les unités
			//	d'impression définies dans l'entité DocumentCategory.
			var options       = PrintEngine.GetOptions (entity);
			var printingUnits = PrintEngine.GetPrintingUnits (entity);

			return PrintEngine.Print (coreData, entity, options, printingUnits);
		}

		public static string Print(CoreData coreData, AbstractEntity entity, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			//	Imprime un document et retourne le source xml correspondant, sans aucune interaction.
			var entities = new List<AbstractEntity> ();
			entities.Add (entity);

			return PrintEngine.Print (coreData, entities, options, printingUnits);
		}

		public static string Print(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			//	Imprime un document et retourne le source xml correspondant, sans aucune interaction.
			System.Diagnostics.Debug.Assert (coreData != null);
			System.Diagnostics.Debug.Assert (entities != null);
			System.Diagnostics.Debug.Assert (options != null);
			System.Diagnostics.Debug.Assert (printingUnits != null);

			if (entities.Count () == 0)
			{
				return null;
			}

			//	Crée les classes qui savent imprimer l'entité qui représente le document (EntityPrinters.AbstrctPrinter).
			var documentPrinters = EntityPrinters.AbstractPrinter.CreateDocumentPrinters (coreData, entities, options, printingUnits);

			if (documentPrinters.Count () == 0 || printingUnits.Count == 0)
			{
				return null;
			}

			//	Prépare toutes les pages à imprimer, pour toutes les entités.
			//	On crée autant de sections que de pages, soit une section par page.
			var sections = new List<SectionToPrint> ();
			var printingUnitList = PrinterApplicationSettings.GetPrintingUnitList ();
			var pageTypes = VerbosePageType.GetAll ();

			for (int entityRank = 0; entityRank < entities.Count (); entityRank++)
			{
				var entity = entities.ElementAt (entityRank);

				int documentPrinterRank = 0;
				foreach (var documentPrinter in documentPrinters)
				{
					foreach (var pair in printingUnits.ContentPair)
					{
						var pageType         = pair.Key;
						var printingUnitName = pair.Value;

						PrintingUnit printingUnit = printingUnitList.Where (p => p.LogicalName == printingUnitName).FirstOrDefault ();

						if (printingUnit != null &&
							Common.InsidePageSize (printingUnit.PhysicalPaperSize, documentPrinter.MinimalPageSize, documentPrinter.MaximalPageSize))
						{
							documentPrinter.SetPrintingUnit (printingUnit);
							documentPrinter.PreviewMode = PreviewMode.Print;
							documentPrinter.BuildSections ();

							if (!documentPrinter.IsEmpty (pageType))
							{
								var jobName = pageTypes.Where (x => x.Type == pageType).Select (x => x.Job).FirstOrDefault ();

								for (int copy = 0; copy < printingUnit.Copies; copy++)
								{
									var physicalPages = documentPrinter.GetPhysicalPages (pageType);
									foreach (var physicalPage in physicalPages)
									{
										string e = (entityRank+1).ToString ();
										string p = (documentPrinterRank+1).ToString ();
										string d = (documentPrinter.GetDocumentRank (physicalPage)+1).ToString ();
										string c = (copy+1).ToString ();
										string internalJobName = string.Concat (jobName, ".", e, ".", p, ".", d, ".", c);

										sections.Add (new SectionToPrint (printingUnit, internalJobName, physicalPage, entityRank, documentPrinter));
									}
								}
							}

							documentPrinterRank++;
						}
					}
				}
			}

			//	Trie toutes les pages, qui sont regroupées logiquement par jobs et par imprimantes physiques.
			sections.Sort (SectionToPrint.CompareSectionToPrint);

			//	Sépare les copies multiples de pages identiques sur une même imprimante.
			//	Par exemple: 0,0,0,1,1,1,2,2,2 devient 0,1,2,0,1,2,0,1,2
			int index = 0;
			while (index < sections.Count-1)
			{
				SectionToPrint p1 = sections[index];
				SectionToPrint p2 = sections[index+1];

				if (p1.InternalJobName                  == p2.InternalJobName                  &&
					p1.PrintingUnit.PhysicalPrinterName == p2.PrintingUnit.PhysicalPrinterName &&
					p1.FirstPage                        == p2.FirstPage)
				{
					sections.RemoveAt (index+1);  // supprime p2...
					sections.Add (p2);            // ...puis remet-la à la fin
				}
				else
				{
					index++;
				}
			}

			//	Fusionne toutes les pages contigües qui font partie du même job en sections de plusieurs pages.
			//	Par exemple: 0,1,2,0,1,2,0,1,2 devient 0..2, 0..2, 0..2
			index = 0;
			while (index < sections.Count-1)
			{
				SectionToPrint p1 = sections[index];
				SectionToPrint p2 = sections[index+1];

				if (p1.InternalJobName          == p2.InternalJobName &&
					p1.PrintingUnit             == p2.PrintingUnit    &&
					p1.EntityRank               == p2.EntityRank      &&
					p1.FirstPage + p1.PageCount == p2.FirstPage)
				{
					p1.PageCount += p2.PageCount;  // ajoute les pages de p2 à p1
					sections.RemoveAt (index+1);   // supprime p2
				}
				else
				{
					index++;
				}
			}

			//	Fusionne toutes les sections en jobs. Un job est un ensemble de sections utilisant la
			//	même imprimante physique, mais pas forcément le même bac.
			var jobs = new List<JobToPrint> ();

			foreach (var page in sections)
			{
				var job = new JobToPrint ();
				job.Sections.Add (page);

				jobs.Add (job);
			}

			index = 0;
			while (index < jobs.Count-1)
			{
				JobToPrint j1 = jobs[index];
				JobToPrint j2 = jobs[index+1];

				if (j1.InternalJobName     == j2.InternalJobName     &&
					j1.PrinterPhysicalName == j2.PrinterPhysicalName)
				{
					j1.Sections.AddRange (j2.Sections);  // ajoute les sections de j2 à j1
					jobs.RemoveAt (index+1);             // supprime j2
				}
				else
				{
					index++;
				}
			}

			//	Nomme tous les jobs.
			index = 0;
			foreach (var job in jobs)
			{
				job.JobFullName = string.Concat (job.Sections[0].EntityPrinter.JobName, ".", (index+1).ToString ());
				index++;
			}

			return PrintEngine.SerializeJobs (jobs);
		}


		#region Dictionary getters
		private static OptionsDictionary GetOptions(AbstractEntity entity)
		{
			//	Retourne les options à utiliser pour l'entité.
			var result = new OptionsDictionary ();

			if (entity is DocumentMetadataEntity)
			{
				var metadata = entity as DocumentMetadataEntity;

				foreach (var documentOptions in metadata.DocumentCategory.DocumentOptions)
				{
					var option = documentOptions.GetOptions ();
					result.Merge (option);
				}
			}

#if true
			if (result.Count == 0)  // TODO: Hack à supprimer dès que possible !
			{
				result = OptionsDictionary.GetDefault ();
				result.Add (DocumentOption.EsrPosition, "WithInside");  // force un BV
			}
#endif

			return result;
		}

		private static PrintingUnitsDictionary GetPrintingUnits(AbstractEntity entity)
		{
			//	Retourne les unités d'impression à utiliser pour l'entité.
			var result = new PrintingUnitsDictionary ();

			if (entity is DocumentMetadataEntity)
			{
				var metadata = entity as DocumentMetadataEntity;

				foreach (var documentOptions in metadata.DocumentCategory.DocumentPrintingUnits)
				{
					var printingUnit = documentOptions.GetPrintingUnits ();
					result.Merge (printingUnit);
				}
			}

#if true
			if (result.Count == 0)  // TODO: Hack à supprimer dès que possible !
			{
				result.Add (PageType.All, "Blanc");
				//?result.Add (PageType.Esr, "Jaune");
				//?result.Add (PageType.Copy, "Brouillon");
				result.Add (PageType.Label, "Etiquettes");
			}
#endif

			return result;
		}
		#endregion


		#region Serializer
		private static string SerializeJobs(List<JobToPrint> jobs)
		{
			//	Retourne la chaîne xmlSource qui sérialise une liste de jobs d'impression.
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
						xSection.Add (new XAttribute ("printer-width",    section.EntityPrinter.RequiredPageSize.Width));
						xSection.Add (new XAttribute ("printer-height",   section.EntityPrinter.RequiredPageSize.Height));

						for (int page = section.FirstPage; page < section.FirstPage+section.PageCount; page++)
						{
							var xPage = new XElement ("page");
							xPage.Add (new XAttribute ("rank", page));

							var port = new XmlPort (xPage);
							section.EntityPrinter.PreviewMode = PreviewMode.Print;
							section.EntityPrinter.CurrentPage = page;
							section.EntityPrinter.SetPrintingUnit (section.PrintingUnit);
							section.EntityPrinter.BuildSections ();
							section.EntityPrinter.PrintBackgroundCurrentPage (port);
							section.EntityPrinter.PrintForegroundCurrentPage (port);

							xSection.Add (xPage);
						}

						xJob.Add (xSection);
					}

					xRoot.Add (xJob);
				}
			}

			return xDocument.ToString (SaveOptions.None);
		}

		private static List<DeserializedJob> DeserializeJobs(CoreData coreData, string xmlSource, double zoom=0)
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
								page.Miniature = port.Deserialize (id => PrintEngine.GetImage (coreData, id), new Size (width, height), zoom);
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

		public static Image GetImage(CoreData coreData, string id)
		{
			//	Retrouve l'image dans la base de données, à partir de son identificateur (ImageBlobEntity.Code).
			var store = coreData.ImageDataStore;
			var data = store.GetImageData (id);
			return data.GetImage ();
		}
		#endregion


		private static void RegisterFonts()
		{
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.Creus.Core.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}
	}
}
