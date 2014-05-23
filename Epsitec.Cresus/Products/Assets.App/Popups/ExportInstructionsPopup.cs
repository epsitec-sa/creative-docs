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
				var c0 = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (c0 != null);
				var format = ExportInstructionsPopup.GetFormat (c0.Value.GetValueOrDefault ());

				var c1 = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (c1 != null);
				var filename = c1.Value;

				return new ExportInstructions (format, filename);
			}
			set
			{
				var c0 = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (c0 != null);
				c0.Value = ExportInstructionsPopup.GetRank (value.Format);

				var c1 = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (c1 != null);
				c1.Value = value.Filename;
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
			controller.Value = System.IO.Path.Combine (
				System.IO.Path.GetDirectoryName (controller.Value),
				System.IO.Path.GetFileNameWithoutExtension (controller.Value) + ExportInstructionsPopup.GetFormatExt (this.ExportInstructions.Format));
			controller.Update ();

			this.okButton.Text = "Exporter";
			this.okButton.Enable = !string.IsNullOrEmpty (this.ExportInstructions.Filename);
		}


		private static ExportFormat GetFormat(int rank)
		{
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
			var list = ExportInstructionsPopup.Formats.ToList ();
			return list.IndexOf (format);
		}

		private static string MultiLabels
		{
			get
			{
				return string.Join ("<br/>", ExportInstructionsPopup.Formats.Select (x => ExportInstructionsPopup.GetFormatName (x)));
			}
		}

		public static string GetFormatName(ExportFormat format)
		{
			switch (format)
			{
				case ExportFormat.Text:
					return "Fichier texte tabulé";

				case ExportFormat.Csv:
					return "Fichier csv";

				case ExportFormat.Html:
					return "Fichier html";

				case ExportFormat.Pdf:
					return "Document pdf mis en pages";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid format", format));
			}
		}

		public static string GetFormatExt(ExportFormat format)
		{
			switch (format)
			{
				case ExportFormat.Text:
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
			get
			{
				yield return ExportFormat.Text;
				yield return ExportFormat.Csv;
				yield return ExportFormat.Html;
				yield return ExportFormat.Pdf;
			}
		}
	}
}