//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class ExportStatics<T>
		where T : struct
	{
		public static void ShowExportPopup(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller)
		{
			var popup = new ExportPopup (accessor)
			{
				Inverted = false,
				Filename = LocalSettings.ExportFilename,
				Filters  = ExportStatics<T>.ExportFilters,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportFilename = popup.Filename;

					try
					{
						ExportStatics<T>.Export (dataFiller, popup.Filename, popup.Inverted);
					}
					catch (System.Exception ex)
					{
						MessagePopup.ShowError (target, ex.Message);
						return;
					}

					MessagePopup.ShowMessage (target, "Exportation effectée avec succès.");
				}
			};
		}

		private static void Export(AbstractTreeTableFiller<T> dataFiller, string filename, bool inverted)
		{
			var ext = System.IO.Path.GetExtension (filename);

			switch (ext)
			{
				case ".txt":
					ExportStatics<T>.TxtExport (dataFiller, filename, inverted);
					break;

				case ".csv":
					ExportStatics<T>.CsvExport (dataFiller, filename, inverted);
					break;
			}
		}

		private static void TxtExport(AbstractTreeTableFiller<T> dataFiller, string filename, bool inverted)
		{
			var engine = new TextExport<T> ()
			{
				ExportTextProfile = ExportTextProfile.TxtProfile,
			};

			engine.Export (dataFiller, filename, inverted);
		}

		private static void CsvExport(AbstractTreeTableFiller<T> dataFiller, string filename, bool inverted)
		{
			var engine = new TextExport<T> ()
			{
				ExportTextProfile = ExportTextProfile.CsvProfile,
			};

			engine.Export (dataFiller, filename, inverted);
		}


		private static IEnumerable<FilterItem> ExportFilters
		{
			get
			{
				yield return new FilterItem ("pdf", "Document mis en page", "*.pdf");
				yield return new FilterItem ("txt", "Fichier texte tabulé", "*.txt");
				yield return new FilterItem ("csv", "Fichier texte csv",    "*.csv");
			}
		}

	}
}