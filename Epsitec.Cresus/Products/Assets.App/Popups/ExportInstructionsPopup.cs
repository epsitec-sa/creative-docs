//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Cresus.Assets.App.Export;
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
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Inverser les lignes avec les colonnes",
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
				var c0 = this.GetController (0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (c0 != null);
				var inverted = c0.Value;

				var c1 = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (c1 != null);
				var filename = c1.Value;

				return new ExportInstructions (filename, inverted);
			}
			set
			{
				var c0 = this.GetController (0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (c0 != null);
				c0.Value = value.Inverted;

				var c1 = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (c1 != null);
				c1.Value = value.Filename;
			}
		}

		public IEnumerable<FilterItem>			Filters
		{
			set
			{
				var controller = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Filters = value;
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
			this.okButton.Text = "Exporter";
			this.okButton.Enable = !string.IsNullOrEmpty (this.ExportInstructions.Filename);
		}
	}
}