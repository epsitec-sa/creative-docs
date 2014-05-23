//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à l'exportation d'un TreeTable.
	/// </summary>
	public class ExportInstructionsPopup : StackedPopup
	{
		public ExportInstructionsPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Exportation des données";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = ExportInstructionsPopup.MultiLabels,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Filename,
				Label                 = "Fichier",
				Width                 = 300,
			});

			this.SetDescriptions (list);
		}


		public ExportInstructions				ExportInstructions
		{
			get
			{
				ExportFormat	format;
				string			filename;

				{
					var controller = this.GetController (0) as RadioStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					format = ExportInstructionsPopup.GetFormat (controller.Value.GetValueOrDefault ());
				}

				{
					var controller = this.GetController (1) as FilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					filename = controller.Value;
				}

				return new ExportInstructions (format, filename);
			}
			set
			{
				{
					var controller = this.GetController (0) as RadioStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = ExportInstructionsPopup.GetRank (value.Format);
				}

				{
					var controller = this.GetController (1) as FilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Filename;
				}
			}
		}


		public override void CreateUI()
		{
			base.CreateUI ();

			var controller = this.GetController (1);
			controller.SetFocus ();
		}

		protected override void UpdateWidgets()
		{
			var controller = this.GetController (1) as FilenameStackedController;
			System.Diagnostics.Debug.Assert (controller != null);
			controller.Format = this.ExportInstructions.Format;
			controller.Value = ExportInstructionsPopup.ForceExt (controller.Value, ExportInstructionsPopup.GetFormatExt (this.ExportInstructions.Format));
			controller.Update ();

			this.okButton.Text = "Exporter";
			this.okButton.Enable = !string.IsNullOrEmpty (this.ExportInstructions.Filename);
		}


		private static string ForceExt(string filename, string ext)
		{
			//	Retourne un nom de fichier complet (avec chemin d'accès) dont on a forcé l'extension.
			return System.IO.Path.Combine (
				System.IO.Path.GetDirectoryName (filename),
				System.IO.Path.GetFileNameWithoutExtension (filename) + ext);
		}


		private static ExportFormat GetFormat(int rank)
		{
			//	Retourne un format d'après son rang.
			var list = ExportInstructionsPopup.Formats.ToArray ();

			if (rank >= 0 && rank < list.Length)
			{
				return list[rank];
			}
			else
			{
				return ExportFormat.Unknown;
			}
		}

		private static int GetRank(ExportFormat format)
		{
			//	Retourne le rang d'un format, ou -1.
			var list = ExportInstructionsPopup.Formats.ToList ();
			return list.IndexOf (format);
		}

		private static string MultiLabels
		{
			//	Retourne le texte permettant de créer des boutons radios.
			get
			{
				return string.Join ("<br/>", ExportInstructionsPopup.Formats.Select (x => ExportInstructionsPopup.GetFormatName (x)));
			}
		}

		public static string GetFormatName(ExportFormat format)
		{
			//	Retourne le nom en clair d'un format.
			switch (format)
			{
				case ExportFormat.Txt:
					return "TXT — Fichier texte tabulé";

				case ExportFormat.Csv:
					return "CSV — Fichier texte pour tableur";

				case ExportFormat.Html:
					return "HTML — Fichier texte avec balises";

				case ExportFormat.Pdf:
					return "PDF — Document mis en pages";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid format", format));
			}
		}

		public static string GetFormatExt(ExportFormat format)
		{
			//	Retourne l'extension pour un format.
			switch (format)
			{
				case ExportFormat.Txt:
					return ".txt";

				case ExportFormat.Csv:
					return ".csv";

				case ExportFormat.Html:
					return ".html";

				case ExportFormat.Pdf:
					return ".pdf";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid format", format));
			}
		}

		private static IEnumerable<ExportFormat> Formats
		{
			//	Enumère tous les formats disponibles, par ordre d'importance.
			get
			{
				yield return ExportFormat.Txt;
				yield return ExportFormat.Csv;
				yield return ExportFormat.Html;
				yield return ExportFormat.Pdf;
			}
		}
	}
}