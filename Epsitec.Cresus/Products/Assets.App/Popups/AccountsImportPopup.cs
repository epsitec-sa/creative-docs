//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à l'importation d'un plan comptable.
	/// </summary>
	public class AccountsImportPopup : StackedPopup
	{
		public AccountsImportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Importation d'un plan comptable Crésus";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.ImportAccountsFilename,
				Label                 = "Fichier",
				Width                 = 300,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = AccountsMergeModeHelpers.MultiLabels,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Importer";
			this.defaultControllerRankFocus = 1;
		}


		public AccountsImportInstructions		ImportInstructions
		{
			get
			{
				AccountsMergeMode	mode;
				string				filename;

				{
					var controller = this.GetController (0) as ImportAccountsFilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					filename = controller.Value;
				}

				{
					var controller = this.GetController (1) as RadioStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					mode = AccountsMergeModeHelpers.GetMode (controller.Value.GetValueOrDefault ());
				}

				return new AccountsImportInstructions (mode, filename);
			}
			set
			{
				{
					var controller = this.GetController (0) as ImportAccountsFilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Filename;
				}

				{
					var controller = this.GetController (1) as RadioStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = AccountsMergeModeHelpers.GetRank (value.Mode);
				}
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			var controller = this.GetController (0) as ImportAccountsFilenameStackedController;
			System.Diagnostics.Debug.Assert (controller != null);
			controller.Value = this.ImportInstructions.Filename;
			controller.Update ();

			this.okButton.Enable = !string.IsNullOrEmpty (this.ImportInstructions.Filename);
		}
	}
}