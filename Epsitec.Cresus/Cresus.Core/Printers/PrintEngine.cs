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
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Printers
{
	public sealed class PrintEngine
	{
		public PrintEngine()
		{
		}


		public void Print(AbstractEntity entity)
		{
			var entities = new List<AbstractEntity> ();

			if (entity != null)
			{
				entities.Add (entity);
			}

			this.Print (entities);
		}

		public void Print(IEnumerable<AbstractEntity> entities)
		{
			if (entities == null || entities.Count () == 0)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Il n'y a rien à imprimer.").OpenDialog ();
				return;
			}

			List<Printer> printers = new List<Printer>
			(
				from printer in Printer.Load ()
				where PrinterSettings.InstalledPrinters.Contains (printer.Name)
				select printer
			);

			if (printers.Count == 0)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Aucune imprimante n'est configurée pour cet ordinateur.").OpenDialog ();
				return;
			}

			//?var entityPrinter = new Printers.AbstractEntityPrinter<T> (entities.First ());
			var entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (entities.First ());

			if (entityPrinter == null)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Ce type de donnée ne peut pas être imprimé.").OpenDialog ();
				return;
			}

			var printDialog = new Dialogs.PrintDialog (CoreProgram.Application, entityPrinter, entities, printers);
			printDialog.OpenDialog ();
		}
	}
}
