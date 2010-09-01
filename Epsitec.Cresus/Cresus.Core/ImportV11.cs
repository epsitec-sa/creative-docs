//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class ImportV11
	{
		public static void Import(CoreApplication application)
		{
			string filename = ImportV11.OpenFileDialog (application);

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			ImportV11.ImportFile (application, filename);
		}

		private static string OpenFileDialog(CoreApplication application)
		{
			var dialog = new FileOpenDialog ();

			dialog.Title = "Importation d'un fichier de paiements de type \"V11\"";
			//?dialog.InitialDirectory = "";
			//?dialog.FileName = "";

			dialog.Filters.Add ("v11", "Fichier de paiements", "*.v11;*.bvr;*.esr");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.Owner = application.Window;
			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			return dialog.FileName;
		}

		private static void ImportFile(CoreApplication application, string filename)
		{
			string[] lines;

			try
			{
				lines = System.IO.File.ReadAllLines (filename);
			}
			catch (System.Exception e)
			{
				string message = string.Format ("Impossible d'ouvrir le fichier {0}\n\n{1}", filename, e.ToString ());
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
			}

			// TODO:
		}
	}
}
