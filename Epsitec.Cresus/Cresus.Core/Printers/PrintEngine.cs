//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer;
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

			//	Vérifie si un minimun d'imprimantes sont définies pour imprimer l'entité.
			DocumentType documentType = entityPrinter.DocumentTypeSelected;

			if (!documentType.IsDocumentPrintersDefined)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une ou plusieurs imprimantes n'ont pas été définies.").OpenDialog ();
				return;
			}

			//	Prépare toutes les pages à imprimer, pour toutes les entités.
			//	On crée autant de sections que de pages, soit une section par page.
			List<SectionToPrint> sections = new List<SectionToPrint> ();
			List<Printer> printerList = PrinterSettings.GetPrinterList ();

			for (int i=0; i<entities.Count; i++)
			{
				var entity = entities[i];

				if (i > 0)
				{
					//	S'il ne s'agit pas de la première entité, on crée un nouveau entityPrinter en
					//	reprenant les réglages du précédent.
					EntityPrintingSettings settings = entityPrinter.EntityPrintingSettings;

					entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (entity);
					entityPrinter.EntityPrintingSettings = settings;
				}

				//	Construit l'ensemble des pages.
				entityPrinter.BuildSections ();

				foreach (DocumentPrinter documentPrinter in documentType.DocumentPrinters)
				{
					Printer printer = printerList.Where (p => p.LogicalName == documentPrinter.LogicalPrinterName).FirstOrDefault ();

					if (printer != null)
					{
						if (!entityPrinter.IsEmpty (documentPrinter.PrinterFunction))
						{
							var physicalPages = entityPrinter.GetPhysicalPages (documentPrinter.PrinterFunction);

							foreach (var physicalPage in physicalPages)
							{
								sections.Add (new SectionToPrint (printer, physicalPage, i, entityPrinter));
							}
						}
					}
				}
			}

			//	Trie toutes les pages, qui sont regroupées logiquement par imprimante physique.
			sections.Sort (PrintEngine.CompareSectionToPrint);

			//	Fusionne toutes les pages contigües qui utilisent la même imprimante en sections de plusieurs pages.
			int index = 0;
			while (index < sections.Count-1)
			{
				SectionToPrint p1 = sections[index];
				SectionToPrint p2 = sections[index+1];

				if (p1.Printer == p2.Printer &&
					p1.EntityRank == p2.EntityRank &&
					p1.FirstPage+p1.PageCount == p2.FirstPage)
				{
					p1.PageCount += p2.PageCount;  // ajoute les pages de p2 à p1
					sections.RemoveAt (index+1);  // supprime p2
				}
				else
				{
					index++;
				}
			}

			//	Imprime toutes les sections.
			foreach (var page in sections)
			{
				PrintEngine.PrintEntity (page.Printer, page.EntityPrinter, page.FirstPage, page.PageCount);
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


		private static void PrintEntity(Printer printer, AbstractEntityPrinter entityPrinter, int firstPage, int pageCount)
		{
			var printerSettings = Epsitec.Common.Printing.PrinterSettings.FindPrinter (printer.PhysicalName);

			bool checkTray = printerSettings.PaperSources.Any (tray => (tray.Name == printer.Tray));

			if (checkTray)
			{
				try
				{
					PrintEngine.PrintEntityBase (printer, entityPrinter, firstPage, pageCount);
				}
				catch (System.Exception e)
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une erreur s'est produite lors de l'impression").OpenDialog ();
				}
			}
			else
			{
				string message = string.Format ("Le bac ({0}) de l'imprimante séléctionnée ({1}) n'existe pas.", printer.Tray, printer.PhysicalName);
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
			}
		}

		private static void PrintEntityBase(Printer printer, Printers.AbstractEntityPrinter entityPrinter, int firstPage, int pageCount)
		{
			PrintDocument printDocument = new PrintDocument ();

			printDocument.DocumentName = entityPrinter.JobName;
			printDocument.SelectPrinter (printer.PhysicalName);
			printDocument.PrinterSettings.Copies = 1;
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);
			printDocument.DefaultPageSettings.PaperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, paperSource => paperSource.Name == printer.Tray);

			Size size = entityPrinter.PageSize;
			double height = size.Height;
			double width  = size.Width;

			double xOffset = printer.XOffset;
			double yOffset = printer.YOffset;

			Transform transform;

			if (entityPrinter.PageSize.Width < entityPrinter.PageSize.Height)  // portrait ?
			{
				transform = Transform.Identity;
			}
			else  // paysage ?
			{
				transform = Transform.CreateRotationDegTransform (90, entityPrinter.PageSize.Height/2, entityPrinter.PageSize.Height/2);
			}

			var engine = new MultiPagePrintEngine (entityPrinter, transform, firstPage, pageCount);
			printDocument.Print (engine);
		}


		#region Section to print
		private class SectionToPrint
		{
			public SectionToPrint(Printer printer, int firstPage, int entityRank, AbstractEntityPrinter entityPrinter)
			{
				//	Crée une section d'une page.
				this.printer       = printer;
				this.firstPage     = firstPage;
				this.PageCount     = 1;
				this.entityRank    = entityRank;
				this.entityPrinter = entityPrinter;
			}

			public Printer Printer
			{
				get
				{
					return this.printer;
				}
			}

			public int FirstPage
			{
				get
				{
					return this.firstPage;
				}
			}

			public int PageCount
			{
				get;
				set;
			}

			public int EntityRank
			{
				get
				{
					return this.entityRank;
				}
			}

			public AbstractEntityPrinter EntityPrinter
			{
				get
				{
					return this.entityPrinter;
				}
			}

			private readonly Printer				printer;
			private readonly int					firstPage;
			private readonly int					entityRank;
			private readonly AbstractEntityPrinter	entityPrinter;
		}

		private static int CompareSectionToPrint(SectionToPrint x, SectionToPrint y)
		{
			//	Détermine comment regrouper les pages. On cherche à grouper les pages
			//	qui utilisent une même imprimante physique.
			int result;

			result = string.Compare (x.Printer.PhysicalName, y.Printer.PhysicalName);
			if (result != 0)
			{
				return result;
			}

			if (x.EntityRank != y.EntityRank)
			{
				return (x.EntityRank < y.EntityRank) ? -1 : 1;
			}

			if (x.FirstPage != y.FirstPage)
			{
				return (x.FirstPage < y.FirstPage) ? -1 : 1;
			}

			return 0;
		}
		#endregion


		private enum Operation
		{
			Print,
			Preview,
		}
	}
}
