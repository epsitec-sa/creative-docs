//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à l'exportation d'un TreeTable.
	/// </summary>
	public class ExportPopup : StackedPopup
	{
		public ExportPopup(DataAccessor accessor)
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


		public bool								Inverted
		{
			get
			{
				var controller = this.GetController (0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public string							Filename
		{
			get
			{
				var controller = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
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
			this.okButton.Enable = !string.IsNullOrEmpty (this.Filename);
		}
	}
}