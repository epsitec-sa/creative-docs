//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Dialogs
{
	public static class DialogsHelper
	{
		public static void ShowOpenMandat(Widget target, string directory, string filename, System.Action<string> action)
		{
			//	Affiche le dialogue permettant de choisir le fichier à ouvrir
			//	et effectue l'action correspondante.
			if (DialogsHelper.ShowMandatOpenDialog (target, ref directory, ref filename))
			{
				var path = System.IO.Path.Combine (directory, filename);
				action (path);
			}
		}

		public static void ShowSaveMandat(Widget target, string directory, string filename, SaveMandatMode mode, System.Action<string, SaveMandatMode> action)
		{
			//	Affiche le dialogue permettant de choisir le fichier à enregistrer
			//	et effectue l'action correspondante.
			if (DialogsHelper.ShowMandatSaveDialog (target, ref directory, ref filename))
			{
				var path = System.IO.Path.Combine (directory, filename);
				action (path, SaveMandatMode.SaveUI);
			}
		}

		public static void ShowImportAccounts(Widget target, string path, System.Action<string> action)
		{
			//	Affiche le dialogue permettant de choisir le plan comptable à importer
			//	et effectue l'action correspondante.
			var directory = System.IO.Path.GetDirectoryName (path);
			var filename  = System.IO.Path.GetFileName      (path);

			if (DialogsHelper.ShowAccountsOpenDialog (target, ref directory, ref filename))
			{
				path = System.IO.Path.Combine (directory, filename);
				action (path);
			}
		}

		public static void ShowExportData(Widget target, ExportFormat format, string path, System.Action<string> action)
		{
			//	Affiche le dialogue permettant de choisir le fichier à exporter
			//	et effectue l'action correspondante.
			var directory = System.IO.Path.GetDirectoryName (path);
			var filename  = System.IO.Path.GetFileName      (path);

			if (DialogsHelper.ShowDataSaveDialog (target, format, ref directory, ref filename))
			{
				path = System.IO.Path.Combine (directory, filename);
				action (path);
			}
		}


		private static bool ShowMandatOpenDialog(Widget target, ref string directory, ref string filename)
		{
			//	Affiche le dialogue permettant de choisir le fichier à ouvrir.
			var f = FileOpenDialog.ShowDialog (
				target.Window,
				Res.Strings.Popup.OpenMandat.Title.ToString (),
				directory,
				filename,
				IOHelpers.Extension,
				Res.Strings.Popup.OpenMandat.DialogFormatName.ToString ());

			if (string.IsNullOrEmpty (f))
			{
				return false;
			}
			else
			{
				directory = System.IO.Path.GetDirectoryName (f);
				filename  = System.IO.Path.GetFileName (f);

				return true;
			}
		}
		
		private static bool ShowMandatSaveDialog(Widget target, ref string directory, ref string filename)
		{
			//	Affiche le dialogue permettant de choisir le fichier à enregistrer, en mode 'PromptForOverwriting'.
			var f = FileSaveDialog.ShowDialog (
				target.Window,
				Res.Strings.Popup.SaveMandat.Title.ToString (),
				directory,
				filename,
				IOHelpers.Extension,
				Res.Strings.Popup.OpenMandat.DialogFormatName.ToString ());

			if (string.IsNullOrEmpty (f))
			{
				return false;
			}
			else
			{
				directory = System.IO.Path.GetDirectoryName (f);
				filename  = System.IO.Path.GetFileName (f);

				return true;
			}
		}

		private static bool ShowAccountsOpenDialog(Widget target, ref string directory, ref string filename)
		{
			//	Affiche le dialogue permettant de choisir un plan comptable à importer.
			var f = FileOpenDialog.ShowDialog (
				target.Window,
				Res.Strings.Popup.AccountsImport.DialogTitle.ToString (),
				directory,
				filename,
				".cre|.crp",
				Res.Strings.Popup.AccountsImport.DialogFormatName.ToString ());

			if (string.IsNullOrEmpty (f))
			{
				return false;
			}
			else
			{
				directory = System.IO.Path.GetDirectoryName (f);
				filename  = System.IO.Path.GetFileName (f);

				return true;
			}
		}

		private static bool ShowDataSaveDialog(Widget target, ExportFormat format, ref string directory, ref string filename)
		{
			//	Affiche le dialogue permettant de choisir un fichier à exporter.
			var f = FileSaveDialog.ShowDialog (
				target.Window,
				Res.Strings.Popup.Export.DialogTitle.ToString (),
				directory,
				filename,
				ExportInstructionsHelpers.GetFormatExt  (format),
				ExportInstructionsHelpers.GetFormatName (format));

			if (string.IsNullOrEmpty (f))
			{
				return false;
			}
			else
			{
				directory = System.IO.Path.GetDirectoryName (f);
				filename  = System.IO.Path.GetFileName (f);

				return true;
			}
		}
	}
}