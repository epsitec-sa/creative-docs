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
	public class ExportEngine<T>
		where T : struct
	{
		public ExportEngine(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller)
		{
			this.target     = target;
			this.accessor   = accessor;
			this.dataFiller = dataFiller;
		}


		public void StartExportProcess()
		{
			//	Débute le processus d'exportation qui ouvrira plusieurs popups.
			//	(1) ExportInstructionsPopup pour choisir le format et le fichier.
			//	(2) ExportXxxPopup pour choisir le profile (selon le format).
			//	(3) ExportOpenPopup pour ouvrir le fichier exporté ou l'emplacement.

			var popup = new ExportInstructionsPopup (this.accessor)
			{
				ExportInstructions = LocalSettings.ExportInstructions,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportInstructions = popup.ExportInstructions;  // enregistre dans les réglages
					this.Export (popup.ExportInstructions);
				}
			};
		}


		private void Export(ExportInstructions instructions)
		{
			//	Effectue l'exportation selon les instructions (format et filename).
			switch (instructions.Format)
			{
				case ExportFormat.Text:
					this.ShowTxtPopup (instructions);
					break;

				case ExportFormat.Csv:
					this.ShowCsvPopup (instructions);
					break;

				case ExportFormat.Html:
					this.ShowHtmlPopup (instructions);
					break;

				default:
					var ext = ExportInstructionsPopup.GetFormatExt (instructions.Format);
					var message = string.Format ("L'extension \"{0}\" n'est pas supportée.", ext);
					this.ShowErrorPopup (message);
					break;
			}
		}


		private void ShowTxtPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportTextPopup (this.accessor)
			{
				Profile = LocalSettings.ExportTxtProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportTxtProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportText (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						this.ShowErrorPopup (ex.Message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}

		private void ShowCsvPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportTextPopup (this.accessor)
			{
				Profile = LocalSettings.ExportCsvProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportCsvProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportText (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						this.ShowErrorPopup (ex.Message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}

		private void ShowHtmlPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportHtmlPopup (this.accessor)
			{
				Profile = LocalSettings.ExportHtmlProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportHtmlProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportHtml (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						this.ShowErrorPopup (ex.Message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}


		private void ExportText(ExportInstructions instructions, TextExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			var engine = new TextExport<T> ()
			{
				Instructions = instructions,
				Profile      = profile,
			};

			engine.Export (this.dataFiller);
		}

		private void ExportHtml(ExportInstructions instructions, HtmlExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			var engine = new HtmlExport<T> ()
			{
				Instructions = instructions,
				Profile      = profile,
			};

			engine.Export (this.dataFiller);
		}


		private void ShowOpenPopup(ExportInstructions instructions)
		{
			//	Affiche le popup (3) permettant d'ouvrir le fichier exporté ou l'emplacement,
			//	tout à la fin du processus.
			var popup = new ExportOpenPopup (this.accessor)
			{
				OpenLocation = false,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					if (popup.OpenLocation)
					{
						ExportEngine<T>.OpenLocation (instructions);
					}
					else
					{
						ExportEngine<T>.OpenFile (instructions);
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


		private void ShowErrorPopup(string message)
		{
			//	Affiche une erreur.
			MessagePopup.ShowMessage (this.target, "Exportation impossible", message);
		}


		private readonly Widget							target;
		private readonly DataAccessor					accessor;
		private readonly AbstractTreeTableFiller<T>		dataFiller;
	}
}