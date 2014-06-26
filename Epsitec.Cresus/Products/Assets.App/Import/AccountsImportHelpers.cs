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

			if (instructions.Mode == AccountsMergeMode.Merge && this.accountsMerge.HasCurrentAccounts)
			{
				if (this.accountsMerge.Todo.Any ())
				{
					this.ShowMergePopup ();
				}
				else
				{
					this.ShowErrorPopup ("Il n'y a aucun compte à fusionner.<br/>Le plan comptable est à jour.");
				}
			}
			else
			{
				if (CategoriesLogic.HasAccounts (this.accessor))
				{
					this.ShowReplacePopup ();
				}
				else
				{
					this.ReplaceImport ();
				}
			}
		}

		private void ShowReplacePopup()
		{
			const string question = "L'importation effacera tous les comptes dans les catégories d'immobilisation. Etes-vous certain de vouloir continuer ?";
			YesNoPopup.Show (this.target, question, this.ReplaceImport);
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
					this.accountsMerge.Do ();  // effectue l'importation en mode Merge
					this.updateAction ();
				}
			};
		}


		private bool ReadFile(AccountsImportInstructions instructions)
		{
			//	Lit le fichier .crp et crée le moteur d'importation AccountsMerge.
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

		private void ReplaceImport()
		{
			this.accountsMerge.Do ();  // effectue l'importation en mode Replace
			CategoriesLogic.ClearAccounts (this.accessor);
			this.updateAction ();
		}


		private void ShowErrorPopup(string message)
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