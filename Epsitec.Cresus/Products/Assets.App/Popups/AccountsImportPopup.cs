//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup affichant un résumé de l'impact de l'importation d'un plan comptable.
	/// </summary>
	public class AccountsImportPopup : AbstractStackedPopup
	{
		public AccountsImportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.Accounts.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 400,
				Height                = 15*5,  // place pour 5 lignes du rapport
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Import.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public string							Filename;


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			var report = this.Report;

			//	Met à jour le rapport.
			{
				var controller = this.GetController (0) as LabelStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetLabel (report.Message);
			}

			this.okButton.Enable = (report.Mode != AccountsImportMode.Error);
		}


		private AccountsImportReport Report
		{
			//	Retourne le rapport sur le plan comptable à importer.
			get
			{
				using (var h = new AccountsImportHelpers (this.accessor, null, null))
				{
					return h.GetReport (this.Filename);
				}
			}
		}
	}
}