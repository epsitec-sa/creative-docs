﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
				StackedControllerType = StackedControllerType.Label,
				Width                 = 300,
				Height                = 15*2,  // place pour 2 lignes
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Importer";
			this.defaultControllerRankFocus = 0;
		}


		public string							Filename
		{
			get
			{
				string				filename;

				{
					var controller = this.GetController (0) as ImportAccountsFilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					filename = controller.Value;
				}

				return filename;
			}
			set
			{
				{
					var controller = this.GetController (0) as ImportAccountsFilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value;
				}
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			string accountsDescription;
			bool ok = this.GetReport (out accountsDescription);

			{
				var controller = this.GetController (0) as ImportAccountsFilenameStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = this.Filename;
				controller.Update ();
			}

			{
				var controller = this.GetController (1) as LabelStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetLabel (accountsDescription);
			}

			this.okButton.Enable = ok;
		}


		private bool GetReport(out string report)
		{
			//	Retourne le rapport sur le plan comptable à importer.
			using (var h = new AccountsImportHelpers (this.accessor, null, null))
			{
				return h.GetReport (this.Filename, out report);
			}
		}
	}
}