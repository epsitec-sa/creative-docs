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

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Printers
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

		public static bool CanPrint(AbstractEntity entity)
		{
			return AbstractEntityPrinter.FindEntityPrinterType (entity) != null;
		}


		public static void Print(CoreData coreData, AbstractEntity entity)
		{
			PrintEngine.Print (coreData, new AbstractEntity[] { entity });
		}

		public static void Print(CoreData coreData, IEnumerable<AbstractEntity> collection, AbstractEntityPrinter entityPrinter = null)
		{
			//	Imprime plusieurs entités sur les unités d'impression correspondantes.
			//	Par exemple:
			//		page 1: Imprimante A bac 1
			//		page 2: Imprimante A bac 1
			//		page 3: Imprimante A bac 2
			//		page 4: Imprimante A bac 1
			//		page 5: Imprimante A bac 1
			//		page 6: Imprimante A bac 2
			//	Sections générées:
			//		page 1: Imprimante A bac 1
			//		page 2: Imprimante A bac 1
			//		page 4: Imprimante A bac 1
			//		page 5: Imprimante A bac 1
			//		page 3: Imprimante A bac 2
			//		page 6: Imprimante A bac 2
			//	Sections imprimées:
			//		page 1..2: Imprimante A bac 1
			//		page 3:    Imprimante A bac 2
			//		page 4..4: Imprimante A bac 1
			//		page 6:    Imprimante A bac 2
			//	Le tri est effectué par CompareSectionToPrint.

			List<AbstractEntity> entities = PrintEngine.PrepareEntities (collection, Operation.Print);

			if (entities.Count == 0)
			{
				return;
			}

			//	Si l'entityPrinter n'existe pas, on le crée pour la première entité, et on affiche le dialogue
			//	pour choisir comment imprimer l'entité (choix du type de document et des options).
			if (entityPrinter == null)
			{
				entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (coreData, entities.FirstOrDefault ());

				var typeDialog = new Dialogs.DocumentTypeDialog (CoreProgram.Application, entityPrinter, entities, isPreview: false);
				typeDialog.OpenDialog ();

				if (typeDialog.Result != DialogResult.Accept)
				{
					return;
				}
			}

			//	Vérifie si un minimun d'unités d'impression sont définies pour imprimer le type de document choisi.
			DocumentTypeDefinition documentType = entityPrinter.SelectedDocumentTypeDefinition;

			if (!documentType.IsDocumentPrintersDefined)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une ou plusieurs unités d'impression n'ont pas été définies.").OpenDialog ();
				return;
			}

			//	Prépare toutes les pages à imprimer, pour toutes les entités.
			//	On crée autant de sections que de pages, soit une section par page.
			List<SectionToPrint> sections = new List<SectionToPrint> ();
			List<PrinterUnit> printerUnitList = PrinterApplicationSettings.GetPrinterUnitList ();

			for (int entityRank = 0; entityRank < entities.Count; entityRank++)
			{
				var entity = entities[entityRank];

				if (entityRank > 0)
				{
					//	S'il ne s'agit pas de la première entité, on crée un nouveau entityPrinter en
					//	reprenant les réglages du précédent.
					EntityPrintingSettings settings = entityPrinter.EntityPrintingSettings;

					entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (coreData, entity);
					entityPrinter.EntityPrintingSettings = settings;
				}

				int documentPrinterRank = 0;
				foreach (var documentPrinter in entityPrinter.DocumentPrinters)
				{
					foreach (var documentPrinterFunction in documentType.DocumentPrinterFunctions)
					{
						PrinterUnit printerUnit = printerUnitList.Where (p => p.LogicalName == documentPrinterFunction.LogicalPrinterName).FirstOrDefault ();

						if (printerUnit != null &&
							Common.InsidePageSize (printerUnit.PhysicalPaperSize, documentPrinter.MinimalPageSize, documentPrinter.MaximalPageSize))
						{
							//	Construit l'ensemble des pages, en tenant compte des options forcées de l'unité d'impression.
							documentPrinter.SetPrinterUnit (printerUnit);
							documentPrinter.BuildSections (printerUnit.ForcingOptionsToClear, printerUnit.ForcingOptionsToSet);

							if (!documentPrinter.IsEmpty (documentPrinterFunction.PrinterFunction))
							{
								for (int copy = 0; copy < printerUnit.Copies; copy++)
								{
									var physicalPages = documentPrinter.GetPhysicalPages (documentPrinterFunction.PrinterFunction);
									foreach (var physicalPage in physicalPages)
									{
										string e = (entityRank+1).ToString ();
										string p = (documentPrinterRank+1).ToString ();
										string d = (documentPrinter.GetDocumentRank (physicalPage)+1).ToString ();
										string c = (copy+1).ToString ();
										string internalJobName = string.Concat (documentPrinterFunction.Job, ".", e, ".", p, ".", d, ".", c);

										sections.Add (new SectionToPrint (printerUnit, internalJobName, physicalPage, entityRank, documentPrinter));
									}
								}
							}
						}
					}

					documentPrinterRank++;
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

				if (p1.InternalJobName                 == p2.InternalJobName                 &&
					p1.PrinterUnit.PhysicalPrinterName == p2.PrinterUnit.PhysicalPrinterName &&
					p1.FirstPage                       == p2.FirstPage                       )
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
					p1.PrinterUnit              == p2.PrinterUnit     &&
					p1.EntityRank               == p2.EntityRank      &&
					p1.FirstPage + p1.PageCount == p2.FirstPage       )
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
					j1.PrinterPhysicalName == j2.PrinterPhysicalName )
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
				job.JobFullName = string.Concat (job.Sections[0].DocumentPrinter.JobName, ".", (index+1).ToString ());
				index++;
			}

			//	Affiche les jobs.
			var jobDialog = new Dialogs.PrintingJobDialog (CoreProgram.Application, jobs);
			jobDialog.OpenDialog ();

			if (jobDialog.Result == DialogResult.Accept)  // imprimer ?
			{
				//	Imprime tous les jobs (pages sur une même imprimante physique, mais éventuellement sur
				//	plusieurs bacs différents).
				foreach (var job in jobs)
				{
					PrintEngine.PrintJob (job);
				}
			}

			if (jobDialog.Result == DialogResult.Answer1)  // enregistrer ?
			{
				PrintEngine.SaveJobs (coreData, jobs);
			}
		}


		public static void Preview(CoreData coreData, AbstractEntity entity)
		{
			PrintEngine.Preview (coreData, new AbstractEntity[] { entity });
		}

		public static void Preview(CoreData coreData, IEnumerable<AbstractEntity> collection)
		{
			List<AbstractEntity> entities = PrintEngine.PrepareEntities (collection, Operation.Preview);

			if (entities.Count == 0)
			{
				return;
			}

			var entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (coreData, entities.FirstOrDefault ());

			var typeDialog = new Dialogs.DocumentTypeDialog (CoreProgram.Application, entityPrinter, entities, isPreview: true);
			typeDialog.OpenDialog ();

			if (typeDialog.Result == DialogResult.Accept)
			{
				var printDialog = new Dialogs.PreviewDialog (CoreProgram.Application, coreData, entityPrinter, entities);
				printDialog.IsModal = false;
				printDialog.OpenDialog ();
			}
		}

	
		private static List<AbstractEntity> PrepareEntities(IEnumerable<AbstractEntity> collection, Operation operation)
		{
			List<AbstractEntity> entities = new List<AbstractEntity> ();

			if (collection != null && collection.Any ())
			{
				entities.AddRange (collection.Where (x => PrintEngine.CanPrint (x)));

				if (entities.Count == 0)
				{
					string message = "";
					
					switch (operation)
					{
						case Operation.Print:	message = "Ce type de donnée ne peut pas être imprimé.";	break;
						case Operation.Preview:	message = "Ce type de donnée ne peut pas être visualisé.";	break;
					}

                    MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				}
			}
			else
			{
				string message = "";
				
				switch (operation)
				{
					case Operation.Print:	message = "Il n'y a rien à imprimer.";	break;
					case Operation.Preview:	message = "Il n'y a rien à visualier.";	break;
				}

				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
			}

			return entities;
		}


		private static void PrintJob(JobToPrint job)
		{
			//	Imprime un job, c'est-à-dire plusieurs pages sur une même imprimante physique, mais éventuellement sur
			//	plusieurs bacs différents.
			List<SectionToPrint> sectionsToPrint = job.Sections.Where (x => x.Enable).ToList ();

			if (sectionsToPrint.Count == 0)  // rien à imprimer ?
			{
				return;
			}

			var firstSection = sectionsToPrint.FirstOrDefault ();
			PrintDocument printDocument = new PrintDocument ();

			printDocument.DocumentName = job.JobFullName;
			printDocument.SelectPrinter (FormattedText.Unescape (firstSection.PrinterUnit.PhysicalPrinterName));
			printDocument.PrinterSettings.Copies = 1;
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);

			var engine = new JobPrintEngine (printDocument, sectionsToPrint);
			printDocument.Print (engine);
		}


		private static void SaveJobs(CoreData coreData, List<JobToPrint> jobs)
		{
#if false
			//	Sérialise et enregistre tous les jobs d'impression.
			string xmlSource = PrintEngine.SerializeJobs (jobs);
			System.IO.File.WriteAllText ("XmlExport-debug.txt", xmlSource);  // TODO: debug !

			var deserializeJobs = Printers.PrintEngine.DeserializeJobs (coreData, xmlSource);

			var dialog = new Dialogs.XmlPreviewerDialog (CoreProgram.Application, coreData, deserializeJobs);
			dialog.IsModal = true;
			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)  // imprimer ?
			{
				PrintEngine.PrintJobs (coreData, deserializeJobs);
			}
#endif
		}

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
					xJob.Add (new XAttribute ("title",        job.JobFullName));
					xJob.Add (new XAttribute ("printer-name", job.PrinterPhysicalName));

					foreach (var section in sectionsToPrint)
					{
						var xSection = new XElement ("section");
						xSection.Add (new XAttribute ("printer-unit",     section.PrinterUnit.LogicalName));
						xSection.Add (new XAttribute ("printer-tray",     section.PrinterUnit.PhysicalPrinterTray));
						xSection.Add (new XAttribute ("printer-x-offset", section.PrinterUnit.XOffset));
						xSection.Add (new XAttribute ("printer-y-offset", section.PrinterUnit.YOffset));
						xSection.Add (new XAttribute ("printer-width",    section.DocumentPrinter.RequiredPageSize.Width));
						xSection.Add (new XAttribute ("printer-height",   section.DocumentPrinter.RequiredPageSize.Height));

						for (int page = section.FirstPage; page < section.FirstPage+section.PageCount; page++)
						{
							var xPage = new XElement ("page");
							xPage.Add (new XAttribute ("rank", page));

							var port = new XmlPort (xPage);
							section.DocumentPrinter.PreviewMode = PreviewMode.Print;
							section.DocumentPrinter.CurrentPage = page;
							section.DocumentPrinter.SetPrinterUnit (section.PrinterUnit);
							section.DocumentPrinter.BuildSections (section.PrinterUnit.ForcingOptionsToClear, section.PrinterUnit.ForcingOptionsToSet);
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


		private static void RegisterFonts()
		{
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.Creus.Core.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}

		private enum Operation
		{
			Print,
			Preview,
		}
	}
}
