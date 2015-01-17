//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Dialogs
{
	public static class FileOpenDialog
	{
		public static string ShowDialog(Window parent, string title, string initialDirectory, string filename, string ext, string formatName)
		{
			//	Affiche le dialogue Windows standard permettant de choisir un fichier à ouvrir.
			var dialog = new Epsitec.Common.Dialogs.FileOpenDialog
			{
				InitialDirectory = initialDirectory,
				FileName         = filename,
				DefaultExt       = ext,
				Title            = title,
				OwnerWindow      = parent,
			};

			var filter = new Epsitec.Common.Dialogs.FilterItem ("x", formatName, ext);
			dialog.Filters.Add (filter);
			dialog.FilterIndex = 0;

			dialog.OpenDialog ();

			if (dialog.Result == Epsitec.Common.Dialogs.DialogResult.Accept)
			{
				return dialog.FileName;
			}
			else
			{
				return null;
			}
		}
	}
}