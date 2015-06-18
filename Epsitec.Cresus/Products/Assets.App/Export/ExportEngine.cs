//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Dialogs;
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
			//	(1) ExportFormatPopup pour choisir le format.
			//	(2) ExportXxxPopup pour choisir le profile (selon le format).
			//	(3) ShowExportData pour choisir le fichier (selon le format).
			//	(4) ExportOpenPopup pour ouvrir le fichier exporté ou l'emplacement.

			ExportFormatPopup.Show (this.target, this.accessor, LocalSettings.ExportFormat, delegate (ExportFormat format)
			{
				LocalSettings.ExportFormat = format;
				this.ShowProfilePopup (format);
			});
		}


		private void ShowProfilePopup(ExportFormat format)
		{
			//	Choix du profile selon le format, puis continue le processus.
			switch (format)
			{
				case ExportFormat.Txt:
					this.ShowTxtPopup (format);
					break;

				case ExportFormat.Csv:
					this.ShowCsvPopup (format);
					break;

				case ExportFormat.Xml:
					this.ShowXmlPopup (format);
					break;

				case ExportFormat.Yaml:
					this.ShowYamlPopup (format);
					break;

				case ExportFormat.Json:
					this.ShowJsonPopup (format);
					break;

				case ExportFormat.Pdf:
					this.ShowPdfPopup (format);
					break;

				default:
					var ext = ExportInstructionsHelpers.GetFormatExt (format).Replace (".", "").ToUpper ();
					var message = string.Format (Res.Strings.Export.Engine.UnknownFormat.ToString (), ext);
					this.ShowErrorPopup (message);
					break;
			}
		}


		private void ShowTxtPopup(ExportFormat format)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus.
			ExportTextPopup.Show (this.target, this.accessor, LocalSettings.ExportTxtProfile, delegate (TextExportProfile profile)
			{
				LocalSettings.ExportTxtProfile = profile;  // enregistre dans les réglages

				try
				{
					//	Choix du nom du fichier (3).
					DialogsHelper.ShowExportData (target, format, LocalSettings.ExportTxtFilename, delegate (string filename)
					{
						LocalSettings.ExportTxtFilename = filename;
						this.ExportText (format, profile, filename);
						this.ShowOpenPopup (filename);
					});
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowErrorPopup (message);
					return;
				}
			});
		}

		private void ShowCsvPopup(ExportFormat format)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus.
			ExportTextPopup.Show (this.target, this.accessor, LocalSettings.ExportCsvProfile, delegate (TextExportProfile profile)
			{
				LocalSettings.ExportCsvProfile = profile;  // enregistre dans les réglages

				try
				{
					//	Choix du nom du fichier (3).
					DialogsHelper.ShowExportData (target, format, LocalSettings.ExportCsvFilename, delegate (string filename)
					{
						LocalSettings.ExportCsvFilename = filename;
						this.ExportText (format, profile, filename);
						this.ShowOpenPopup (filename);
					});
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowErrorPopup (message);
					return;
				}
			});
		}

		private void ShowXmlPopup(ExportFormat format)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus.
			ExportXmlPopup.Show (this.target, this.accessor, LocalSettings.ExportXmlProfile, delegate (XmlExportProfile profile)
			{
				LocalSettings.ExportXmlProfile = profile;  // enregistre dans les réglages

				try
				{
					//	Choix du nom du fichier (3).
					DialogsHelper.ShowExportData (target, format, LocalSettings.ExportXmlFilename, delegate (string filename)
					{
						LocalSettings.ExportXmlFilename = filename;
						this.ExportXml (format, profile, filename);
						this.ShowOpenPopup (filename);
					});
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowErrorPopup (message);
					return;
				}
			});
		}

		private void ShowYamlPopup(ExportFormat format)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus.
			ExportYamlPopup.Show (this.target, this.accessor, LocalSettings.ExportYamlProfile, delegate (YamlExportProfile profile)
			{
				LocalSettings.ExportYamlProfile = profile;  // enregistre dans les réglages

				try
				{
					//	Choix du nom du fichier (3).
					DialogsHelper.ShowExportData (target, format, LocalSettings.ExportYamlFilename, delegate (string filename)
					{
						LocalSettings.ExportYamlFilename = filename;
						this.ExportYaml (format, profile, filename);
						this.ShowOpenPopup (filename);
					});
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowErrorPopup (message);
					return;
				}
			});
		}

		private void ShowJsonPopup(ExportFormat format)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus.
			ExportJsonPopup.Show (this.target, this.accessor, LocalSettings.ExportJsonProfile, delegate (JsonExportProfile profile)
			{
				LocalSettings.ExportJsonProfile = profile;  // enregistre dans les réglages

				try
				{
					//	Choix du nom du fichier (3).
					DialogsHelper.ShowExportData (target, format, LocalSettings.ExportJsonFilename, delegate (string filename)
					{
						LocalSettings.ExportJsonFilename = filename;
						this.ExportJson (format, profile, filename);
						this.ShowOpenPopup (filename);
					});
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowErrorPopup (message);
					return;
				}
			});
		}

		private void ShowPdfPopup(ExportFormat format)
		{
			//	Ouvre le popup (2) pour choisir le profile d'exportation, puis continue le processus.
			ExportPdfPopup.Show (this.target, this.accessor, LocalSettings.ExportPdfProfile, delegate (PdfExportProfile profile)
			{
				LocalSettings.ExportPdfProfile = profile;  // enregistre dans les réglages

				try
				{
					//	Choix du nom du fichier (3).
					DialogsHelper.ShowExportData (target, format, LocalSettings.ExportPdfFilename, delegate (string filename)
					{
						LocalSettings.ExportPdfFilename = filename;
						this.ExportPdf (format, profile, filename);
						this.ShowOpenPopup (filename);
					});
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowErrorPopup (message);
					return;
				}
			});
		}


		private void ExportText(ExportFormat format, TextExportProfile profile, string filename)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new TextExport<T> ())
			{
				engine.Export (this.accessor, format, profile, filename, this.dataFiller, this.columnsState);
			}
		}

		private void ExportXml(ExportFormat format, XmlExportProfile profile, string filename)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new XmlExport<T> ())
			{
				engine.Export (this.accessor, format, profile, filename, this.dataFiller, this.columnsState);
			}
		}

		private void ExportYaml(ExportFormat format, YamlExportProfile profile, string filename)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new YamlExport<T> ())
			{
				engine.Export (this.accessor, format, profile, filename, this.dataFiller, this.columnsState);
			}
		}

		private void ExportJson(ExportFormat format, JsonExportProfile profile, string filename)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new JsonExport<T> ())
			{
				engine.Export (this.accessor, format, profile, filename, this.dataFiller, this.columnsState);
			}
		}

		private void ExportPdf(ExportFormat format, PdfExportProfile profile, string filename)
		{
			//	Exporte les données, selon les instructions et le profile, sans aucune interaction.
			using (var engine = new PdfExport<T> ())
			{
				engine.Export (this.accessor, format, profile, filename, this.dataFiller, this.columnsState);
			}
		}


		private void ShowOpenPopup(string filename)
		{
			//	Affiche le popup (4) permettant d'ouvrir le fichier exporté ou l'emplacement,
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
						ExportEngine<T>.OpenLocation (filename);
					}
					else
					{
						ExportEngine<T>.OpenFile (filename);
					}
				}
			};
		}

		private static void OpenFile(string filename)
		{
			//	Ouvre le fichier, en lançant l'application par défaut selon l'extension.
			System.Diagnostics.Process.Start (filename);
		}

		private static void OpenLocation(string filename)
		{
			//	Ouvre l'explorateur de fichier et sélectionne le fichier exporté.
			//	Voir http://stackoverflow.com/questions/9646114/open-file-location
			System.Diagnostics.Process.Start ("explorer.exe", "/select," + filename);
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