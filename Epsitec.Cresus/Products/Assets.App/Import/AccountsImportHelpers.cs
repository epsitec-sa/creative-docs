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
		public AccountsImportHelpers(DataAccessor accessor, Widget target)
		{
			this.accessor = accessor;
			this.target   = target;
		}

		public void Dispose()
		{
		}


		public void ShowImportPopup()
		{
			//	Affiche le popup permettant de choisir un plan comptable à importer.
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
					this.Toto (popup.ImportInstructions);
				}
			};
		}

		private void Toto(AccountsImportInstructions instructions)
		{
			if (instructions.Mode == AccountsMergeMode.Merge)
			{
				this.ShowMergePopup (instructions);
			}
			else
			{
				this.AccountsImport (instructions);
			}
		}

		private void ShowMergePopup(AccountsImportInstructions instructions)
		{
			var todo = this.Todo (instructions);
			var popup = new AccountsMergePopup (this.accessor, todo);

			popup.Create (this.target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				//?if (name == "ok")
				//?{
				//?	LocalSettings.AccountsImportInstructions = popup.ImportInstructions;  // enregistre dans les réglages
				//?	this.AccountsImport (popup.ImportInstructions);
				//?	this.UpdateData ();
				//?}
			};
		}

		private Dictionary<DataObject, DataObject> Todo(AccountsImportInstructions instructions)
		{
			using (var importEngine = new AccountsImport ())
			{
				var importedData = new GuidList<DataObject> ();
				var currentData = this.accessor.Mandat.GetData (BaseType.Accounts);

				try
				{
					importEngine.Import (importedData, instructions.Filename);
				}
				catch (System.Exception ex)
				{
					this.ShowErrorPopup (ex.Message);
					return null;
				}

				using (var mergeEngine = new AccountsMerge ())
				{
					return mergeEngine.Todo (currentData, importedData, instructions.Mode);
				}
			}
		}

		private void AccountsImport(AccountsImportInstructions instructions)
		{
			using (var importEngine = new AccountsImport ())
			{
				var importedData = new GuidList<DataObject> ();
				var currentData = this.accessor.Mandat.GetData (BaseType.Accounts);

				try
				{
					importEngine.Import (importedData, instructions.Filename);
				}
				catch (System.Exception ex)
				{
					this.ShowErrorPopup (ex.Message);
					return;
				}

				this.Merge (currentData, importedData, instructions.Mode);

			}
		}

		private void Merge(GuidList<DataObject> current, GuidList<DataObject> import, AccountsMergeMode mode)
		{
			using (var mergeEngine = new AccountsMerge ())
			{
				mergeEngine.Merge (current, import, mode);
			}
		}


		private void ShowErrorPopup(string message)
		{
			//	Affiche une erreur.
			MessagePopup.ShowMessage (this.target, "Importation impossible", message);
		}


		private readonly DataAccessor			accessor;
		private readonly Widget					target;
	}
}