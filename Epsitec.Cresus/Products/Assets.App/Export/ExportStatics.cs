//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
				ExportInstructions = LocalSettings.ExportInstructions,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportInstructions = popup.ExportInstructions;
					ExportStatics<T>.Export (target, accessor, dataFiller, popup.ExportInstructions);
				}
			};
		}

		private static void Export(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			//	Effectue l'exportation sans aucune interaction.
			switch (instructions.Format)
			{
				case ExportFormat.Text:
					ExportStatics<T>.ShowTxtPopup (target, accessor, dataFiller, instructions);
					break;

				case ExportFormat.Csv:
					ExportStatics<T>.ShowCsvPopup (target, accessor, dataFiller, instructions);
					break;

				case ExportFormat.Html:
					ExportStatics<T>.ShowHtmlPopup (target, accessor, dataFiller, instructions);
					break;

				default:
					var ext = ExportInstructionsPopup.GetFormatExt (instructions.Format);
					var message = string.Format ("L'extension \"{0}\" n'est pas supportée.", ext);
					MessagePopup.ShowMessage (target, "Exportation impossible", message);
					break;
			}
		}


		private static void ShowTxtPopup(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var popup = new ExportTextPopup (accessor)
			{
				Profile = LocalSettings.ExportTxtProfile,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportTxtProfile = popup.Profile;

					try
					{
						ExportStatics<T>.ExportText (target, dataFiller, instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						MessagePopup.ShowMessage (target, "Exportation impossible", ex.Message);
						return;
					}

					ExportStatics<T>.ShowOpenPopup (target, accessor, instructions);
				}
			};
		}

		private static void ShowCsvPopup(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var popup = new ExportTextPopup (accessor)
			{
				Profile = LocalSettings.ExportCsvProfile,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportCsvProfile = popup.Profile;

					try
					{
						ExportStatics<T>.ExportText (target, dataFiller, instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						MessagePopup.ShowMessage (target, "Exportation impossible", ex.Message);
						return;
					}

					ExportStatics<T>.ShowOpenPopup (target, accessor, instructions);
				}
			};
		}

		private static void ShowHtmlPopup(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions)
		{
			var popup = new ExportHtmlPopup (accessor)
			{
				Profile = LocalSettings.ExportHtmlProfile,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportHtmlProfile = popup.Profile;

					try
					{
						ExportStatics<T>.ExportHtml (target, dataFiller, instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						MessagePopup.ShowMessage (target, "Exportation impossible", ex.Message);
						return;
					}

					ExportStatics<T>.ShowOpenPopup (target, accessor, instructions);
				}
			};
		}


		private static void ExportText(Widget target, AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions, TextExportProfile profile)
		{
			var engine = new TextExport<T> ()
			{
				Instructions = instructions,
				Profile      = profile,
			};

			engine.Export (dataFiller);
		}

		private static void ExportHtml(Widget target, AbstractTreeTableFiller<T> dataFiller, ExportInstructions instructions, HtmlExportProfile profile)
		{
			var engine = new HtmlExport<T> ()
			{
				Instructions = instructions,
				Profile      = profile,
			};

			engine.Export (dataFiller);
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