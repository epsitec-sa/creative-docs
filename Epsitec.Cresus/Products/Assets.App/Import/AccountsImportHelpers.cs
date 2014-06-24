//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Export
{
	public class AccountsImportHelpers : System.IDisposable
	{
		public AccountsImportHelpers(DataAccessor accessor, Widget target, System.Action updateAction)
		{
			this.accessor     = accessor;
			this.target       = target;
			this.updateAction = updateAction;
		}

		public void Dispose()
		{
			if (this.accountsMerge != null)
			{
				this.accountsMerge.Dispose ();
			}
		}


		public void ShowImportPopup()
		{
			//	Affiche le popup permettant de choisir un plan comptable à importer, ainsi
			//	que le mode.
			var popup = new AccountsImportPopup (this.accessor)
			{
				ImportInstructions = LocalSettings.AccountsImportInstructions,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.AccountsImportInstructions = popup.ImportInstructions;  // enregistre dans les réglages
					this.Import (popup.ImportInstructions);
				}
			};
		}

		private void Import(AccountsImportInstructions instructions)
		{
			if (!this.ReadFile (instructions))
			{
				return;
			}

			if (instructions.Mode == AccountsMergeMode.Merge)
			{
				this.ShowMergePopup ();
			}
			else
			{
				this.accountsMerge.Merge ();
				this.updateAction ();
			}
		}

		private void ShowMergePopup()
		{
			//	Affiche le popup permettant de choisir comment effectuer la fusion.
			var popup = new AccountsMergePopup (this.accessor, this.accountsMerge.Todo);

			popup.Create (this.target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.accountsMerge.Merge ();
					this.updateAction ();
				}
			};
		}


		private bool ReadFile(AccountsImportInstructions instructions)
		{
			var currentAccounts = this.accessor.Mandat.GetData (BaseType.Accounts);

			using (var importEngine = new AccountsImport ())
			{
				var importedAccounts = new GuidList<DataObject> ();

				try
				{
					importEngine.Import (importedAccounts, instructions.Filename);
				}
				catch (System.Exception ex)
				{
					this.ShowErrorPopup (ex.Message);
					return false;
				}

				this.accountsMerge = new AccountsMerge (currentAccounts, importedAccounts, instructions.Mode);
				return true;
			}
		}


		private void ShowErrorPopup(string message)
		{
			//	Affiche une erreur.
			MessagePopup.ShowMessage (this.target, "Importation impossible", message);
		}


		private readonly DataAccessor			accessor;
		private readonly Widget					target;
		private readonly System.Action			updateAction;

		private AccountsMerge					accountsMerge;
	}
}