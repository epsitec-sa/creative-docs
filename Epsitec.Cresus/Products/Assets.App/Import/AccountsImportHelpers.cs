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
	/// <summary>
	/// C'est ici que se trouve toute la logique interactive pour l'importation d'un plan
	/// comptable, avec la succession de popups jusqu'à l'importation elle-même.
	/// </summary>
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
			//	que le mode (Replace ou Merge).
			var popup = new AccountsImportPopup (this.accessor)
			{
				Filename = LocalSettings.AccountsImportFilename,
			};

			popup.Create (this.target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.AccountsImportFilename = popup.Filename;  // enregistre dans les réglages
					this.Import (popup.Filename);
				}
			};
		}

		private void Import(string filename)
		{
			if (this.ReadFile (filename))
			{
				this.ShowMessagePopup ("L'importation s'est effectuée avec succès.");
			}
		}

		private bool ReadFile(string filename)
		{
			//	Lit le fichier .crp et crée le moteur d'importation AccountsMerge.
			using (var importEngine = new AccountsImport ())
			{
				DateRange range;
				var importedAccounts = new GuidList<DataObject> ();

				try
				{
					range = importEngine.Import (importedAccounts, filename);
				}
				catch (System.Exception ex)
				{
					this.ShowMessagePopup (ex.Message);
					return false;
				}

				this.accessor.Mandat.AddAccounts (range, importedAccounts);
				return true;
			}
		}

		private void ShowMessagePopup(string message)
		{
			//	Affiche une erreur.
			MessagePopup.ShowMessage (this.target, "Importation", message);
		}


		private readonly DataAccessor			accessor;
		private readonly Widget					target;
		private readonly System.Action			updateAction;

		private AccountsMerge					accountsMerge;
	}
}