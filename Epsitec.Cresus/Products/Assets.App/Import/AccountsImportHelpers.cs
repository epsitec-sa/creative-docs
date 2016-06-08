﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Dialogs;
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


		public AccountsImportReport GetReport(string filename)
		{
			//	Retourne le rapport sur la future importation d'un plan comptable.
			using (var importEngine = new AccountsImport ())
			{
				var importedAccounts = new GuidDictionary<DataObject> (this.accessor.UndoManager);

				var report = string.Format (Res.Strings.Popup.AccountsImport.Report.ToString (), filename);

				try
				{
					var range = importEngine.Import (importedAccounts, null, filename);

					bool existing = this.accessor.Mandat.AccountsDateRanges.Contains (range);
					if (existing)
					{
						bool equal = AccountsLogic.Compare (importedAccounts, this.accessor.Mandat.GetAccounts (range));
						if (equal)
						{
							string message = Res.Strings.AccountsImport.Message.Equal.ToString ();
							return new AccountsImportReport (AccountsImportMode.Error, report + message);
						}
						else
						{
							string message = string.Format (Res.Strings.AccountsImport.Message.Update.ToString (), range.ToNiceString (), importedAccounts.Count);
							return new AccountsImportReport (AccountsImportMode.Update, report + message);
						}
					}
					else
					{
						string message = string.Format (Res.Strings.AccountsImport.Message.New.ToString (), range.ToNiceString (), importedAccounts.Count);
						return new AccountsImportReport (AccountsImportMode.Add, report + message);
					}
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					return new AccountsImportReport (AccountsImportMode.Error, report + message);
				}
			}
		}

		public void ShowImportPopup()
		{
			//	Affiche le dialogue standard de Windows pour choisir le plan comptable à importer,
			//	puis affiche le popup de résumé, puis effectue l'importation.

			//	Affiche le dialogue standard de Windows.
			DialogsHelper.ShowImportAccounts (this.target, LocalSettings.AccountsImportFilename, delegate (string path)
			{
				LocalSettings.AccountsImportFilename = path;

				//	Affiche le popup de résumé.
				AccountsImportPopup.Show (this.target, this.accessor, path, delegate
				{
					this.Import (path);
				});
			});
		}


		public void ChangePath(DateRange dateRange)
		{
			//	Affiche le dialogue standard de Windows pour choisir le plan comptable, puis modifie le
			//	chemin d'accès.
			var filename = this.accessor.Mandat.GetAccountsFilename (dateRange);

			DialogsHelper.ShowImportAccounts (this.target, filename, delegate (string path)
			{
				LocalSettings.AccountsImportFilename = path;
				this.Import (path);  // on refait l'importation
			});
		}

		public void Delete(DateRange dateRange)
		{
			//	Supprime un plan comptable de la liste des plans comptables dans le mandat.
			this.accessor.UndoManager.Start ();

			this.accessor.Mandat.DeleteAccounts         (dateRange);
			this.accessor.Mandat.DeleteVatCodes         (dateRange);
			this.accessor.Mandat.DeleteAccountsFilename (dateRange);

			this.accessor.WarningsDirty = true;
			this.updateAction ();

			var op = string.Concat (Res.Commands.Accounts.Delete.Description, " — ", dateRange.ToNiceString ());
			var desc = UndoManager.GetDescription (op, null);
			this.accessor.UndoManager.SetDescription (desc);
			this.accessor.UndoManager.SetAfterViewState ();
		}


		private void Import(string filename)
		{
			//	Lit le fichier .crp et ajoute-le à la liste des plans comptables dans le mandat.
			this.accessor.UndoManager.Start ();

			using (var importEngine = new AccountsImport ())
			{
				DateRange range;
				var importedAccounts = new GuidDictionary<DataObject> (this.accessor.UndoManager);
				var importedVatCodes = new GuidDictionary<DataObject> (this.accessor.UndoManager);

				try
				{
					range = importEngine.Import (importedAccounts, importedVatCodes, filename);
				}
				catch (System.Exception ex)
				{
					string message = TextLayout.ConvertToTaggedText (ex.Message);
					this.ShowMessagePopup (message);
					return;
				}

				this.accessor.Mandat.AddAccounts         (range, importedAccounts);
				this.accessor.Mandat.AddVatCodes         (range, importedVatCodes);
				this.accessor.Mandat.AddAccountsFilename (range, filename);
				//?this.accessor.Mandat.CurrentAccountsDateRange = range;
				this.accessor.WarningsDirty = true;

				this.updateAction ();

				//	N'affiche rien lors d'une importation effectuée avec succès.
			}

			var desc = UndoManager.GetDescription (Res.Commands.Accounts.Import.Description, null);
			this.accessor.UndoManager.SetDescription (desc);
			this.accessor.UndoManager.SetAfterViewState ();
		}

		private void ShowMessagePopup(string message)
		{
			//	Affiche une erreur.
			MessagePopup.ShowMessage (this.target, Res.Strings.Popup.Message.AccountsImport.Title.ToString (), message);
		}


		private readonly DataAccessor			accessor;
		private readonly Widget					target;
		private readonly System.Action			updateAction;
	}
}