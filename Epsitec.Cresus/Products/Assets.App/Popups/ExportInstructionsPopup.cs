﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
				MultiLabels           = ExportInstructionsHelpers.MultiLabels,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.ExportFilename,
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
					format = ExportInstructionsHelpers.GetFormat (controller.Value.GetValueOrDefault ());
				}

				{
					var controller = this.GetController (1) as ExportFilenameStackedController;
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
					controller.Value = ExportInstructionsHelpers.GetRank (value.Format);
				}

				{
					var controller = this.GetController (1) as ExportFilenameStackedController;
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

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			var controller = this.GetController (1) as ExportFilenameStackedController;
			System.Diagnostics.Debug.Assert (controller != null);
			controller.Format = this.ExportInstructions.Format;
			controller.Value = ExportInstructionsHelpers.ForceExt (controller.Value, ExportInstructionsHelpers.GetFormatExt (this.ExportInstructions.Format));
			controller.Update ();

			this.okButton.Text = "Exporter";
			this.okButton.Enable = !string.IsNullOrEmpty (this.ExportInstructions.Filename);
		}
	}
}