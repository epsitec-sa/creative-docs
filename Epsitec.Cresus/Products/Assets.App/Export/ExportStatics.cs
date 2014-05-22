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
			var popup = new ExportInstructionsPopup (accessor)
			{
				ExportInstructions = new ExportInstructions (LocalSettings.ExportFilename, false),
				Filters            = ExportStatics<T>.ExportFilters,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportFilename = popup.ExportInstructions.Filename;

					try
					{
						ExportStatics<T>.Export (dataFiller, popup.ExportInstructions);
					}
					catch (System.Exception ex)
					{
						MessagePopup.ShowError (target, ex.Message);
						return;
					}

					ExportStatics<T>.ShowOpenPopup (target, accessor, popup.ExportInstructions);
				}
			};
		}

		private static void Export(AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var ext = System.IO.Path.GetExtension (instructions.Filename);

			switch (ext)
			{
				case ".txt":
					ExportStatics<T>.TxtExport (dataFiller, instructions);
					break;

				case ".csv":
					ExportStatics<T>.CsvExport (dataFiller, instructions);
					break;
			}
		}

		private static void TxtExport(AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var engine = new TextExport<T> ()
			{
				ExportTextProfile = ExportTextProfile.TxtProfile,
			};

			engine.Export (dataFiller, instructions);
		}

		private static void CsvExport(AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var engine = new TextExport<T> ()
			{
				ExportTextProfile = ExportTextProfile.CsvProfile,
			};

			engine.Export (dataFiller, instructions);
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


		private static void ShowOpenPopup(Widget target, DataAccessor accessor, ExportInstructions instructions)
		{
			var popup = new ExportOpenPopup (accessor)
			{
				OpenLocation = false,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					if (popup.OpenLocation)
					{
						ExportStatics<T>.OpenLocation (instructions);
					}
					else
					{
						ExportStatics<T>.OpenFile (instructions);
					}
				}
			};
		}

		private static void OpenFile(ExportInstructions instructions)
		{
			System.Diagnostics.Process.Start (instructions.Filename);
		}

		private static void OpenLocation(ExportInstructions instructions)
		{
			//	Voir http://stackoverflow.com/questions/9646114/open-file-location
			System.Diagnostics.Process.Start ("explorer.exe", "/select," + instructions.Filename);
		}
	}
}