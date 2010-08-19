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

			List<Printer> printers = PrintEngine.GetInstalledPrinters ();
			
			if (printers.Count == 0)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Aucune imprimante n'est configurée pour cet ordinateur.").OpenDialog ();
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
			
			var printDialog = new Dialogs.PrintDialog (CoreProgram.Application, entityPrinter, entities, printers);
			printDialog.OpenDialog ();
		}

		private static List<Printer> GetInstalledPrinters()
		{
			List<Printer> printers = new List<Printer>
						(
							from printer in Printer.Load ()
							where PrinterSettings.InstalledPrinters.Contains (printer.Name)
							select printer
						);
			
			return printers;
		}

		private static List<AbstractEntity> PrepareEntities(IEnumerable<AbstractEntity> collection, Operation operation)
		{
			List<AbstractEntity> entities = new List<AbstractEntity> ();

			if ((collection != null) &&
				(collection.Any ()))
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
		
		private enum Operation
		{
			Print,
			Preview,
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
	}
}
