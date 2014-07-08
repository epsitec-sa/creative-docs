//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
		}


		public bool GetReport(string filename, out string report)
		{
			//	Retourne le rapport sur la future importation d'un plan comptable.
			//	Retourne true si l'importation est possible.
			using (var importEngine = new AccountsImport ())
			{
				var importedAccounts = new GuidList<DataObject> ();

				try
				{
					var range = importEngine.Import (importedAccounts, filename);

					bool existing = this.accessor.Mandat.AccountsDateRanges.Contains (range);
					if (existing)
					{
						bool equal = AccountsLogic.Compare (importedAccounts, this.accessor.Mandat.GetAccounts (range));
						if (equal)
						{
							report = "L'importation est inutile.<br/>Ce plan comptable a déjà été importé.";
							return false;  // erreur
						}
						else
						{
							report = string.Format ("Mise à jour de la période {0}.<br/>{1} comptes à importer.", range.ToNiceString (), importedAccounts.Count);
							return true;  // ok pour l'importation
						}
					}
					else
					{
						report = string.Format ("Nouvelle période {0}.<br/>{1} comptes à importer.", range.ToNiceString (), importedAccounts.Count);
						return true;  // ok pour l'importation
					}
				}
				catch (System.Exception ex)
				{
					report = TextLayout.ConvertToTaggedText (ex.Message);
					return false;  // erreur
				}
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
			//	Lit le fichier .crp et ajoute-le à la liste des plans comptables dans le mandat.
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
					return;
				}

				this.accessor.Mandat.AddAccounts (range, importedAccounts);
				this.accessor.Mandat.CurrentAccountsDateRange = range;

				this.updateAction ();
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
	}
}