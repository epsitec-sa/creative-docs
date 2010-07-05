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

namespace Epsitec.Cresus.Core
{
	public sealed class PrintEngine
	{
		public PrintEngine()
		{
		}


		public void Print(AbstractEntity entity)
		{
			var entities = new List<AbstractEntity> ();
			entities.Add (entity);

			this.Print (entities);
		}

		public void Print(IEnumerable<AbstractEntity> entities)
		{
			List<Printer> printers = new List<Printer>
			(
				from printer in Printer.Load ()
				where PrinterSettings.InstalledPrinters.Contains (printer.Name)
				select printer
			);

			bool checkPrintersList = (printers.Count > 0);

			if (!checkPrintersList)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Aucune imprimante n'est configurée pour cet ordinateur.").OpenDialog ();
			}
			else
			{
				var printDialog = new Dialogs.PrintDialog (CoreProgram.Application, entities, printers);
				printDialog.OpenDialog ();
			}
		}
	}
}
