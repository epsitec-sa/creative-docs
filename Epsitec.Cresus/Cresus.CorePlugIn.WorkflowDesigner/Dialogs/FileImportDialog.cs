//	Copyright � 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Dialogs
{
	public class FileImportDialog : AbstractFileDialog
	{
		public FileImportDialog(Widget parent)
		{
			this.parent                  = parent;
			this.title                   = "Importation d'un workflow";
			this.owner                   = this.parent.Window;
			this.InitialDirectory        = FileImportDialog.initialDirectory;
			this.InitialFileName         = FileImportDialog.initialFilename;
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.fileDialogType          = FileDialogType.Open;

			this.Filters.Add (new FilterItem ("x", "Fichier XML", ".xml"));
		}


		public void PathMemorize()
		{
			FileImportDialog.initialDirectory = System.IO.Path.GetDirectoryName (this.FileName);
			FileImportDialog.initialFilename  = System.IO.Path.GetFileName (this.FileName);
		}


		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les fronti�res de l'application.
			var w = this.parent.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override IFavoritesSettings FavoritesSettings
		{
			get
			{
				return FileImportDialog.settings;
			}
		}


		public class Settings : IFavoritesSettings
		{
			public Settings()
			{
				this.favoritesBig = true;
				this.favoritesList = new List<string> ();
			}


			#region IFavoritesSettings Members

			public bool UseLargeIcons
			{
				get
				{
					return this.favoritesBig;
				}
				set
				{
					this.favoritesBig = value;
				}
			}

			public IList<string> Items
			{
				get
				{
					return this.favoritesList;
				}
			}

			#endregion
			
			
			protected bool							favoritesBig;
			protected List<string>					favoritesList;
		}


		//	Tous les r�glages sont conserv�s dans des variables statiques,
		//	ce qui est une solution rapide et provisoire !
		private static Settings			settings = new Settings ();
		private static string			initialDirectory = null;
		private static string			initialFilename = null;

		private readonly Widget			parent;
	}
}
