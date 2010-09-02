//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Printers
{
	public static class PrintEngine
	{
		public static bool CanPrint(AbstractEntity entity)
		{
			return AbstractEntityPrinter.FindEntityPrinterType (entity) != null;
		}


		public static void Print(AbstractEntity entity)
		{
			PrintEngine.Print (new AbstractEntity[] { entity });
		}

		public static void Print(IEnumerable<AbstractEntity> collection, AbstractEntityPrinter entityPrinter = null)
		{
			//	Imprime plusieurs entités sur les imprimantes/bacs correspondants.
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
				entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (entities.FirstOrDefault ());

				var typeDialog = new Dialogs.DocumentTypeDialog (CoreProgram.Application, entityPrinter, entities, isPreview: false);
				typeDialog.OpenDialog ();

				if (typeDialog.Result != DialogResult.Accept)
				{
					return;
				}
			}

			//	Vérifie si un minimun d'imprimantes sont définies pour imprimer le type de document choisi.
			DocumentTypeDefinition documentType = entityPrinter.DocumentTypeSelected;

			if (!documentType.IsDocumentPrintersDefined)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une ou plusieurs imprimantes n'ont pas été définies.").OpenDialog ();
				return;
			}

			//	Prépare toutes les pages à imprimer, pour toutes les entités.
			//	On crée autant de sections que de pages, soit une section par page.
			List<SectionToPrint> sections = new List<SectionToPrint> ();
			List<Printer> printerList = PrinterSettings.GetPrinterList ();

			for (int entityRank = 0; entityRank < entities.Count; entityRank++)
			{
				var entity = entities[entityRank];

				if (entityRank > 0)
				{
					//	S'il ne s'agit pas de la première entité, on crée un nouveau entityPrinter en
					//	reprenant les réglages du précédent.
					EntityPrintingSettings settings = entityPrinter.EntityPrintingSettings;

					entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (entity);
					entityPrinter.EntityPrintingSettings = settings;
				}

				//	Construit l'ensemble des pages.
				entityPrinter.BuildSections ();

				foreach (var documentPrinter in documentType.DocumentPrinters)
				{
					Printer printer = printerList.Where (p => p.LogicalName == documentPrinter.LogicalPrinterName).FirstOrDefault ();

					if (printer != null)
					{
						if (!entityPrinter.IsEmpty (documentPrinter.PrinterFunction))
						{
							for (int copy = 0; copy < printer.Copies; copy++)
							{
								string jobName = string.Concat (documentPrinter.Job, ".", (entityRank+1).ToString (), ".", (copy+1).ToString ());

								var physicalPages = entityPrinter.GetPhysicalPages (documentPrinter.PrinterFunction);
								foreach (var physicalPage in physicalPages)
								{
									sections.Add (new SectionToPrint (printer, jobName, physicalPage, entityRank, entityPrinter));
								}
							}
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

				if (p1.Job                         == p2.Job                         &&
					p1.Printer.PhysicalPrinterName == p2.Printer.PhysicalPrinterName &&
					p1.FirstPage                   == p2.FirstPage                   )
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

				if (p1.Job                      == p2.Job        &&
					p1.Printer                  == p2.Printer    &&
					p1.EntityRank               == p2.EntityRank &&
					p1.FirstPage + p1.PageCount == p2.FirstPage  )
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
			//	même imprimante, mais pas forcément le même bac.
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

				if (j1.Job                 == j2.Job                 &&
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

			//	Affiche les jobs.
			var jobDialog = new Dialogs.PrintingJobDialog (CoreProgram.Application, jobs);
			jobDialog.OpenDialog ();

			if (jobDialog.Result == DialogResult.Accept)
			{
				//	Imprime tous les jobs (pages sur une même imprimante physique, mais éventuellement sur
				//	plusieurs bacs différents).
				foreach (var job in jobs)
				{
					PrintEngine.PrintJob (job);
				}
			}
		}


		public static void Preview(AbstractEntity entity)
		{
			PrintEngine.Preview (new AbstractEntity[] { entity });
		}

		public static void Preview(IEnumerable<AbstractEntity> collection)
		{
			List<AbstractEntity> entities = PrintEngine.PrepareEntities (collection, Operation.Preview);

			if (entities.Count == 0)
			{
				return;
			}

			var entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (entities.FirstOrDefault ());

			var typeDialog = new Dialogs.DocumentTypeDialog (CoreProgram.Application, entityPrinter, entities, isPreview: true);
			typeDialog.OpenDialog ();

			if (typeDialog.Result == DialogResult.Accept)
			{
				var printDialog = new Dialogs.PreviewDialog (CoreProgram.Application, entityPrinter, entities);
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
			List<SectionToPrint> sections = new List<SectionToPrint> ();

			foreach (var s in job.Sections)
			{
				if (s.PrintThisSection)
				{
					sections.Add (s);
				}
			}

			if (sections.Count == 0)
			{
				return;
			}

			var firstSection = sections.FirstOrDefault ();
			if (firstSection == null)
			{
				return;
			}

			PrintDocument printDocument = new PrintDocument ();

			printDocument.DocumentName = firstSection.EntityPrinter.JobName;
			printDocument.SelectPrinter (FormattedText.Unescape (firstSection.Printer.PhysicalPrinterName));
			printDocument.PrinterSettings.Copies = 1;
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);

			var engine = new JobPrintEngine (printDocument, sections);
			printDocument.Print (engine);
		}



		private enum Operation
		{
			Print,
			Preview,
		}
	}
}
