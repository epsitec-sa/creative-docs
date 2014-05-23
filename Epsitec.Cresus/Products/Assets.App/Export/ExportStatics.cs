//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class ExportStatics<T>
		where T : struct
	{
		public static void ShowExportPopup(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller)
		{
			//	Débute le processus d'exportation en ouvrant le popup pour choisir les instructions,
			//	puis continue le processus initié jusqu'à son terme.
			var popup = new ExportInstructionsPopup (accessor)
			{
				ExportInstructions = new ExportInstructions (LocalSettings.ExportFilename, hasHeader: true, inverted: false),
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
						MessagePopup.ShowMessage (target, "Exportation impossible", ex.Message);
						return;
					}

					ExportStatics<T>.ShowOpenPopup (target, accessor, popup.ExportInstructions);
				}
			};
		}

		private static void Export(AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			//	Effectue l'exportation sans aucune interaction.
			var ext = System.IO.Path.GetExtension (instructions.Filename);  // extrait l'extension avec le point

			switch (ext)
			{
				case ".txt":
					ExportStatics<T>.ExportTxt (dataFiller, instructions);
					break;

				case ".csv":
					ExportStatics<T>.ExportCsv (dataFiller, instructions);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("L'extension \"{0}\" n'est pas supportée.", ext));
			}
		}

		private static void ExportTxt(AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var engine = new TextExport<T> ()
			{
				ExportTextProfile = ExportTextProfile.TxtProfile,
			};

			engine.Export (dataFiller, instructions);
		}

		private static void ExportCsv(AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var engine = new TextExport<T> ()
			{
				ExportTextProfile = ExportTextProfile.CsvProfile,
			};

			engine.Export (dataFiller, instructions);
		}


		private static IEnumerable<FilterItem> ExportFilters
		{
			//	Retourne la liste des formats supportés, pour le dialogue OpenFile standard.
			get
			{
				yield return new FilterItem ("pdf", "Document mis en page", "*.pdf");
				yield return new FilterItem ("txt", "Fichier texte tabulé", "*.txt");
				yield return new FilterItem ("csv", "Fichier texte csv",    "*.csv");
			}
		}


		private static void ShowOpenPopup(Widget target, DataAccessor accessor, ExportInstructions instructions)
		{
			//	Affiche le popup permettant d'ouvrir le fichier ou l'emplacement.
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
			//	Ouvre le fichier, en lançant l'application par défaut selon l'extension.
			System.Diagnostics.Process.Start (instructions.Filename);
		}

		private static void OpenLocation(ExportInstructions instructions)
		{
			//	Ouvre l'explorateur de fichier et sélectionne le fichier exporté.
			//	Voir http://stackoverflow.com/questions/9646114/open-file-location
			System.Diagnostics.Process.Start ("explorer.exe", "/select," + instructions.Filename);
		}
	}
}