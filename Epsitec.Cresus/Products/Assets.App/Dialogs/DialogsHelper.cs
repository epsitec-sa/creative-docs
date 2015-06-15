//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.App.Dialogs
{
	public static class DialogsHelper
	{
		public static void ShowOpen(Widget target, string directory, string filename, System.Action<string> action)
		{
			//	Affiche le dialogue permettant de choisir le fichier à ouvrir
			//	et effectue l'action correspondante.
			if (DialogsHelper.ShowFilenameOpenDialog (target, ref directory, ref filename))
			{
				var path = System.IO.Path.Combine (directory, filename);
				action (path);
			}
		}

		public static void ShowSave(Widget target, string directory, string filename, SaveMandatMode mode, System.Action<string, SaveMandatMode> action)
		{
			//	Affiche le dialogue permettant de choisir le fichier à enregistrer
			//	et effectue l'action correspondante.
			if (DialogsHelper.ShowFilenameSaveDialog (target, ref directory, ref filename))
			{
				var path = System.IO.Path.Combine (directory, filename);
				action (path, SaveMandatMode.SaveUI);
			}
		}


		private static bool ShowFilenameOpenDialog(Widget target, ref string directory, ref string filename)
		{
			//	Affiche le dialogue permettant de choisir le fichier à ouvrir.
			var f = FileOpenDialog.ShowDialog (
				target.Window,
				Res.Strings.Popup.OpenMandat.Title.ToString (),
				directory,
				filename,
				IOHelpers.Extension, Res.Strings.Popup.OpenMandat.DialogFormatName.ToString ());

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
		
		private static bool ShowFilenameSaveDialog(Widget target, ref string directory, ref string filename)
		{
			//	Affiche le dialogue permettant de choisir le fichier à enregistrer, en mode 'PromptForOverwriting'.
			var f = FileSaveDialog.ShowDialog (
				target.Window,
				Res.Strings.Popup.SaveMandat.Title.ToString (),
				directory,
				filename,
				IOHelpers.Extension, Res.Strings.Popup.OpenMandat.DialogFormatName.ToString ());

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