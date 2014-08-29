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
	public class ExportEngine<T> : System.IDisposable
		where T : struct
	{
		public ExportEngine(Widget target, DataAccessor accessor, AbstractTreeTableFiller<T> dataFiller, ColumnsState columnsState)
		{
			this.target       = target;
			this.accessor     = accessor;
			this.dataFiller   = dataFiller;
			this.columnsState = columnsState;
		}

		public void Dispose()
		{
		}


		public void StartExportProcess()
		{
			//	Débute le processus d'exportation qui ouvrira plusieurs popups successifs:
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
				case ExportFormat.Txt:
					this.ShowTxtPopup (instructions);
					break;

				case ExportFormat.Csv:
					this.ShowCsvPopup (instructions);
					break;

				case ExportFormat.Xml:
					this.ShowXmlPopup (instructions);
					break;

				case ExportFormat.Yaml:
					this.ShowYamlPopup (instructions);
					break;

				case ExportFormat.Json:
					this.ShowJsonPopup (instructions);
					break;

				case ExportFormat.Pdf:
					this.ShowPdfPopup (instructions);
					break;

				default:
					var ext = ExportInstructionsHelpers.GetFormatExt (instructions.Format).Replace (".", "").ToUpper ();
					var message = string.Format (Res.Strings.Export.Engine.UnknownFormat.ToString (), ext);
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
						string message = TextLayout.ConvertToTaggedText (ex.Message);
						this.ShowErrorPopup (message);
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
						string message = TextLayout.ConvertToTaggedText (ex.Message);
						this.ShowErrorPopup (message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}

		private void ShowXmlPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportXmlPopup (this.accessor)
			{
				Profile = LocalSettings.ExportXmlProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportXmlProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportXml (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						string message = TextLayout.ConvertToTaggedText (ex.Message);
						this.ShowErrorPopup (message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}

		private void ShowYamlPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportYamlPopup (this.accessor)
			{
				Profile = LocalSettings.ExportYamlProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportYamlProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportYaml (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						string message = TextLayout.ConvertToTaggedText (ex.Message);
						this.ShowErrorPopup (message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}

		private void ShowJsonPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportJsonPopup (this.accessor)
			{
				Profile = LocalSettings.ExportJsonProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportJsonProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportJson (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						string message = TextLayout.ConvertToTaggedText (ex.Message);
						this.ShowErrorPopup (message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}

		private void ShowPdfPopup(ExportInstructions instructions)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus
			var popup = new ExportPdfPopup (this.accessor)
			{
				Profile = LocalSettings.ExportPdfProfile,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportPdfProfile = popup.Profile;  // enregistre dans les réglages

					try
					{
						this.ExportPdf (instructions, popup.Profile);
					}
					catch (System.Exception ex)
					{
						string message = TextLayout.ConvertToTaggedText (ex.Message);
						this.ShowErrorPopup (message);
						return;
					}

					this.ShowOpenPopup (instructions);
				}
			};
		}


		private void ExportText(ExportInstructions instructions, TextExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new TextExport<T> ())
			{
				engine.Export (instructions, profile, this.dataFiller, this.columnsState);
			}
		}

		private void ExportXml(ExportInstructions instructions, XmlExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new XmlExport<T> ())
			{
				engine.Export (instructions, profile, this.dataFiller, this.columnsState);
			}
		}

		private void ExportYaml(ExportInstructions instructions, YamlExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new YamlExport<T> ())
			{
				engine.Export (instructions, profile, this.dataFiller, this.columnsState);
			}
		}

		private void ExportJson(ExportInstructions instructions, JsonExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new JsonExport<T> ())
			{
				engine.Export (instructions, profile, this.dataFiller, this.columnsState);
			}
		}

		private void ExportPdf(ExportInstructions instructions, PdfExportProfile profile)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new PdfExport<T> ())
			{
				engine.Export (instructions, profile, this.dataFiller, this.columnsState);
			}
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
			MessagePopup.ShowMessage (this.target, Res.Strings.Popup.Message.ExportError.Text.ToString (), message);
		}


		private readonly Widget							target;
		private readonly DataAccessor					accessor;
		private readonly AbstractTreeTableFiller<T>		dataFiller;
		private readonly ColumnsState					columnsState;
	}
}