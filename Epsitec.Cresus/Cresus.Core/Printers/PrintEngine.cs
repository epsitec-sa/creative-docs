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
			List<AbstractEntity> entities = PrintEngine.PrepareEntities (collection, Operation.Print);

			if (entities.Count == 0)
			{
				return;
			}

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

			DocumentType documentType = entityPrinter.DocumentTypeSelected;

			foreach (PrinterToUse printerToUse in documentType.PrintersToUse)
			{
				if (string.IsNullOrWhiteSpace (printerToUse.LogicalPrinterName))
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une ou plusieurs imprimantes n'ont pas été choisies.").OpenDialog ();
					return;
				}
			}

			List<Printer> printerList = Dialogs.PrinterListDialog.GetPrinterSettings ();

			foreach (PrinterToUse printerToUse in documentType.PrintersToUse)
			{
				Printer printer = printerList.Where (p => p.LogicalName == printerToUse.LogicalPrinterName).FirstOrDefault ();

				if (printer != null)
				{
					PrintEngine.PrintEntities (printer, printerToUse.Code, entityPrinter, entities);
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


		private static void PrintEntities(Printer printer, string printerCode, AbstractEntityPrinter entityPrinter, List<AbstractEntity> entities)
		{
			PrinterSettings printerSettings = PrinterSettings.FindPrinter (printer.PhysicalName);

			bool checkTray = printerSettings.PaperSources.Any (tray => (tray.Name == printer.Tray));

			if (checkTray)
			{
				try
				{
					foreach (var entity in entities)
					{
						PrintEngine.PrintEntity (printer, printerCode, entityPrinter, entity);
					}
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

		private static void PrintEntity(Printer printer, string printerCode, Printers.AbstractEntityPrinter entityPrinter, AbstractEntity entity)
		{
			PrintDocument printDocument = new PrintDocument ();

			printDocument.DocumentName = entityPrinter.JobName;
			printDocument.SelectPrinter (printer.PhysicalName);
			//?printDocument.PrinterSettings.Copies = int.Parse (FormattedText.Unescape (this.nbCopiesTextField.Text), CultureInfo.InvariantCulture);
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);
			printDocument.DefaultPageSettings.PaperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, paperSource => paperSource.Name == printer.Tray);

			Size size = entityPrinter.PageSize;
			double height = size.Height;
			double width  = size.Width;

			double xOffset = printer.XOffset;
			double yOffset = printer.YOffset;

			entityPrinter.BuildSections (printerCode);

			Transform transform;

			if (entityPrinter.PageSize.Width < entityPrinter.PageSize.Height)  // portrait ?
			{
				transform = Transform.Identity;
			}
			else  // paysage ?
			{
				transform = Transform.CreateRotationDegTransform (90, entityPrinter.PageSize.Height/2, entityPrinter.PageSize.Height/2);
			}

			var engine = new MultiPagePrintEngine (entityPrinter, transform);
			printDocument.Print (engine);
		}


		private enum Operation
		{
			Print,
			Preview,
		}
	}
}
